using Npgsql;

namespace Ocluse.LiquidSnow.Jobs.Postgres;

/// <summary>
/// Configures the PostgreSQL job store.
/// </summary>
public sealed class PostgresJobsOptions
{
    /// <summary>
    /// Gets or sets the schema containing the jobs table.
    /// </summary>
    public string Schema { get; set; } = "liquidsnow_jobs";

    /// <summary>
    /// Gets or sets whether the schema and table should be created during scheduler startup.
    /// Keep disabled when schema changes are managed by deployment migrations.
    /// </summary>
    public bool CreateSchemaIfNotExists { get; set; }

    internal string? ConnectionString { get; set; }

    internal NpgsqlDataSource? DataSource { get; set; }
}
