using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Kit.Contracts;
public interface INavController
{
    void Navigate<TPage>(object? data = null) where TPage : IPage;

    void Navigate(Type pageType, object? data = null);

    void Replace<TPage>(object? data = null) where TPage : IPage;

    void Replace(Type pageType, object? data = null);

    Task<T?> ExecuteAsync<T>(Func<Task<T>> func);

    Task ExecuteAsync(Func<Task> func);

    Task<bool?> ShowDialogAsync(ManagedAlertOptions options);

    Task ShowRechargeSheetAsync();

    void SetBusy(bool state);

    void GoBack();
}
