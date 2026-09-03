# Job system architecture and implementation

This document describes the internal execution model and the requirements for implementing a persistence provider. For application-level examples, see [Jobs: application guide](jobs.md).

## Components

The system is split into four responsibilities:

1. `IJobScheduler` converts application jobs into durable records, cancels records, and runs the worker loop.
2. `IJobSerializer` maps runtime job types to stable names and JSON payloads. `IJobKeySerializer` maps job and queue IDs to stable strings.
3. `IJobStore` owns persistence, atomic claiming, queue exclusion, leases, and optimistic concurrency.
4. `IJobDispatcher` creates the handler call from a deserialized job. Each scheduler execution creates a dependency-injection scope before resolving it.

`AddJobs` wires these components into dependency injection and registers the scheduler as both a singleton `IJobScheduler` and an `IHostedService`. Unless replaced, it uses `JsonJobSerializer`, `DefaultJobKeySerializer`, and `InMemoryJobStore`.

## Record identity and representation

`JobStoreRecord` is the boundary between the scheduler and a store. A record contains:

- a serialized job ID and optional serialized queue ID;
- the stable type name and payload bytes;
- the due time and `OneTime`, `FixedRate`, or `TaskSeries` schedule kind;
- the recurrence interval and next tick;
- a durable sequence for deterministic ordering;
- lease owner, lease expiry, and optimistic version fields;
- attempt count and last execution error.

The logical primary key is `(QueueId ?? empty scope, Id)`. `StoreAsync` is an upsert: rescheduling the same logical identity replaces its payload and schedule. The PostgreSQL implementation stores the empty scope as `''` and uses `(scope, id)` as its primary key.

## Scheduling-to-completion flow

1. `ScheduleAsync` or `QueueAsync` initializes the store, serializes the runtime job and identifiers, derives the schedule kind, and calls `IJobStore.StoreAsync`.
2. The scheduler wakes its worker. The hosted service also starts the worker at application startup so already-persisted work is discovered.
3. The worker calculates its free concurrency slots and calls `ClaimDueAsync` with the current UTC time, a process-unique worker ID, the lease duration, and a bounded claim count.
4. The store atomically returns due records after assigning their lease owner and expiry, incrementing their attempts and optimistic version.
5. For every claim, the scheduler starts a lease-renewal heartbeat and dispatches the deserialized job in a new dependency-injection scope. The heartbeat runs every one third of the lease duration.
6. If renewal fails, the scheduler cancels the handler token. The versioned lease prevents that worker from committing stale completion after another worker has recovered the record.
7. On completion, a one-time record is deleted. A recurring record has its lease cleared and its due time and tick advanced.

The worker polls no less often than `PollInterval`, but shortens its wait when `GetNextDueTimeAsync` reports earlier work. Scheduling or cancelling locally signals it immediately. Store failures are logged and retried after `PollInterval`.

## Recurrence calculation

`IRoutineJob` becomes `FixedRate`. Its next due time is the previous due time plus the interval. If that timestamp has already passed, the scheduler skips enough intervals to select the first future timestamp; it does not run a catch-up burst.

`ITaskSeriesJob` becomes `TaskSeries`. Its next due time is the current time after execution plus the interval, making the interval a delay between executions.

The next tick is the previous tick plus one. Handler failure does not change these recurrence rules.

## Queue exclusion

A store must enforce both of these rules while claiming queued work:

- no record may be claimed from a queue that already has an unexpired leased record;
- among claimable records in an unblocked queue, only the earliest by `(DueAt, Sequence)` may be claimed.

This gives serial execution within each queue without globally serializing independent queues. A queue item with a later due time does not block an earlier-due item merely because it was inserted first; ordering applies among records eligible at the claim time.

## Leases, recovery, and fencing

Leases support multiple workers and process recovery:

- a record is claimable when it is due and unleased, or when its existing lease has expired;
- every successful claim identifies an owner and increments the version;
- renew and complete operations match the logical key, owner, and version;
- renewal extends the expiry without changing the version;
- rescheduling after completion clears the lease and increments the version;
- a stale worker's renew or complete operation returns `false` after another claim changes ownership or version.

This is a fencing mechanism. It prevents two workers from updating the same stored execution state, but it cannot make arbitrary handler side effects exactly once. Handlers remain responsible for idempotency.

On graceful shutdown, active handler tokens are cancelled. The scheduler attempts to release each interrupted record with its original due time and tick so another worker can claim it. If a process stops abruptly, recovery occurs after lease expiry.

## Handler dispatch and errors

Ordinary jobs resolve one `IJobHandler<T>` for the concrete type, optionally falling back through base classes. Multicast jobs resolve all matching handlers and run them sequentially or in parallel according to `ExecuteParallel`.

The scheduler asks the dispatcher to throw handler exceptions. The dispatcher first publishes `JobFailedEvent`, then the scheduler captures the exception text for persistence. Completion behavior is intentionally occurrence-based:

- a failed one-time occurrence is removed;
- a failed recurring occurrence is rescheduled normally with `LastError` populated;
- a process interruption or lost lease leaves the occurrence recoverable and may produce another delivery.

Deserialization and other scheduler-level execution errors are also captured, but do not pass through `JobFailedEvent`. A deployment should retain registrations for all type names that can still exist in its store.

## Implement `IJobStore`

Register a custom singleton `IJobStore` after `AddJobs`, replacing the default registration. The implementation must be safe for its intended topology; a multi-instance provider needs cross-process atomicity, not only in-process locks.

### `InitializeAsync`

Prepare or validate required resources. This method can be reached concurrently through hosted startup and a scheduling call, although the scheduler serializes initialization within one instance. Schema creation should be idempotent. Prefer deployment migrations in production.

### `StoreAsync`

Atomically add or replace the record at `(QueueId, Id)`. On replacement:

- update payload and scheduling fields;
- assign a new durable ordering sequence;
- invalidate any prior owner by clearing its lease and advancing the version;
- reset attempts and last error.

The sequence must be monotonic within the store domain used for ordering.

### `CancelAsync`

Delete the exact logical key and return whether a record was removed. Deleting a leased record is valid and fences its worker because its later renewal or completion can no longer match.

### `ClaimDueAsync`

In one atomic transaction or equivalent operation:

1. find unleased records due at or before `request.Now`, plus records whose leases expired at or before that time;
2. apply queue exclusion and head selection;
3. order candidates by `(DueAt, Sequence)` and limit them to `MaximumCount`;
4. set `LeaseOwner`, set `LeaseExpiresAt` to `Now + LeaseDuration`, increment `Attempts` and `Version`;
5. return the updated records, including their new versions.

Concurrent claimers must not receive the same active version. PostgreSQL implements this with a CTE, `FOR UPDATE ... SKIP LOCKED`, and an update that returns the leased rows.

### `RenewLeaseAsync`

Update expiry only when ID, queue ID, worker ID, and version all match. Do not increment the version during renewal: the scheduler continues using the lease value returned by the claim. Return `false` when ownership was lost.

### `CompleteAsync`

Match the same four lease fields. When `NextDueAt` is `null`, delete the record. Otherwise update the due time, tick, and last error, clear the lease, and advance the version. Return `false` for a stale lease.

### `GetNextDueTimeAsync`

Return the minimum claim-relevant timestamp: `DueAt` for unleased records and `LeaseExpiresAt` for leased records. Return `null` when the store is empty. This is a scheduling hint; correctness must still come from `ClaimDueAsync`.

## Implement serializers

A custom `IJobSerializer` must produce stable type names and self-contained payloads and must reject unknown or invalid data clearly. It is a singleton, so its implementation must be thread-safe. Type-name changes need backward-compatible aliases or a data migration.

A custom `IJobKeySerializer` must be deterministic across processes, cultures, versions, and restarts. Include type information when different ID types could otherwise collide. Its output is persisted and indexed, so keep it reasonably compact and treat format changes as a storage migration.

## Verification checklist

A persistence provider should have tests that demonstrate:

- store, replace, cancel, and one-time completion behavior;
- deterministic due-time and insertion ordering;
- one active claim per queue and concurrency across different queues;
- expired-lease recovery;
- rejection of renewals and completions from stale workers;
- recurrence rescheduling with tick and error updates;
- atomic, non-overlapping claims from concurrent workers;
- initialization idempotence and safe schema-name handling;
- UTC timestamp round-tripping and cancellation-token propagation.

The in-memory tests in `test/Core/Jobs/JobsTests.cs` and PostgreSQL tests in `test/Jobs/Postgres/PostgresJobStoreTests.cs` are useful behavioral references.
