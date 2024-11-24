using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Contracts;

internal interface IContainer
{
    RenderFragment EmptyTemplate { get; }

    RenderFragment LoadingTemplate { get; }

    RenderFragment<Exception?> ErrorTemplate { get; }

    RenderFragment UnauthorizedTemplate { get; }

    RenderFragment NotFoundTemplate { get; }
}
