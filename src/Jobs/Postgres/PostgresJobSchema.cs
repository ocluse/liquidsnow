using System.Text.RegularExpressions;

namespace Ocluse.LiquidSnow.Jobs.Postgres;

/// <summary>
/// Provides the PostgreSQL schema used by the durable job store.
/// </summary>
public static partial class PostgresJobSchema
{
    /// <summary>
    /// Creates an idempotent schema initialization script.
    /// </summary>
    public static string GetCreateScript(string schema = "liquidsnow_jobs")
    {
        ValidateName(schema);
        string qualifiedTable = $"\"{schema}\".\"jobs\"";

        return $$"""
            CREATE SCHEMA IF NOT EXISTS "{{schema}}";

            CREATE SEQUENCE IF NOT EXISTS "{{schema}}"."jobs_sequence";

            CREATE TABLE IF NOT EXISTS {{qualifiedTable}} (
                scope text NOT NULL,
                id text NOT NULL,
                type_name text NOT NULL,
                payload jsonb NOT NULL,
                schedule_kind smallint NOT NULL,
                due_at timestamptz NOT NULL,
                interval_ticks bigint NULL,
                tick bigint NOT NULL,
                sequence bigint NOT NULL DEFAULT nextval('"{{schema}}"."jobs_sequence"'::regclass),
                version bigint NOT NULL DEFAULT 0,
                lease_owner text NULL,
                lease_expires_at timestamptz NULL,
                attempts integer NOT NULL DEFAULT 0,
                last_error text NULL,
                created_at timestamptz NOT NULL DEFAULT now(),
                updated_at timestamptz NOT NULL DEFAULT now(),
                CONSTRAINT jobs_pk PRIMARY KEY (scope, id)
            );

            CREATE INDEX IF NOT EXISTS jobs_due_idx
                ON {{qualifiedTable}} (due_at, sequence)
                WHERE lease_owner IS NULL;

            CREATE INDEX IF NOT EXISTS jobs_lease_idx
                ON {{qualifiedTable}} (lease_expires_at)
                WHERE lease_owner IS NOT NULL;

            CREATE INDEX IF NOT EXISTS jobs_queue_idx
                ON {{qualifiedTable}} (scope, due_at, sequence)
                WHERE scope <> '';
            """;
    }

    internal static string GetQualifiedTable(string schema)
    {
        ValidateName(schema);
        return $"\"{schema}\".\"jobs\"";
    }

    private static void ValidateName(string schema)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(schema);
        if (!SchemaNameExpression().IsMatch(schema))
        {
            throw new ArgumentException(
                "The PostgreSQL jobs schema may contain only letters, digits, and underscores, and cannot start with a digit.",
                nameof(schema));
        }
    }

    [GeneratedRegex("^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant)]
    private static partial Regex SchemaNameExpression();
}
