using System.IO.Packaging;
using System.Text;
using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Utils;
using Ocluse.LiquidSnow.Cryptography.Symmetrics;

namespace Ocluse.LiquidSnow.Cryptography.IO.Internals
{

    internal class CryptoContainer(ISymmetric algorithm, Stream stream, byte[] key) : ICryptoContainer
    {

        #region Constructors

        #endregion

        #region Properties

        public byte[] Key { get; } = key;

        public ISymmetric Algorithm { get; } = algorithm;

        protected Package Package { get; } = Package.Open(stream, FileMode.OpenOrCreate);

        #endregion

        #region Protected Methods

        protected virtual Task<Uri> GetPartUriAsync(string name, CancellationToken cancellationToken)
        {
            Uri uri = PackUriHelper.CreatePartUri(new Uri(name, UriKind.Relative));

            return Task.FromResult(uri);
        }

        #endregion

        #region Stream IO

        protected async Task AddStreamCore(Uri uri, Stream input, bool overwrite = false, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            PackagePart part = Package.PartExists(uri)
                ? overwrite ? Package.GetPart(uri) : throw new IOException("Item already exists")
                : Package.CreatePart(uri, "");

            using Stream output = part.GetStream();
            using ICryptoFile ef = IOBuilder.CreateFile(Algorithm, Key, output);
            await ef.WriteAsync(input, progress, cancellationToken).ConfigureAwait(false);
        }

        public async Task AddStreamAsync(string name, Stream input, bool overwrite = false, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            Uri uri = await GetPartUriAsync(name, cancellationToken);
            await AddStreamCore(uri, input, overwrite, progress, cancellationToken).ConfigureAwait(false);
        }

        protected async Task GetStreamCore(Uri uri, Stream output, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            if (!Package.PartExists(uri))
            {
                throw new FileNotFoundException("Item does not exist");
            }

            using Stream input = Package.GetPart(uri).GetStream();
            using ICryptoFile ef = IOBuilder.CreateFile(Algorithm, Key, input);
            await ef.ReadAsync(output, progress, cancellationToken).ConfigureAwait(false);
        }

        public async Task GetStreamAsync(string name, Stream output, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            Uri uri = await GetPartUriAsync(name, cancellationToken);
            await GetStreamCore(uri, output, progress, cancellationToken).ConfigureAwait(false);
        }

        #endregion

        #region Byte IO

        public async Task AddBytesAsync(string name, byte[] data, bool overwrite = false, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            using MemoryStream msData = new(data);
            await AddStreamAsync(name, msData, overwrite, progress, cancellationToken).ConfigureAwait(false);
        }

        public async Task<byte[]> GetBytesAsync(string name, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            using MemoryStream msData = new();
            await GetStreamAsync(name, msData, progress, cancellationToken).ConfigureAwait(false);
            return msData.ToArray();
        }
        #endregion

        #region String IO
        public async Task AddTextAsync(string name, string contents, bool overwrite = false, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            await AddBytesAsync(name, contents.GetBytes<UTF8Encoding>(), overwrite, progress, cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> GetTextAsync(string name, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            byte[] bytes = await GetBytesAsync(name, progress, cancellationToken).ConfigureAwait(false);
            return bytes.GetString<UTF8Encoding>();
        }
        #endregion

        #region Misc Methods

        public virtual Task<List<string>> EnumerateItemsAsync(CancellationToken cancellationToken = default)
        {
            PackagePartCollection parts = Package.GetParts();

            List<string> result = [];

            foreach (var part in parts)
            {
                result.Add(part.Uri.ToString().TrimStart('/'));
            }

            return Task.FromResult(result);
        }

        public async Task ExtractContainerAsync(string outputDirectory, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(outputDirectory))
            {
                _ = Directory.CreateDirectory(outputDirectory);
            }

            var items = await EnumerateItemsAsync(cancellationToken);

            if(items.Count == 0)
            {
                return;
            }

            int index = 0;

            Progress<double>? innerProgress = new() { };

            innerProgress.ProgressChanged += (o, e) =>
            {
                double percent = (index + e) / items.Count;
                progress?.Report(percent);
            };

            if (progress == null)
            {
                innerProgress = null;
            }

            foreach (var item in items)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                string path = IOUtility.CombinePath(outputDirectory, item);

                //ensure the parent directory exists:
                string? parent = Path.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
                {
                    _ = Directory.CreateDirectory(parent);
                }

                using FileStream fs = File.OpenWrite(path);
                await GetStreamAsync(item, fs, innerProgress, cancellationToken);

                index++;
            }
        }

        public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            Uri uri = await GetPartUriAsync(name, cancellationToken);
            return Package.PartExists(uri);
        }

        public async Task<bool> DeleteAsync(string name)
        {
            Uri uri = await GetPartUriAsync(name, CancellationToken.None);

            if (Package.PartExists(uri))
            {
                try
                {
                    Package.DeletePart(uri);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public void Dispose()
        {
            Package.Close();
            stream.Dispose();
        }

        #endregion
    }
}
