using Ocluse.LiquidSnow.Cryptography.Symmetrics;
using Ocluse.LiquidSnow.Extensions;
using System.Text;

namespace Ocluse.LiquidSnow.Cryptography.IO.Internals;

internal class CryptoFile(ISymmetric algorithm, byte[] key, Stream stream) : ICryptoFile
{

    #region Private Fields

    private readonly Stream _stream = stream;

    #endregion
    #region Constructors
    #endregion

    #region Public Methods

    public async Task WriteAsync(Stream stream, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        _stream.Position = 0;
        _stream.SetLength(0);

        await algorithm.EncryptAsync(input: stream, output: _stream, key, progress, cancellationToken);
    }

    public async Task ReadAsync(Stream stream, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        _stream.Position = 0;
        await algorithm.DecryptAsync(input: _stream, output: stream, key, progress, cancellationToken);
    }

    public async Task WriteBytesAsync(byte[] buffer, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        using MemoryStream msData = new(buffer);
        await WriteAsync(msData, progress, cancellationToken);

    }

    public async Task<byte[]> ReadBytesAsync(IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        using MemoryStream ms = new();
        await ReadAsync(ms, progress, cancellationToken);
        return ms.ToArray();
    }

    public async Task WriteTextAsync(string contents, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        await WriteBytesAsync(contents.GetBytes<UTF8Encoding>(), progress, cancellationToken);
    }

    public async Task<string> ReadTextAsync(IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        byte[] data = await ReadBytesAsync(progress, cancellationToken);
        return data.GetString<UTF8Encoding>();
    }

    public void Dispose()
    {
        _stream.Dispose();
    }

    #endregion

}