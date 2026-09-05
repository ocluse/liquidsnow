# Fixed-point numerics

`Fix64` is a signed Q32.32 number: its numeric value is `RawValue / 4294967296`.
It implements `INumber<Fix64>`, `ISignedNumber<Fix64>`, and `IMinMaxValue<Fix64>`
on .NET 8. Its range is −2147483648 through 2147483648 − 2⁻³².
`Precision` is one raw bit (2⁻³²); `Epsilon` is a comparison tolerance (2⁻²⁰).

```csharp
static T Average<T>(T a, T b) where T : System.Numerics.INumber<T>
    => a / T.CreateChecked(2) + b / T.CreateChecked(2);

Fix64 value = Fix64.CreateChecked(1.25m);
decimal converted = decimal.CreateChecked(value);
```

## Arithmetic and conversions

- Ordinary addition, subtraction, multiplication, division, negation, and
  increment/decrement saturate at the representable endpoints. Checked operators
  throw `OverflowException`. Integer overloads follow the same policy.
- Multiplication, division, and conversion into Q32.32 round to nearest, with
  midpoint ties going to the even raw value. All division overloads reject zero.
- Constructors, explicit numeric conversions into `Fix64`, parsing, and
  `CreateChecked` reject values outside the range, NaN, and infinity.
  `CreateSaturating` and `CreateTruncating` clamp overflow; NaN becomes zero.
  Truncating conversion follows fractional numeric types rather than integer bit wrapping.
- Conversions to integers truncate toward zero. Generic conversions out of
  `Fix64` use decimal's checked, saturating, or truncating conversion semantics.
- `Round` and `RoundToPrecision` support every `MidpointRounding` mode. Rounding
  an endpoint to an unrepresentable integer saturates. Decimal precision is 0–9.
- `FastAdd`, `FastSub`, `FastMul`, and bit shifts retain their low-level behavior;
  they do not saturate, and `FastMul` discards fractional product bits.

## Text and serialization compatibility

`Parse` and `TryParse` now consume **numeric text**, so `Parse("1")` means one,
and `Parse(value.ToString())` preserves every raw bit. They previously interpreted
text as raw storage integers. Use `ParseRaw`/`TryParseRaw` with `RawToString` for
that representation. Default text formatting/parsing is invariant; provider and
span overloads support culture-specific text and generic math.

JSON remains a signed 64-bit **raw integer**, preserving the existing wire format.
Applications reading JSON through a double-only number type must preserve those
integers without passing through a double if exact round trips are needed.

Other intentional behavior changes include negative integer casts truncating
toward zero, nearest-even arithmetic ties, saturating increments, and rejecting
out-of-range constructors rather than relying on runtime floating-to-integer casts.

## Functions and geometry

`Pow2` saturates at exponent 31 and rounds to zero at or below −33. `Pow` retains
its positive-base logarithmic implementation (negative bases with nonzero exponents
are outside its domain). `Ln` retains its fractional result.

Trigonometry uses deterministic decimal guard digits and integer square roots,
then quantizes to Q32.32. Angle reduction uses a higher precision period, including
for large angles. `Tan` throws when the quantized cosine is zero. The decimal
series prioritize accuracy; they are not a promise of native floating-point speed.

Vector lengths, distances, normalization, and sphere distance comparisons use
wider intermediates. A final length that cannot fit still saturates. Public squared
lengths and other scalar results remain limited by the Q32.32 range; saturation is
not a substitute for arbitrary-precision geometry. Quaternion rotation preserves
the input vector's length. Range overlap depth is the length of the intersection,
and disjoint/touching ranges do not pass `CheckOverlap`.

Regression coverage in `test/Core/Numerics/FixedMathTests.cs` includes randomized
`BigInteger` arithmetic comparisons, text/JSON round trips, generic conversions,
rounding modes, transcendental sweeps and boundaries, square-root bounds, and geometry.
