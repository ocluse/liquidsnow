using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Kit.Contracts;
public interface IStorage
{
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    Task WriteAsync(string key, Stream input, CancellationToken cancellationToken = default);

    Task ReadAsync(string key, Stream output, CancellationToken cancellationToken = default);

    Task<Stream> OpenReadAsync(string key, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default);
}

public interface ICacheStorage : IStorage;

public interface IPersistedStorage : IStorage;