using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Ocluse.LiquidSnow.Jobs.Persistence;
using Ocluse.LiquidSnow.Jobs.Postgres;

namespace Ocluse.LiquidSnow.DependencyInjection;

/// <summary>
/// Adds PostgreSQL persistence to the jobs subsystem.
/// </summary>
public static class PostgresJobsBuilderExtensions
{
    /// <summary>
    /// Uses PostgreSQL job persistence with a provider-owned data source.
    /// </summary>
    public static JobsBuilder UsePostgres(
        this JobsBuilder builder,
        string connectionString,
        Action<PostgresJobsOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        PostgresJobsOptions options = new() { ConnectionString = connectionString };
        configure?.Invoke(options);
        return ConfigureStore(builder, options);
    }

    /// <summary>
    /// Uses PostgreSQL job persistence with a shared Npgsql data source.
    /// </summary>
    public static JobsBuilder UsePostgres(
        this JobsBuilder builder,
        NpgsqlDataSource dataSource,
        Action<PostgresJobsOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(dataSource);

        PostgresJobsOptions options = new() { DataSource = dataSource };
        configure?.Invoke(options);
        return ConfigureStore(builder, options);
    }

    private static JobsBuilder ConfigureStore(JobsBuilder builder, PostgresJobsOptions options)
    {
        _ = PostgresJobSchema.GetQualifiedTable(options.Schema);
        builder.Services.RemoveAll<IJobStore>();
        builder.Services.AddSingleton(options);
        builder.Services.TryAddSingleton<PostgresJobStore>();
        builder.Services.AddSingleton<IJobStore>(provider => provider.GetRequiredService<PostgresJobStore>());
        return builder;
    }
}
