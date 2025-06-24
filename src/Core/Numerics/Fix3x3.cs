#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Ocluse.LiquidSnow.Numerics.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics;

/// <summary>
/// Represents a 3x3 matrix used for linear transformations in 2D and 3D space, such as rotation, scaling, and shearing.
/// </summary>
[Serializable]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct Fix3x3(
    Fix64 m00, Fix64 m01, Fix64 m02,
    Fix64 m10, Fix64 m11, Fix64 m12,
    Fix64 m20, Fix64 m21, Fix64 m22
    ) : IEquatable<Fix3x3>
{
    #region Fields and Constants

    [JsonInclude]
    public Fix64 M00 = m00;

    [JsonInclude]
    public Fix64 M01 = m01;

    [JsonInclude]
    public Fix64 M02 = m02;

    [JsonInclude]
    public Fix64 M10 = m10;

    [JsonInclude]
    public Fix64 M11 = m11;

    [JsonInclude]
    public Fix64 M12 = m12;

    [JsonInclude]
    public Fix64 M20 = m20;

    [JsonInclude]
    public Fix64 M21 = m21;

    [JsonInclude]
    public Fix64 M22 = m22;

    /// <summary>
    /// Returns the identity matrix (no scaling, rotation, or translation).
    /// </summary>
    public static readonly Fix3x3 Identity = new(new FixVector3(1f, 0f, 0f), new FixVector3(0f, 1f, 0f), new FixVector3(0f, 0f, 1f));

    /// <summary>
    /// Returns a matrix with all elements set to zero.
    /// </summary>
    public static readonly Fix3x3 Zero = new(new FixVector3(0f, 0f, 0f), new FixVector3(0f, 0f, 0f), new FixVector3(0f, 0f, 0f));

    #endregion
    #region Constructors

    /// <summary>
    /// Initializes a new FixedMatrix3x3 using three FixedVector3 values representing the rows.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Fix3x3(
        FixVector3 m00_m01_m02,
        FixVector3 m10_m11_m12,
        FixVector3 m20_m21_m22
    ) : this(m00_m01_m02.X, m00_m01_m02.Y, m00_m01_m02.Z, m10_m11_m12.X, m10_m11_m12.Y, m10_m11_m12.Z, m20_m21_m22.X, m20_m21_m22.Y, m20_m21_m22.Z) { }

    #endregion

    #region Properties and Methods (Instance)

    public Fix64 this[int index]
    {
        readonly get
        {
            return index switch
            {
                0 => M00,
                1 => M10,
                2 => M20,
                4 => M01,
                5 => M11,
                6 => M21,
                8 => M02,
                9 => M12,
                10 => M22,
                _ => throw new IndexOutOfRangeException("Invalid matrix index!"),
            };
        }
        set
        {
            switch (index)
            {
                case 0:
                    M00 = value;
                    break;
                case 1:
                    M10 = value;
                    break;
                case 2:
                    M20 = value;
                    break;
                case 4:
                    M01 = value;
                    break;
                case 5:
                    M11 = value;
                    break;
                case 6:
                    M21 = value;
                    break;
                case 8:
                    M02 = value;
                    break;
                case 9:
                    M12 = value;
                    break;
                case 10:
                    M22 = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid matrix index!");
            }
        }
    }

    /// <inheritdoc cref="Normalize(Fix3x3)" />
    public Fix3x3 Normalize()
    {
        return this = Normalize(this);
    }

    /// <inheritdoc cref="ResetScaleToIdentity(Fix3x3)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Fix3x3 ResetScaleToIdentity()
    {
        return this = ResetScaleToIdentity(this);
    }

    /// <summary>
    /// Calculates the determinant of a 3x3 matrix.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 GetDeterminant()
    {
        return M00 * (M11 * M22 - M12 * M21) -
               M01 * (M10 * M22 - M12 * M20) +
               M02 * (M10 * M21 - M11 * M20);
    }

    /// <summary>
    /// Inverts the diagonal elements of the matrix.
    /// </summary>
    /// <remarks>
    /// protects against the case where you would have an infinite value on the diagonal, which would cause problems in subsequent computations.
    /// If m00 or m22 are zero, handle that as a special case and manually set the inverse to zero,
    /// since for a theoretical object with no inertia along those axes, it would be impossible to impart a rotation in those directions
    ///
    ///  bear in mind that having a zero on the inertia tensor's diagonal isn't generally valid for real,
    ///  3-dimensional objects (unless they are "infinitely thin" along one axis),
    ///  so if you end up with such a tensor, it's a sign that something else might be wrong in your setup.        
    /// </remarks>
    public readonly Fix3x3 InvertDiagonal()
    {
        if (M11 == Fix64.Zero)
        {
            Console.WriteLine("Cannot invert a diagonal matrix with zero elements on the diagonal.");
            return this;
        }

        return new Fix3x3(
            M00 != Fix64.Zero ? Fix64.One / M00 : Fix64.Zero, Fix64.Zero, Fix64.Zero,
            Fix64.Zero, Fix64.One / M11, Fix64.Zero,
            Fix64.Zero, Fix64.Zero, M22 != Fix64.Zero ? Fix64.One / M22 : Fix64.Zero
        );
    }

    #endregion

    #region Static Matrix Generators and Transformations

    /// <summary>
    /// Creates a 3x3 matrix representing a rotation around the X-axis.
    /// </summary>
    /// <param name="angle">The angle of rotation in radians.</param>
    /// <returns>A 3x3 rotation matrix.</returns>
    public static Fix3x3 CreateRotationX(Fix64 angle)
    {
        Fix64 cos = MathFix.Cos(angle);
        Fix64 sin = MathFix.Sin(angle);

        return new Fix3x3(
            Fix64.One, Fix64.Zero, Fix64.Zero,
            Fix64.Zero, cos, -sin,
            Fix64.Zero, sin, cos
        );
    }

    /// <summary>
    /// Creates a 3x3 matrix representing a rotation around the Y-axis.
    /// </summary>
    /// <param name="angle">The angle of rotation in radians.</param>
    /// <returns>A 3x3 rotation matrix.</returns>
    public static Fix3x3 CreateRotationY(Fix64 angle)
    {
        Fix64 cos = MathFix.Cos(angle);
        Fix64 sin = MathFix.Sin(angle);

        return new Fix3x3(
            cos, Fix64.Zero, sin,
            Fix64.Zero, Fix64.One, Fix64.Zero,
            -sin, Fix64.Zero, cos
        );
    }

    /// <summary>
    /// Creates a 3x3 matrix representing a rotation around the Z-axis.
    /// </summary>
    /// <param name="angle">The angle of rotation in radians.</param>
    /// <returns>A 3x3 rotation matrix.</returns>
    public static Fix3x3 CreateRotationZ(Fix64 angle)
    {
        Fix64 cos = MathFix.Cos(angle);
        Fix64 sin = MathFix.Sin(angle);

        return new Fix3x3(
            cos, -sin, Fix64.Zero,
            sin, cos, Fix64.Zero,
            Fix64.Zero, Fix64.Zero, Fix64.One
        );
    }

    /// <summary>
    /// Creates a 3x3 shear matrix.
    /// </summary>
    /// <param name="shX">Shear factor along the X-axis.</param>
    /// <param name="shY">Shear factor along the Y-axis.</param>
    /// <param name="shZ">Shear factor along the Z-axis.</param>
    /// <returns>A 3x3 shear matrix.</returns>
    public static Fix3x3 CreateShear(Fix64 shX, Fix64 shY, Fix64 shZ)
    {
        return new Fix3x3(
            Fix64.One, shX, shY,
            shX, Fix64.One, shZ,
            shY, shZ, Fix64.One
        );
    }

    /// <summary>
    /// Creates a scaling matrix that applies a uniform or non-uniform scale transformation.
    /// </summary>
    /// <param name="scale">The scale factors along the X, Y, and Z axes.</param>
    /// <returns>A 3x3 scaling matrix.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix3x3 CreateScale(FixVector3 scale)
    {
        return new Fix3x3(
            scale.X, Fix64.Zero, Fix64.Zero,
            Fix64.Zero, scale.Y, Fix64.Zero,
            Fix64.Zero, Fix64.Zero, scale.Z
        );
    }

    /// <summary>
    /// Creates a uniform scaling matrix with the same scale factor on all axes.
    /// </summary>
    /// <param name="scaleFactor">The uniform scale factor.</param>
    /// <returns>A 3x3 scaling matrix with uniform scaling.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix3x3 CreateScale(Fix64 scaleFactor)
    {
        return CreateScale(new FixVector3(scaleFactor, scaleFactor, scaleFactor));
    }

    /// <summary>
    /// Normalizes the basis vectors of a 3x3 matrix to ensure they are orthogonal and unit length.
    /// </summary>
    /// <remarks>
    /// This method recalculates and normalizes the X, Y, and Z basis vectors of the matrix to avoid numerical drift 
    /// that can occur after multiple transformations. It also ensures that the Z-axis is recomputed to maintain 
    /// orthogonality by taking the cross-product of the normalized X and Y axes.
    /// 
    /// Use Cases:
    /// - Ensuring stability and correctness after repeated transformations involving rotation and scaling.
    /// - Useful in physics calculations where orthogonal matrices are required (e.g., inertia tensors or rotations).
    /// </remarks>
    public static Fix3x3 Normalize(Fix3x3 matrix)
    {
        var x = new FixVector3(matrix.M00, matrix.M01, matrix.M02).Normalize();
        var y = new FixVector3(matrix.M10, matrix.M11, matrix.M12).Normalize();
        var z = FixVector3.Cross(x, y).Normalize();

        matrix.M00 = x.X; matrix.M01 = x.Y; matrix.M02 = x.Z;
        matrix.M10 = y.X; matrix.M11 = y.Y; matrix.M12 = y.Z;
        matrix.M20 = z.X; matrix.M21 = z.Y; matrix.M22 = z.Z;

        return matrix;
    }

    /// <summary>
    /// Resets the scaling part of the matrix to identity (1,1,1).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix3x3 ResetScaleToIdentity(Fix3x3 matrix)
    {
        matrix.M00 = Fix64.One;  // Reset scale on X-axis
        matrix.M11 = Fix64.One;  // Reset scale on Y-axis
        matrix.M22 = Fix64.One;  // Reset scale on Z-axis
        return matrix;
    }

    /// <inheritdoc cref="SetLossyScale(Fix64, Fix64, Fix64)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix3x3 SetLossyScale(FixVector3 scale)
    {
        return SetLossyScale(scale.X, scale.Y, scale.Z);
    }

    /// <summary>
    /// Creates a scaling matrix (puts the 'scale' vector down the diagonal)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix3x3 SetLossyScale(Fix64 x, Fix64 y, Fix64 z)
    {
        return new Fix3x3(
            x, Fix64.Zero, Fix64.Zero,
            Fix64.Zero, y, Fix64.Zero,
            Fix64.Zero, Fix64.Zero, z
        );
    }

    /// <summary>
    /// Applies the provided local scale to the matrix by modifying the diagonal elements.
    /// </summary>
    /// <param name="matrix">The matrix to set the scale against.</param>
    /// <param name="localScale">A FixedVector3 representing the local scale to apply.</param>
    public static Fix3x3 SetScale(Fix3x3 matrix, FixVector3 localScale)
    {
        matrix.M00 = localScale.X; // Apply scale on X-axis
        matrix.M11 = localScale.Y; // Apply scale on Y-axis
        matrix.M22 = localScale.Z; // Apply scale on Z-axis

        return matrix;
    }

    /// <summary>
    /// Sets the global scale of an object using FixedMatrix3x3.
    /// Similar to SetGlobalScale for FixedMatrix4x4, but for a 3x3 matrix.
    /// </summary>
    /// <param name="matrix">The transformation matrix (3x3) representing the object's global state.</param>
    /// <param name="globalScale">The desired global scale represented as a FixedVector3.</param>
    /// <remarks>
    /// The method extracts the current global scale from the matrix and computes the new local scale 
    /// by dividing the desired global scale by the current global scale. 
    /// The new local scale is then applied to the matrix.
    /// </remarks>
    public static Fix3x3 SetGlobalScale(Fix3x3 matrix, FixVector3 globalScale)
    {
        // normalize the matrix to avoid drift in the rotation component
        matrix.Normalize();

        // Reset the local scaling portion of the matrix
        matrix.ResetScaleToIdentity();

        // Compute the new local scale by dividing the desired global scale by the current global scale
        FixVector3 newLocalScale = new(
            globalScale.X / Fix64.One,
            globalScale.Y / Fix64.One,
            globalScale.Z / Fix64.One
        );

        // Apply the new local scale to the matrix
        return matrix.SetScale(newLocalScale);
    }

    /// <summary>
    /// Extracts the scaling factors from the matrix by returning the diagonal elements.
    /// </summary>
    /// <returns>A FixedVector3 representing the scale along X, Y, and Z axes.</returns>
    public static FixVector3 ExtractScale(Fix3x3 matrix)
    {
        return new FixVector3(
            new FixVector3(matrix.M00, matrix.M01, matrix.M02).Magnitude,
            new FixVector3(matrix.M10, matrix.M11, matrix.M12).Magnitude,
            new FixVector3(matrix.M20, matrix.M21, matrix.M22).Magnitude
        );
    }

    /// <summary>
    /// Extracts the scaling factors from the matrix by returning the diagonal elements (lossy).
    /// </summary>
    /// <returns>A FixedVector3 representing the scale along X, Y, and Z axes (lossy).</returns>
    public static FixVector3 ExtractLossyScale(Fix3x3 matrix)
    {
        return new FixVector3(matrix.M00, matrix.M11, matrix.M22);
    }

    #endregion

    #region Static Matrix Operations

    /// <summary>
    /// Linearly interpolates between two matrices.
    /// </summary>
    public static Fix3x3 Lerp(Fix3x3 a, Fix3x3 b, Fix64 t)
    {
        // Perform a linear interpolation between two matrices
        return new Fix3x3(
            MathFix.LinearInterpolate(a.M00, b.M00, t), MathFix.LinearInterpolate(a.M01, b.M01, t), MathFix.LinearInterpolate(a.M02, b.M02, t),
            MathFix.LinearInterpolate(a.M10, b.M10, t), MathFix.LinearInterpolate(a.M11, b.M11, t), MathFix.LinearInterpolate(a.M12, b.M12, t),
            MathFix.LinearInterpolate(a.M20, b.M20, t), MathFix.LinearInterpolate(a.M21, b.M21, t), MathFix.LinearInterpolate(a.M22, b.M22, t)
        );
    }

    /// <summary>
    /// Transposes the matrix (swaps rows and columns).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix3x3 Transpose(Fix3x3 matrix)
    {
        return new Fix3x3(
            matrix.M00, matrix.M10, matrix.M20,
            matrix.M01, matrix.M11, matrix.M21,
            matrix.M02, matrix.M12, matrix.M22
        );
    }

    /// <summary>
    /// Attempts to invert the matrix. If the determinant is zero, returns false and sets result to null.
    /// </summary>
    public static bool Invert(Fix3x3 matrix, out Fix3x3? result)
    {
        // Calculate the determinant
        Fix64 det = matrix.GetDeterminant();

        if (det == Fix64.Zero)
        {
            result = null;
            return false;
        }

        // Calculate the inverse
        Fix64 invDet = Fix64.One / det;

        // Compute the inverse matrix
        result = new Fix3x3(
            invDet * (matrix.M11 * matrix.M22 - matrix.M21 * matrix.M12),
            invDet * (matrix.M02 * matrix.M21 - matrix.M01 * matrix.M22),
            invDet * (matrix.M01 * matrix.M12 - matrix.M02 * matrix.M11),

            invDet * (matrix.M12 * matrix.M20 - matrix.M10 * matrix.M22),
            invDet * (matrix.M00 * matrix.M22 - matrix.M02 * matrix.M20),
            invDet * (matrix.M02 * matrix.M10 - matrix.M00 * matrix.M12),

            invDet * (matrix.M10 * matrix.M21 - matrix.M11 * matrix.M20),
            invDet * (matrix.M01 * matrix.M20 - matrix.M00 * matrix.M21),
            invDet * (matrix.M00 * matrix.M11 - matrix.M01 * matrix.M10)
        );

        return true;
    }

    /// <summary>
    /// Transforms a direction vector from local space to world space using this transformation matrix.
    /// Ignores translation.
    /// </summary>
    /// <param name="matrix">The transformation matrix.</param>
    /// <param name="direction">The local-space direction vector.</param>
    /// <returns>The transformed direction in world space.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 TransformDirection(Fix3x3 matrix, FixVector3 direction)
    {
        return new FixVector3(
            matrix.M00 * direction.X + matrix.M01 * direction.Y + matrix.M02 * direction.Z,
            matrix.M10 * direction.X + matrix.M11 * direction.Y + matrix.M12 * direction.Z,
            matrix.M20 * direction.X + matrix.M21 * direction.Y + matrix.M22 * direction.Z
        );
    }

    /// <summary>
    /// Transforms a direction from world space into the local space of the matrix.
    /// Ignores translation.
    /// </summary>
    /// <param name="matrix">The transformation matrix.</param>
    /// <param name="direction">The world-space direction.</param>
    /// <returns>The transformed local-space direction.</returns>
    public static FixVector3 InverseTransformDirection(Fix3x3 matrix, FixVector3 direction)
    {
        if (!Invert(matrix, out Fix3x3? inverseMatrix) || !inverseMatrix.HasValue)
            throw new InvalidOperationException("Matrix is not invertible.");

        return new FixVector3(
            inverseMatrix.Value.M00 * direction.X + inverseMatrix.Value.M01 * direction.Y + inverseMatrix.Value.M02 * direction.Z,
            inverseMatrix.Value.M10 * direction.X + inverseMatrix.Value.M11 * direction.Y + inverseMatrix.Value.M12 * direction.Z,
            inverseMatrix.Value.M20 * direction.X + inverseMatrix.Value.M21 * direction.Y + inverseMatrix.Value.M22 * direction.Z
        );
    }

    #endregion

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix3x3 operator -(Fix3x3 a, Fix3x3 b)
    {
        // Subtract each element
        return new Fix3x3(
            a.M00 - b.M00, a.M01 - b.M01, a.M02 - b.M02,
            a.M10 - b.M10, a.M11 - b.M11, a.M12 - b.M12,
            a.M20 - b.M20, a.M21 - b.M21, a.M22 - b.M22
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix3x3 operator +(Fix3x3 a, Fix3x3 b)
    {
        // Add each element
        return new Fix3x3(
            a.M00 + b.M00, a.M01 + b.M01, a.M02 + b.M02,
            a.M10 + b.M10, a.M11 + b.M11, a.M12 + b.M12,
            a.M20 + b.M20, a.M21 + b.M21, a.M22 + b.M22
        );
    }
    /// <summary>
    /// Negates all elements of the matrix.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix3x3 operator -(Fix3x3 a)
    {
        // Negate each element
        return new Fix3x3(
            -a.M00, -a.M01, -a.M02,
            -a.M10, -a.M11, -a.M12,
            -a.M20, -a.M21, -a.M22
        );
    }
    public static Fix3x3 operator *(Fix3x3 a, Fix3x3 b)
    {
        // Perform matrix multiplication
        return new Fix3x3(
            a.M00 * b.M00 + a.M01 * b.M10 + a.M02 * b.M20,
            a.M00 * b.M01 + a.M01 * b.M11 + a.M02 * b.M21,
            a.M00 * b.M02 + a.M01 * b.M12 + a.M02 * b.M22,

            a.M10 * b.M00 + a.M11 * b.M10 + a.M12 * b.M20,
            a.M10 * b.M01 + a.M11 * b.M11 + a.M12 * b.M21,
            a.M10 * b.M02 + a.M11 * b.M12 + a.M12 * b.M22,

            a.M20 * b.M00 + a.M21 * b.M10 + a.M22 * b.M20,
            a.M20 * b.M01 + a.M21 * b.M11 + a.M22 * b.M21,
            a.M20 * b.M02 + a.M21 * b.M12 + a.M22 * b.M22
        );
    }

    public static Fix3x3 operator *(Fix3x3 a, Fix64 scalar)
    {
        // Perform matrix multiplication by scalar
        return new Fix3x3(
            a.M00 * scalar, a.M01 * scalar, a.M02 * scalar,
            a.M10 * scalar, a.M11 * scalar, a.M12 * scalar,
            a.M20 * scalar, a.M21 * scalar, a.M22 * scalar
        );
    }

    public static Fix3x3 operator *(Fix64 scalar, Fix3x3 a)
    {
        // Perform matrix multiplication by scalar
        return a * scalar;
    }

    public static Fix3x3 operator /(Fix3x3 a, int divisor)
    {
        // Perform matrix multiplication by scalar
        return new Fix3x3(
            a.M00 / divisor, a.M01 / divisor, a.M02 / divisor,
            a.M10 / divisor, a.M11 / divisor, a.M12 / divisor,
            a.M20 / divisor, a.M21 / divisor, a.M22 / divisor
        );
    }

    public static Fix3x3 operator /(int divisor, Fix3x3 a)
    {
        return a / divisor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Fix3x3 left, Fix3x3 right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Fix3x3 left, Fix3x3 right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region Equality and HashCode Overrides

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Fix3x3 other)
    {
        // Compare each element for equality
        return
            M00 == other.M00 && M01 == other.M01 && M02 == other.M02 &&
            M10 == other.M10 && M11 == other.M11 && M12 == other.M12 &&
            M20 == other.M20 && M21 == other.M21 && M22 == other.M22;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj)
    {
        return obj is Fix3x3 other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + M00.GetHashCode();
            hash = hash * 23 + M01.GetHashCode();
            hash = hash * 23 + M02.GetHashCode();
            hash = hash * 23 + M10.GetHashCode();
            hash = hash * 23 + M11.GetHashCode();
            hash = hash * 23 + M12.GetHashCode();
            hash = hash * 23 + M20.GetHashCode();
            hash = hash * 23 + M21.GetHashCode();
            hash = hash * 23 + M22.GetHashCode();
            return hash;
        }
    }

    #endregion

    #region Conversion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString()
    {
        return $"[{M00}, {M01}, {M02}; {M10}, {M11}, {M12}; {M20}, {M21}, {M22}]";
    }

    #endregion
}
