#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Ocluse.LiquidSnow.Numerics.Curves;

/// <summary>
/// Specifies the interpolation method used when evaluating a <see cref="FixCurve"/>.
/// </summary>
public enum FixCurveMode
{
    /// <summary>Linear interpolation between keyframes.</summary>
    Linear,

    /// <summary>Step interpolation, instantly jumping between keyframe values.</summary>
    Step,

    /// <summary>Smooth interpolation using a cosine function (SmoothStep).</summary>
    Smooth,

    /// <summary>Cubic interpolation for smoother curves using tangents.</summary>
    Cubic
}
