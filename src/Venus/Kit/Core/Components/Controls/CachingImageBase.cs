using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Kit.Components.Controls;
public class CachingImageBase : ComponentBase
{
    [Inject]
    public ICacheStorage Cache { get; set; } = default!;
}
