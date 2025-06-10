using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Kit.Components.Controls;
public class ModalBottomSheet : ModalBase
{
    protected override void BuildClass(ClassBuilder builder)
    {
        builder.Add("modal-bottom-sheet");
    }
}
