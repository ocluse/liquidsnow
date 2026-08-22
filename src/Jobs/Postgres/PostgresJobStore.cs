using System.Text;
using Npgsql;
using NpgsqlTypes;
using Ocluse.LiquidSnow.Jobs.Persistence;

namespace Ocluse.LiquidSnow.Jobs.Postgres;

internal sealed class PostgresJobStore : IJobStore, IAsyncDisposable
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly PostgresJobsOptions _options;
    private readonly string _table;
    private readonly bool _ownsDataSource;

    public PostgresJobStore(PostgresJobsOptions options)
    {
        _options = options;
        _table = PostgresJobSchema.GetQualifiedTable(options.Schema);

        if (options.DataSource is not null)
        {
            _dataSource = options.DataSource;
        }
        else if (!string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            _dataSource = NpgsqlDataSource.Create(options.ConnectionString);
            _ownsDataSource = true;
        }
        else
        {
            throw new InvalidOperationException("A PostgreSQL connection string or NpgsqlDataSource is required.");
        }
    }

    public async ValueTask InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.CreateSchemaIfNotExists)
        {
            return;
        }

        await using NpgsqlCommand command = _dataSource.CreateCommand(PostgresJobSchema.GetCreateScript(_options.Schema));
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async ValueTask StoreAsync(JobStoreRecord job, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(job);
        string sql = $$"""
            INSERT INTO {{_table}} AS current_job (
                scope, id, type_name, payload, schedule_kind, due_at, interval_ticks, tick,
                version, lease_owner, lease_expires_at, attempts, last_error, updated_at)
            VALUES (
                @scope, @id, @type_name, @payload, @schedule_kind, @due_at, @interval_ticks, @tick,
                0, NULL, NULL, 0, NULL, now())
            ON CONFLICT (scope, id) DO UPDATE SET
                type_name = EXCLUDED.type_name,
                payload = EXCLUDED.payload,
                schedule_kind = EXCLUDED.schedule_kind,
                due_at = EXCLUDED.due_at,
                interval_ticks = EXCLUDED.interval_ticks,
                tick = EXCLUDED.tick,
                sequence = nextval('"{{_options.Schema}}"."jobs_sequence"'::regclass),
                version = current_job.version + 1,
                lease_owner = NULL,
                lease_expires_at = NULL,
                attempts = 0,
                last_error = NULL,
                updated_at = now();
            """;

        await using NpgsqlCommand command = _dataSource.CreateCommand(sql);
        AddKeyParameters(command, job.Id, job.QueueId);
        command.Parameters.AddWithValue("type_name", NpgsqlDbType.Text, job.TypeName);
        command.Parameters.AddWithValue("payload", NpgsqlDbType.Jsonb, Encoding.UTF8.GetString(job.Payload));
        command.Parameters.AddWithValue("schedule_kind", NpgsqlDbType.Smallint, (short)job.ScheduleKind);
        command.Parameters.AddWithValue("due_at", NpgsqlDbType.TimestampTz, job.DueAt.ToUniversalTime());
        command.Parameters.AddWithValue(
            "interval_ticks",
            NpgsqlDbType.Bigint,
            job.Interval is null ? DBNull.Value : job.Interval.Value.Ticks);
        command.Parameters.AddWithValue("tick", NpgsqlDbType.Bigint, job.Tick);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async ValueTask<bool> CancelAsync(
        string id,
        string? queueId,
        CancellationToken cancellationToken = default)
    {
        string sql = $"DELETE FROM {_table} WHERE scope = @scope AND id = @id;";
        await using NpgsqlCommand command = _dataSource.CreateCommand(sql);
        AddKeyParameters(command, id, queueId);
        return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
    }

    public async ValueTask<IReadOnlyList<JobStoreRecord>> ClaimDueAsync(
        JobClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        string sql = $$"""
            WITH candidates AS (
                SELECT job.scope, job.id
                FROM {{_table}} AS job
                WHERE (
                    (job.lease_owner IS NULL AND job.due_at <= @now)
                    OR (job.lease_owner IS NOT NULL AND job.lease_expires_at <= @now)
                )
                AND (
                    job.scope = ''
                    OR NOT EXISTS (
                        SELECT 1
                        FROM {{_table}} AS active
                        WHERE active.scope = job.scope
                          AND active.lease_owner IS NOT NULL
                          AND active.lease_expires_at > @now
                    )
                )
                AND (
                    job.scope = ''
                    OR job.sequence = (
                        SELECT queued.sequence
                        FROM {{_table}} AS queued
                        WHERE queued.scope = job.scope
                          AND (
                              (queued.lease_owner IS NULL AND queued.due_at <= @now)
                              OR (queued.lease_owner IS NOT NULL AND queued.lease_expires_at <= @now)
                          )
                        ORDER BY queued.due_at, queued.sequence
                        LIMIT 1
                    )
                )
                ORDER BY job.due_at, job.sequence
                FOR UPDATE OF job SKIP LOCKED
                LIMIT @maximum_count
            )
            UPDATE {{_table}} AS job
            SET lease_owner = @worker_id,
                lease_expires_at = @lease_expires_at,
                attempts = job.attempts + 1,
                version = job.version + 1,
                updated_at = now()
            FROM candidates
            WHERE job.scope = candidates.scope AND job.id = candidates.id
            RETURNING job.scope, job.id, job.type_name, job.payload::text, job.schedule_kind,
                      job.due_at, job.interval_ticks, job.tick, job.sequence, job.version,
                      job.lease_owner, job.lease_expires_at, job.attempts, job.last_error;
            """;

        await using NpgsqlCommand command = _dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("now", NpgsqlDbType.TimestampTz, request.Now.ToUniversalTime());
        command.Parameters.AddWithValue("worker_id", NpgsqlDbType.Text, request.WorkerId);
        command.Parameters.AddWithValue(
            "lease_expires_at",
            NpgsqlDbType.TimestampTz,
            (request.Now + request.LeaseDuration).ToUniversalTime());
        command.Parameters.AddWithValue("maximum_count", NpgsqlDbType.Integer, request.MaximumCount);

        List<JobStoreRecord> jobs = [];
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            jobs.Add(ReadJob(reader));
        }

        return jobs;
    }

    public async ValueTask<bool> RenewLeaseAsync(
        JobLease lease,
        DateTimeOffset leaseExpiresAt,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lease);
        string sql = $$"""
            UPDATE {{_table}}
            SET lease_expires_at = @lease_expires_at,
                updated_at = now()
            WHERE scope = @scope AND id = @id
              AND lease_owner = @worker_id AND version = @version;
            """;

        await using NpgsqlCommand command = _dataSource.CreateCommand(sql);
        AddLeaseParameters(command, lease);
        command.Parameters.AddWithValue("lease_expires_at", NpgsqlDbType.TimestampTz, leaseExpiresAt.ToUniversalTime());
        return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
    }

    public async ValueTask<bool> CompleteAsync(
        JobCompletion completion,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(completion);
        string sql;
        if (completion.NextDueAt is null)
        {
            sql = $$"""
                DELETE FROM {{_table}}
                WHERE scope = @scope AND id = @id
                  AND lease_owner = @worker_id AND version = @version;
                """;
        }
        else
        {
            sql = $$"""
                UPDATE {{_table}}
                SET due_at = @due_at,
                    tick = @tick,
                    version = version + 1,
                    lease_owner = NULL,
                    lease_expires_at = NULL,
                    last_error = @last_error,
                    updated_at = now()
                WHERE scope = @scope AND id = @id
                  AND lease_owner = @worker_id AND version = @version;
                """;
        }

        await using NpgsqlCommand command = _dataSource.CreateCommand(sql);
        AddLeaseParameters(command, completion.Lease);
        if (completion.NextDueAt is not null)
        {
            command.Parameters.AddWithValue("due_at", NpgsqlDbType.TimestampTz, completion.NextDueAt.Value.ToUniversalTime());
            command.Parameters.AddWithValue("tick", NpgsqlDbType.Bigint, completion.NextTick);
            command.Parameters.AddWithValue(
                "last_error",
                NpgsqlDbType.Text,
                completion.Error is null ? DBNull.Value : completion.Error);
        }

        return await command.ExecuteNonQueryAsync(cancellationToken) == 1;
    }

    public async ValueTask<DateTimeOffset?> GetNextDueTimeAsync(CancellationToken cancellationToken = default)
    {
        string sql = $"SELECT MIN(COALESCE(lease_expires_at, due_at)) FROM {_table};";
        await using NpgsqlCommand command = _dataSource.CreateCommand(sql);
        object? result = await command.ExecuteScalarAsync(cancellationToken);
        return result switch
        {
            null or DBNull => null,
            DateTimeOffset value => value,
            DateTime value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)),
            _ => throw new InvalidOperationException($"PostgreSQL returned an unexpected timestamp type: {result.GetType().FullName}.")
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (_ownsDataSource)
        {
            await _dataSource.DisposeAsync();
        }
    }

    private static JobStoreRecord ReadJob(NpgsqlDataReader reader)
    {
        string scope = reader.GetString(0);
        long? intervalTicks = reader.IsDBNull(6) ? null : reader.GetInt64(6);

        return new JobStoreRecord
        {
            QueueId = scope.Length == 0 ? null : scope,
            Id = reader.GetString(1),
            TypeName = reader.GetString(2),
            Payload = Encoding.UTF8.GetBytes(reader.GetString(3)),
            ScheduleKind = (JobScheduleKind)reader.GetInt16(4),
            DueAt = reader.GetFieldValue<DateTimeOffset>(5),
            Interval = intervalTicks is null ? null : TimeSpan.FromTicks(intervalTicks.Value),
            Tick = reader.GetInt64(7),
            Sequence = reader.GetInt64(8),
            Version = reader.GetInt64(9),
            LeaseOwner = reader.IsDBNull(10) ? null : reader.GetString(10),
            LeaseExpiresAt = reader.IsDBNull(11) ? null : reader.GetFieldValue<DateTimeOffset>(11),
            Attempts = reader.GetInt32(12),
            LastError = reader.IsDBNull(13) ? null : reader.GetString(13)
        };
    }

    private static void AddKeyParameters(NpgsqlCommand command, string id, string? queueId)
    {
        command.Parameters.AddWithValue("scope", NpgsqlDbType.Text, queueId ?? string.Empty);
        command.Parameters.AddWithValue("id", NpgsqlDbType.Text, id);
    }

    private static void AddLeaseParameters(NpgsqlCommand command, JobLease lease)
    {
        AddKeyParameters(command, lease.Id, lease.QueueId);
        command.Parameters.AddWithValue("worker_id", NpgsqlDbType.Text, lease.WorkerId);
        command.Parameters.AddWithValue("version", NpgsqlDbType.Bigint, lease.Version);
    }
}
