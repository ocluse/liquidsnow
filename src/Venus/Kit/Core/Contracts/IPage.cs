using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Kit.Contracts;
public interface IPage
{
    void OnNavigatedTo(NavigationToEventArgs e);

    void OnNavigatingTo(NavigationToEventArgs e);

    Task OnNavigatingFromAsync(NavigationFromEventArgs e);

    void OnNavigatedFrom(NavigationFromEventArgs e);
}
