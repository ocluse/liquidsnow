﻿using Ocluse.LiquidSnow.Cryptography.IO.Internals;
using Ocluse.LiquidSnow.Cryptography.Symmetrics;

namespace Ocluse.LiquidSnow.Cryptography.IO;

/// <summary>
/// Contains utility methods for creating instances of <see cref="ICryptoFile"/> and <see cref="ICryptoContainer"/>
/// </summary>
public class IOBuilder
{
    /// <summary>
    /// Creates an instance of a <see cref="ICryptoFile"/>
    /// </summary>
    /// <remarks>
    /// While this method is named "Create" it does not overwrite any existing data in the <paramref name="stream"/>.
    /// Instead, the <paramref name="stream"/> will be opened ready to be read from.
    /// </remarks>
    public static ICryptoFile CreateFile(ISymmetric algorithm, byte[] key, Stream stream)
    {
        return new CryptoFile(algorithm, key, stream);
    }

    /// <summary>
    /// Creates an instance of a <see cref="ICryptoFile"/> 
    /// </summary>
    /// <remarks>
    /// If no file exists at the specified path, a new one will be created, otherwise, the existing file will be opened
    /// </remarks>
    public static ICryptoFile CreateFile(ISymmetric algorithm, byte[] key, string path)
    {
        Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        return CreateFile(algorithm, key, stream);
    }


    /// <summary>
    /// Creates an instance of a <see cref="ICryptoContainer"/>
    /// </summary>
    /// <remarks>
    /// While this method is named "Create" it does not overwrite any existing data in the <paramref name="stream"/>.
    /// Instead, the <paramref name="stream"/> will be opened ready to be read from.
    /// </remarks>
    public static ICryptoContainer CreateContainer(ISymmetric algorithm, byte[] key, Stream stream)
    {
        return new CryptoContainer(algorithm, stream, key);
    }

    /// <summary>
    /// Creates an instance of a <see cref="ICryptoContainer"/>
    /// </summary>
    /// <remarks>
    /// If no file exists at the specified path, a new one will be created, otherwise, the existing file will be opened
    /// </remarks>
    public static ICryptoContainer CreateContainer(ISymmetric algorithm, byte[] key, string path)
    {
        Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        return CreateContainer(algorithm, key, stream);
    }

    /// <summary>
    /// Creates a managed instance of a <see cref="ICryptoContainer"/>
    /// </summary>
    /// <remarks>
    /// A managed crypto-container ensures that only a single key is used for the lifetime of the crypto-container.
    /// </remarks>
    public static ICryptoContainer CreateManagedContainer(ISymmetric algorithm, byte[] key, Stream stream)
    {
        return new ManagedCryptoContainer(algorithm, stream, key);
    }

    /// <summary>
    /// Creates a managed instance of a <see cref="ICryptoContainer"/>
    /// </summary>
    /// <remarks>
    /// A managed crypto-container ensures that only a single key is used for the lifetime of the crypto-container.
    /// If no file exists at the specified path, a new one will be created, otherwise, the existing file will be opened
    /// </remarks>
    public static ICryptoContainer CreateManagedContainer(ISymmetric algorithm, byte[] key, string path)
    {
        Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        return CreateManagedContainer(algorithm, key, stream);
    }
}
