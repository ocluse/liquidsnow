﻿using System.Numerics;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class NumberPicker<T> : InputControlBase<T> where T : INumber<T>
    {
        [Parameter]
        public T? Min { get; set; }

        protected override string InputType { get; } = "number";

        protected override T? GetValue(object? value)
        {
            string? s = value?.ToString();
            return s == null ? Min ?? default : T.Parse(s, null);
        }

    }
}
