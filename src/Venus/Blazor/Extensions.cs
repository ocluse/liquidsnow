using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Blazor
{
    internal static class Extensions
    {
        public static string? GetDisplayMember<T>(this T? item, Func<T?, string>? displayMemberFunc, string? displayMemberPath)
        {
            if(displayMemberFunc != null)
            {
                return displayMemberFunc(item);
            }
            if (item == null)
            {
                return null;
            }
            if (displayMemberPath == null)
            {
                return item.ToString();
            }
            var property = item.GetType().GetProperty(displayMemberPath);
            if (property == null)
            {
                return null;
            }
            return property.GetValue(item)?.ToString();
        }
    }
}
