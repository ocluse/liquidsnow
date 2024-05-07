using Ocluse.LiquidSnow.Cryptography.Symmetrics;
using System.Text;
using Ocluse.LiquidSnow.Extensions;
using System.Security.Cryptography;
using System.IO.Packaging;

namespace Ocluse.LiquidSnow.Cryptography.IO.Internals
{
    internal class ManagedCryptoContainer(ISymmetric algorithm, Stream stream, byte[] key) 
        : CryptoContainer(algorithm, stream, key)
    {
        private bool _keyAsserted;

        private const string _headerFileName = "header";

        private static readonly Uri _headerUri = PackUriHelper.CreatePartUri(new(_headerFileName, UriKind.Relative));

        private async Task AssertKeyAsync(CancellationToken cancellationToken)
        {
            if (!_keyAsserted)
            {
                bool headerExists = Package.PartExists(_headerUri);
                
                if (headerExists)
                {
                    try
                    {
                        _ = await GetHeaderFileAsync(cancellationToken);
                        _keyAsserted = true;
                    }
                    catch (CryptographicException)
                    {
                        throw new UnauthorizedAccessException("Invalid key");
                    }
                }
                else
                {
                    //Ensure that there are no items in the file:
                    if (Package.GetParts().Any())
                    {
                        throw new InvalidOperationException("Invalid crypto-container");
                    }
                    else
                    {
                        //Write the header file:
                        await AddHeaderFileAsync(cancellationToken);
                        _keyAsserted = true;
                    }
                }
            }
        }

        private async Task AddHeaderFileAsync(CancellationToken cancellationToken = default)
        {
            Guid guid = Guid.NewGuid();
            using MemoryStream ms = new(guid.ToByteArray());

            await AddStreamCore(_headerUri, ms, false, cancellationToken: cancellationToken);
        }

        private async Task<Guid> GetHeaderFileAsync(CancellationToken cancellationToken = default)
        {
            using MemoryStream ms = new();

            await GetStreamCore(_headerUri, ms, cancellationToken: cancellationToken);

            ms.Position = 0;

            byte[] buffer = new byte[16];
            ms.Read(buffer, 0, buffer.Length);

            return new Guid(buffer);
        }

        private async Task<string> EncryptNameAsync(string name, CancellationToken cancellationToken)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(name);

            using MemoryStream msInput = new(bytes);
            using MemoryStream msOutput = new();

            await Algorithm.EncryptAsync(msInput, msOutput, Key, cancellationToken: cancellationToken);

            string base64 = Convert.ToBase64String(msOutput.ToArray());

            //url encode the base64 string:
            return base64.ToUrlEncoded();
        }

        private async Task<string> DecryptNameAsync(string name, CancellationToken cancellationToken)
        {
            string urlDecoded = name.ToUrlDecoded();

            byte[] bytes = Convert.FromBase64String(urlDecoded);

            using MemoryStream msInput = new(bytes);
            using MemoryStream msOutput = new();

            await Algorithm.DecryptAsync(msInput, msOutput, Key, cancellationToken: cancellationToken);

            return Encoding.UTF8.GetString(msOutput.ToArray());
        }

        protected override async Task<Uri> GetPartUriAsync(string name, CancellationToken cancellationToken)
        {
            await AssertKeyAsync(cancellationToken);

            string encryptedName = await EncryptNameAsync(name, cancellationToken);

            return await base.GetPartUriAsync(encryptedName, cancellationToken);
        }

        public override async Task<List<string>> EnumerateItemsAsync(CancellationToken cancellationToken = default)
        {
            await AssertKeyAsync(cancellationToken);
            
            List<string> items = await base.EnumerateItemsAsync(cancellationToken);
            
            int headerFileIndex = -1;
            
            for(int i = 0; i < items.Count; i++)
            {
                if (items[i] != _headerFileName)
                {
                    items[i] = await DecryptNameAsync(items[i], CancellationToken.None);
                }
                else
                {
                    headerFileIndex = i;
                }
            }

            //Remove the header file:
            items.RemoveAt(headerFileIndex);

            return items;
        }
    }
}
