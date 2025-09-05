#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.CompilerServices;

namespace Ocluse.LiquidSnow.Numerics.Extensions;

public static class Fix4x4Extensions
{
    #region Extraction, and Setters

    /// <inheritdoc cref="Fix4x4.ExtractLossyScale(Fix4x4)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 ExtractLossyScale(this Fix4x4 matrix)
    {
        return Fix4x4.ExtractLossyScale(matrix);
    }

    /// <inheritdoc cref="Fix4x4.SetGlobalScale(Fix4x4, FixVector3)" />
    public static Fix4x4 SetGlobalScale(this ref Fix4x4 matrix, FixVector3 globalScale)
    {
        return matrix = Fix4x4.SetGlobalScale(matrix, globalScale);
    }

    /// <inheritdoc cref="Fix4x4.SetTranslation(Fix4x4, FixVector3)" />
    public static Fix4x4 SetTranslation(this ref Fix4x4 matrix, FixVector3 position)
    {
        return matrix = Fix4x4.SetTranslation(matrix, position);
    }

    /// <inheritdoc cref="Fix4x4.SetRotation(Fix4x4, FixQuaternion)" />
    public static Fix4x4 SetRotation(this ref Fix4x4 matrix, FixQuaternion rotation)
    {
        return matrix = Fix4x4.SetRotation(matrix, rotation);
    }

    /// <inheritdoc cref="Fix4x4.TransformPoint(Fix4x4, FixVector3)" />
    public static FixVector3 TransformPoint(this Fix4x4 matrix, FixVector3 point)
    {
        return Fix4x4.TransformPoint(matrix, point);
    }

    /// <inheritdoc cref="Fix4x4.InverseTransformPoint(Fix4x4, FixVector3)" />
    public static FixVector3 InverseTransformPoint(this Fix4x4 matrix, FixVector3 point)
    {
        return Fix4x4.InverseTransformPoint(matrix, point);
    }

    #endregion
}