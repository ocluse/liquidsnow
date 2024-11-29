using System.Security.Cryptography;

namespace Ocluse.LiquidSnow.Cryptography.Symmetrics;

internal class Symmetric : ISymmetric
{
    public HashAlgorithmName Hash { get; set; }

    public EncryptionAlgorithm Algorithm { get; set; }

    public PaddingMode PaddingMode { get; set; } = PaddingMode.PKCS7;

    public CipherMode CipherMode { get; set; } = CipherMode.CBC;

    public required byte[] Salt { get; set; }

    public int KeySize { get; set; }

    public int Iterations { get; set; }

    public int BlockSize { get; set; }

    ReadOnlySpan<byte> ISymmetric.Salt => Salt;

    public async Task EncryptAsync(Stream input, Stream output, byte[] password, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        await RunAsync(input, output, password, true, progress, cancellationToken);
    }

    public async Task DecryptAsync(Stream input, Stream output, byte[] password, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        await RunAsync(input, output, password, false, progress, cancellationToken);
    }

    public void Run(Stream input, Stream output, byte[] password, bool forward)
    {
        using SymmetricAlgorithm alg = GetEncryptionAlgorithm();

        using Rfc2898DeriveBytes rdb = new(password, Salt, Iterations, Hash);
        
        byte[] key = rdb.GetBytes(KeySize / 8);
        byte[] iv = rdb.GetBytes(BlockSize / 8);

        alg.Mode = CipherMode;
        alg.Padding = PaddingMode;
        alg.Key = key;
        alg.IV = iv;

        ICryptoTransform trans = forward ? alg.CreateEncryptor() : alg.CreateDecryptor();
        using CryptoStream csInput = new(input, trans, CryptoStreamMode.Read, true);

        while (true)
        {
            byte[] buffer = new byte[8];
            int read = csInput.Read(buffer, 0, buffer.Length);
            if (read == 0)
            {
                break;
            }
            output.Write(buffer, 0, read);
        }
    }

    public Task RunAsync(Stream input, Stream output, byte[] password, bool forward, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        using SymmetricAlgorithm alg = GetEncryptionAlgorithm();

        using Rfc2898DeriveBytes rdb = new(password, Salt, Iterations, Hash);

        byte[] key = rdb.GetBytes(KeySize / 8);
        byte[] iv = rdb.GetBytes(BlockSize / 8);

        alg.Mode = CipherMode;
        alg.Padding = PaddingMode;
        alg.Key = key;
        alg.IV = iv;

        long workedOn = 0;
        ICryptoTransform trans = forward ? alg.CreateEncryptor() : alg.CreateDecryptor();

        using CryptoStream csInput = new(input, trans, CryptoStreamMode.Read, true);

        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            byte[] buffer = new byte[8];
            int read = csInput.Read(buffer, 0, buffer.Length);

            output.Write(buffer, 0, read);

            workedOn += read;
            if (progress != null && input.CanSeek)
            {
                progress.Report(workedOn / (double)input.Length);
            }

            if (read == 0)
            {
                break;
            }
        }

        return Task.CompletedTask;
    }

    private SymmetricAlgorithm GetEncryptionAlgorithm()
    {
        return Algorithm switch
        {
            EncryptionAlgorithm.AES => Aes.Create(),
            EncryptionAlgorithm.DES => DES.Create(),
            EncryptionAlgorithm.TripleDES => TripleDES.Create(),
            EncryptionAlgorithm.RC2 => RC2.Create(),
            _ => throw new NotImplementedException("Algorithm unknown or unsupported")
        };
    }
}
