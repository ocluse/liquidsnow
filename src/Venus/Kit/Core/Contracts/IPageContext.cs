using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Kit.Contracts;
public interface IPageContext
{
    ISnackbarService Snackbar { get; }

    Task Execute(Func<Task> func);

    Task<T?> Execute<T>(Func<Task<T>> func);

    Task<T?> Execute<T>(T? defaultValue, Func<Task<T>> func);
}
