using Ocluse.LiquidSnow.Data;

namespace Ocluse.LiquidSnow.Core.Tests.Data;

public class KeyedPagerTests
{
    [Fact]
    public async Task Refresh_DeduplicatesLoadedItemsByKey()
    {
        TestDataSource dataSource = new(
            _ => Task.FromResult(new LoadResult<int?, TestItem>
            {
                NextKey = 1,
                PreviousKey = -1,
                Items =
                [
                    new TestItem(1, "A"),
                    new TestItem(1, "A-duplicate"),
                    new TestItem(2, "B")
                ]
            }));

        KeyedPager<int?, TestItem, int> pager = new(dataSource, x => x.Id);

        await pager.RefreshAsync();

        Assert.Equal(2, pager.Items.Count);
        Assert.Equal(1, pager.Items[0].Id);
        Assert.Equal("A", pager.Items[0].Value);
        Assert.Equal(2, pager.Items[1].Id);
    }

    [Fact]
    public async Task ReachedEndAndStart_DeduplicateAgainstExistingItems()
    {
        TestDataSource dataSource = new(request =>
        {
            if (request.Type == LoadType.Refresh)
            {
                return Task.FromResult(new LoadResult<int?, TestItem>
                {
                    NextKey = 2,
                    PreviousKey = 0,
                    Items =
                    [
                        new TestItem(1, "A"),
                        new TestItem(2, "B")
                    ]
                });
            }

            if (request.Type == LoadType.Append && request.Key == 2)
            {
                return Task.FromResult(new LoadResult<int?, TestItem>
                {
                    NextKey = null,
                    PreviousKey = 1,
                    Items =
                    [
                        new TestItem(2, "B-duplicate"),
                        new TestItem(3, "C")
                    ]
                });
            }

            if (request.Type == LoadType.Prepend && request.Key == 0)
            {
                return Task.FromResult(new LoadResult<int?, TestItem>
                {
                    NextKey = 1,
                    PreviousKey = -1,
                    Items =
                    [
                        new TestItem(0, "Z"),
                        new TestItem(1, "A-duplicate")
                    ]
                });
            }

            return Task.FromResult(LoadResult<int?, TestItem>.Empty());
        });

        KeyedPager<int?, TestItem, int> pager = new(dataSource, x => x.Id, supportsPrepending: true);

        await pager.RefreshAsync();
        pager.ReachedEnd();
        await WaitUntilAsync(() => pager.Items.Count == 3);

        pager.ReachedStart();
        await WaitUntilAsync(() => pager.Items.Count == 4);

        Assert.Equal([0, 1, 2, 3], pager.Items.Select(x => x.Id).ToArray());
    }

    [Fact]
    public void MutableKeyedPager_AddOrUpdateAndForceAdd_KeepItemsUnique()
    {
        TestDataSource dataSource = new(_ => Task.FromResult(LoadResult<int?, TestItem>.Empty()));
        MutableKeyedPager<int?, TestItem, int> pager = new(dataSource, x => x.Id);

        pager.AddOrUpdate(new TestItem(10, "first"));
        pager.AddOrUpdate(new TestItem(10, "replaced"));

        Assert.Single(pager.Items);
        Assert.Equal("replaced", pager.Items[0].Value);

        pager.AddOrUpdate(new TestItem(20, "second"));
        pager.ForceAdd(new TestItem(20, "moved"), atIndex: 0);

        Assert.Equal(2, pager.Items.Count);
        Assert.Equal(20, pager.Items[0].Id);
        Assert.Equal("moved", pager.Items[0].Value);
        Assert.Equal(10, pager.Items[1].Id);
    }

    [Fact]
    public async Task MutableKeyedPager_QueuesMutationsDuringLoad()
    {
        TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource release = new(TaskCreationOptions.RunContinuationsAsynchronously);

        TestDataSource dataSource = new(async request =>
        {
            if (request.Type == LoadType.Refresh)
            {
                started.TrySetResult();
                await release.Task;

                return new LoadResult<int?, TestItem>
                {
                    NextKey = 1,
                    PreviousKey = -1,
                    Items = [new TestItem(1, "server")]
                };
            }

            return LoadResult<int?, TestItem>.Empty();
        });

        MutableKeyedPager<int?, TestItem, int> pager = new(dataSource, x => x.Id);

        var refreshTask = pager.RefreshAsync();
        await started.Task;

        pager.AddOrUpdate(new TestItem(1, "local"));

        release.TrySetResult();
        await refreshTask;

        Assert.Single(pager.Items);
        Assert.Equal("local", pager.Items[0].Value);
    }

    [Fact]
    public async Task LoadConflictStrategy_Replace_ReplacesExistingItem()
    {
        TestDataSource dataSource = new(request =>
        {
            if (request.Type == LoadType.Refresh)
            {
                return Task.FromResult(new LoadResult<int?, TestItem>
                {
                    NextKey = 2,
                    PreviousKey = -1,
                    Items = [new TestItem(1, "A")]
                });
            }

            if (request.Type == LoadType.Append)
            {
                return Task.FromResult(new LoadResult<int?, TestItem>
                {
                    NextKey = null,
                    PreviousKey = 1,
                    Items = [new TestItem(1, "A-updated")]
                });
            }

            return Task.FromResult(LoadResult<int?, TestItem>.Empty());
        });

        KeyedPager<int?, TestItem, int> pager = new(
            dataSource,
            x => x.Id,
            loadConflictStrategy: ConflictStrategy.Replace);

        await pager.RefreshAsync();
        pager.ReachedEnd();
        await WaitUntilAsync(() => pager.Items.Count == 1 && pager.Items[0].Value == "A-updated");

        Assert.Single(pager.Items);
        Assert.Equal("A-updated", pager.Items[0].Value);
    }

    [Fact]
    public async Task LoadConflictStrategy_Error_SetsErrorStateOnDuplicate()
    {
        TestDataSource dataSource = new(_ => Task.FromResult(new LoadResult<int?, TestItem>
        {
            NextKey = null,
            PreviousKey = null,
            Items =
            [
                new TestItem(1, "A"),
                new TestItem(1, "A-duplicate")
            ]
        }));

        KeyedPager<int?, TestItem, int> pager = new(
            dataSource,
            x => x.Id,
            loadConflictStrategy: ConflictStrategy.Error);

        await pager.RefreshAsync();

        Assert.Equal(LoadState.Error, pager.State.Refresh);
    }

    private static async Task WaitUntilAsync(Func<bool> condition, int timeoutMs = 2000)
    {
        var timeoutAt = DateTime.UtcNow.AddMilliseconds(timeoutMs);
        while (!condition())
        {
            if (DateTime.UtcNow >= timeoutAt)
            {
                throw new TimeoutException("Timed out waiting for condition.");
            }

            await Task.Delay(10);
        }
    }

    private sealed class TestDataSource(Func<LoadRequest<int?>, Task<LoadResult<int?, TestItem>>> load) : IDataSource<int?, TestItem>
    {
        public Task<int?> GetRefreshKeyAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<int?>(0);
        }

        public Task<LoadResult<int?, TestItem>> LoadAsync(LoadRequest<int?> request, CancellationToken cancellationToken = default)
        {
            return load(request);
        }
    }

    private sealed record TestItem(int Id, string Value);
}
