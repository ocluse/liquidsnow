using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Contracts;
internal interface IDialogHandle
{

}

public record DialogProperties
{
    public string? Title { get; init; }

    public bool ShowClose { get; init; }

}
