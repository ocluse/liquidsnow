using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Kit.Models;
public record ManagedAlertOptions
{
    public required string Title { get; init; }

    public required string Text { get; init; }

    public string? Icon { get; init; }

    public required string ConfirmText { get; init; }

    public string? DismissText { get; init; }
}

