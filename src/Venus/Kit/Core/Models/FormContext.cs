using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Kit.Models;
public record FormContext(bool Enabled, Func<Task> Submit)
{
    public bool Disabled => !Enabled;
}