using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Ocluse.LiquidSnow.DependencyInjection;
using Ocluse.LiquidSnow.Jobs;
using Ocluse.LiquidSnow.Jobs.Persistence;
using Ocluse.LiquidSnow.Jobs.Postgres;
using Xunit;

namespace Ocluse.LiquidSnow.Jobs.Postgres.Tests;

public sealed class PostgresJobStoreTests
{
    [Fact]
    public void Schema_script_is_idempotent_and_rejects_unsafe_names()
    {
        string script = PostgresJobSchema.GetCreateScript("scheduled_jobs");

        Assert.Contains("CREATE SCHEMA IF NOT EXISTS \"scheduled_jobs\"", script, StringComparison.Ordinal);
        Assert.Contains("CREATE TABLE IF NOT EXISTS \"scheduled_jobs\".\"jobs\"", script, StringComparison.Ordinal);
        Assert.Throws<ArgumentException>(() => PostgresJobSchema.GetCreateScript("public; DROP TABLE users"));
    }

    [Fact]
    public async Task Store_claim_and_completion_work_when_postgres_is_configured()
    {
        string? connectionString = Environment.GetEnvironmentVariable("LIQUIDSNOW_POSTGRES_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        string schema = $"liquidsnow_jobs_test_{Guid.NewGuid():N}";
        await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);

        try
        {
            ServiceCollection services = new();
            services.AddJobs(typeof(PostgresJobStoreTests).Assembly)
                .UsePostgres(dataSource, options =>
                {
                    options.Schema = schema;
                    options.CreateSchemaIfNotExists = true;
                });

            await using ServiceProvider provider = services.BuildServiceProvider();
            IJobStore store = provider.GetRequiredService<IJobStore>();
            await store.InitializeAsync();

            DateTimeOffset now = DateTimeOffset.UtcNow;
            await store.StoreAsync(new JobStoreRecord
            {
                Id = "string:integration",
                TypeName = "integration.test",
                Payload = "{}"u8.ToArray(),
                ScheduleKind = JobScheduleKind.OneTime,
                DueAt = now,
                Tick = 0
            });

            JobStoreRecord claimed = Assert.Single(await store.ClaimDueAsync(
                new JobClaimRequest(now, "integration-worker", TimeSpan.FromMinutes(1), 1)));
            Assert.Equal("string:integration", claimed.Id);

            Assert.True(await store.CompleteAsync(new JobCompletion(
                new JobLease(claimed.Id, claimed.QueueId, "integration-worker", claimed.Version),
                null,
                1,
                null)));
            Assert.Empty(await store.ClaimDueAsync(
                new JobClaimRequest(now.AddMinutes(2), "integration-worker", TimeSpan.FromMinutes(1), 1)));
        }
        finally
        {
            await using NpgsqlCommand cleanup = dataSource.CreateCommand($"DROP SCHEMA IF EXISTS \"{schema}\" CASCADE;");
            await cleanup.ExecuteNonQueryAsync();
        }
    }
}
