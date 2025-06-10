using Microsoft.Extensions.Logging;
using Ocluse.LiquidSnow.Data;
using Ocluse.LiquidSnow.Venus.Kit.Components.Controls;
using Ocluse.LiquidSnow.Venus.Kit.Components.Managed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ocluse.LiquidSnow.Venus.Kit.Components.Navigation;
public sealed class NavHost : ComponentBase, IDisposable, INavController, ISystemBackReceiver
{
    private readonly List<PageEntry> _pages = [];

    private TaskCompletionSource? _tcsRender;
    private bool _shouldWaitForRender;
    private ManagedAlertDialog _managedAlert = default!;
    private ManagedRechargeBottomSheet _managedRecharge = default!;

    private bool _isNavigating, _isBusy;

    [Inject]
    public SystemBackInterceptor SystemBackInterceptor { get; set; } = null!;

    [Inject]
    public IVenusKitHost AppService { get; set; } = null!;

    [Inject]
    private ILogger<NavHost> Logger { get; set; } = null!;

    [Inject]
    private ISnackbarService Snackbar { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public Type? DefaultInitialPage { get; set; }

    private PageEntry? CurrentPage => _pages.Count > 0 ? _pages[^1] : null;

    protected override void OnInitialized()
    {
        SystemBackInterceptor.Bind(this);
        AppService.IntentReceived += OnIntentReceived;
        Type? initialPage;
        object? initialData;

        if (AppService.LaunchIntent is NavigateVenusKitIntent navigateIntent)
        {
            initialPage = navigateIntent.Page;
            initialData = navigateIntent.Data;
        }
        else
        {
            initialPage = DefaultInitialPage;
            initialData = null;
        }

        if (initialPage != null)
        {
            if (typeof(IPage).IsAssignableFrom(initialPage) == false)
            {
                throw new InvalidOperationException("The provided initial page does not implement IPage.");
            }

            Navigate(initialPage, initialData);
        }
        else
        {
            throw new InvalidOperationException("InitialPage must be set.");
        }
    }

    private void OnIntentReceived(VenusKitIntent intent)
    {
        if (intent is NavigateVenusKitIntent navigateIntent)
        {
            Navigate(navigateIntent.Page, navigateIntent.Data);
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (_shouldWaitForRender && _tcsRender != null && !_tcsRender.Task.IsCompleted)
        {
            _tcsRender.SetResult();
        }
    }

    public async Task<bool?> ShowDialogAsync(ManagedAlertOptions options)
    {
        if (_managedAlert == null)
        {
            throw new InvalidOperationException("ManagedAlertDialog is not initialized.");
        }

        return await _managedAlert.ShowDialogAsync(options);
    }

    public async Task ShowRechargeSheetAsync()
    {
        await _managedRecharge.ShowAsync();
    }

    public void Navigate<TPage>(object? data = null) where TPage : IPage
    {
        Navigate(typeof(TPage), data);
    }

    public void Navigate(Type pageType, object? data = null)
    {
        _ = NavigateAsync(pageType, data);
    }

    public void Replace<TPage>(object? data = null) where TPage : IPage
    {
        _ = ReplaceAsync(typeof(TPage), data);
    }

    public void Replace(Type pageType, object? data = null)
    {
        _ = ReplaceAsync(pageType, data);
    }

    public void GoBack()
    {
        _ = GoBackAsync();
    }

    private async Task NavigateAsync(Type pageType, object? data = null)
    {
        PageEntry toPage = PageEntry.Create(pageType, data);
        PageEntry? fromPage = CurrentPage;

        await PerformNavigation(NavigationType.Push, fromPage, toPage);
    }

    private async Task ReplaceAsync(Type pageType, object? data = null)
    {
        PageEntry toPage = PageEntry.Create(pageType, data);
        PageEntry? fromPage = CurrentPage;

        if (fromPage != null)
        {
            await PerformNavigation(NavigationType.Replace, fromPage, toPage);
        }
        else
        {
            await PerformNavigation(NavigationType.Push, null, toPage);
        }
    }

    private async Task GoBackAsync()
    {
        if (_pages.Count < 2)
        {
            //We still need to call onNavigatingFrom to clean up the current page:
            var instance = CurrentPage?.Instance;

            if (instance != null)
            {
                var args = new NavigationFromEventArgs
                {
                    Data = null,
                    Type = NavigationType.Pop,
                    Destination = null
                };

                await instance.OnNavigatingFromAsync(args);

                if (!args.Cancelled)
                {
                    if (DefaultInitialPage != null && instance.GetType() != DefaultInitialPage)
                    {
                        //Navigate to the default page:
                        await NavigateAsync(DefaultInitialPage, null);
                    }
                    else
                    {
                        AppService.RequestExit();
                    }
                }
            }
        }
        else
        {
            var toPage = _pages[^2];

            var fromPage = _pages[^1];

            await PerformNavigation(NavigationType.Pop, fromPage, toPage);
        }
    }

    private async Task PerformNavigation(NavigationType type, PageEntry? fromPage, PageEntry toPage)
    {
        if (_isNavigating)
        {
            return;
        }

        _isNavigating = true;

        NavigationFromEventArgs navigationFromArgs = new()
        {
            Data = toPage.Data,
            Destination = toPage.PageType,
            Type = type,
        };

        if (fromPage?.Instance != null)
        {
            await fromPage.Instance.OnNavigatingFromAsync(navigationFromArgs);
        }

        if (navigationFromArgs.Cancelled)
        {
            _isNavigating = false;
            return;
        }

        NavigationToEventArgs navigationToArgs = new()
        {
            Data = toPage.Data,
            Type = type,
        };

        //add the to page to the stack if this is a first time navigation:
        if (type is NavigationType.Push or NavigationType.Replace)
        {
            _pages.Add(toPage);
        }

        //apply navigating states:
        toPage.State = PageState.NavigatingTo;
        toPage.NavigationType = type;
        if (fromPage != null)
        {
            fromPage.NavigationType = type;
            fromPage.State = PageState.NavigatingFrom;
        }
        await WaitForRenderAsync();

        //notify the page we are heading to it:
        toPage.EnsuredInstance.OnNavigatingTo(navigationToArgs);

        //delay for the animation to finish:
        const int delay = 300;
        await Task.Delay(delay);

        //apply navigated states and modify stack:
        toPage.State = PageState.NavigatedTo;
        toPage.NavigationType = null;
        if (fromPage != null)
        {
            fromPage.NavigationType = null;
            fromPage.State = PageState.NavigatedFrom;

            if (type is NavigationType.Pop or NavigationType.Replace)
            {
                _pages.Remove(fromPage);
            }
        }

        await WaitForRenderAsync();

        //if the from page still exists, notify it we just left:
        if (fromPage?.Instance != null)
        {
            fromPage.Instance.OnNavigatedFrom(navigationFromArgs);
        }

        _isNavigating = false;
        SetBusy(false);

        //notify the page we have arrived:
        toPage.EnsuredInstance.OnNavigatedTo(navigationToArgs);

        if (navigationToArgs.DataConsumed)
        {
            toPage.Data = null;
        }
    }

    private async Task WaitForRenderAsync()
    {
        _tcsRender = new();
        _shouldWaitForRender = true;

        await InvokeAsync(StateHasChanged);

        await _tcsRender.Task;
        _shouldWaitForRender = false;
        _tcsRender = null;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<NavHost>>(0);
        {
            builder.AddAttribute(1, nameof(CascadingValue<NavHost>.Value), this);
            builder.AddAttribute(2, nameof(CascadingValue<NavHost>.IsFixed), true);
            builder.AddAttribute(3, nameof(CascadingValue<NavHost>.ChildContent), (RenderFragment)(builder2 =>
            {
                builder.OpenRegion(4);
                Render(builder2);
                builder.CloseRegion();
            }));


        }
        builder.CloseComponent();
    }

    private void Render(RenderTreeBuilder builder)
    {

        builder.OpenElement(0, "div");
        {
            builder.AddAttribute(1, "class", "nav-host");
            foreach (var entry in _pages)
            {
                builder.OpenComponent<PageRenderer>(2);
                builder.SetKey(entry.Id);
                builder.AddAttribute(3, nameof(PageRenderer.Entry), entry);
                builder.CloseComponent();
            }
        }
        builder.CloseElement();

        builder.OpenComponent<ManagedAlertDialog>(4);
        {
            builder.AddComponentReferenceCapture(5, item =>
            {
                _managedAlert = (ManagedAlertDialog)item;
            });
        }
        builder.CloseComponent();

        builder.OpenComponent<ManagedRechargeBottomSheet>(6);
        {
            builder.AddComponentReferenceCapture(7, item =>
            {
                _managedRecharge = (ManagedRechargeBottomSheet)item;
            });
        }
        builder.CloseComponent();

        builder.OpenComponent<LoadingOverlay>(8);
        {
            builder.AddAttribute(9, nameof(LoadingOverlay.IsBusy), _isBusy);
        }
        builder.CloseComponent();
    }

    void IDisposable.Dispose()
    {
        SystemBackInterceptor.Unbind(this);
        AppService.IntentReceived -= OnIntentReceived;
    }

    public void SetBusy(bool state)
    {
        if (_isBusy == state)
        {
            return;
        }

        _isBusy = state;
        InvokeAsync(StateHasChanged);
    }

    public async Task<T?> ExecuteAsync<T>(Func<Task<T>> func)
    {
        try
        {
            SetBusy(true);
            var result = await func();
            return result;
        }
        catch (Exception ex)
        {
            HandleExecutionException(ex);
            return default;
        }
        finally
        {
            SetBusy(false);
        }
    }

    public async Task ExecuteAsync(Func<Task> func)
    {
        try
        {
            SetBusy(true);
            await func();
        }
        catch (Exception ex)
        {
            HandleExecutionException(ex);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void HandleExecutionException(Exception ex)
    {
        Logger.LogError(ex, "Execution of a task on NavHost failed with an exception");
        Snackbar.AddError("Something went wrong!");
    }

    public bool HandleBackPressed()
    {
        if (_pages.Count == 0)
        {
            return false;
        }
        else if (_pages.Count == 1 && _pages[0].GetType() == DefaultInitialPage)
        {
            //only the default initial page should make us handle it as usual
            return false;
        }
        else
        {
            GoBack();
            return true;
        }
    }
}

