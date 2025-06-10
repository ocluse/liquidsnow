using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Kit.Components.Controls;

public class Dialog : ModalBase
{
    [Parameter]
    public string? Class { get; set; }

    protected override string ContentAreaClass => $"{base.ContentAreaClass} {Class}".Trim();

    protected override void BuildClass(ClassBuilder builder)
    {
        builder.Add("dialog");
    }
}