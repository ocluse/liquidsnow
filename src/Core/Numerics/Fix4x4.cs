#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics;

/// <summary>
/// Represents a 4x4 matrix used for transformations in 3D space, including translation, rotation, scaling, and perspective projection.
/// </summary>
/// <remarks>
/// Initializes a new FixedMatrix4x4 with individual elements.
/// </remarks>
[Serializable]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct Fix4x4(
    Fix64 m00, Fix64 m01, Fix64 m02, Fix64 m03,
    Fix64 m10, Fix64 m11, Fix64 m12, Fix64 m13,
    Fix64 m20, Fix64 m21, Fix64 m22, Fix64 m23,
    Fix64 m30, Fix64 m31, Fix64 m32, Fix64 m33
    ) : IEquatable<Fix4x4>
{
    #region Fields and Constants

    [JsonInclude]
    public Fix64 M00 = m00;

    [JsonInclude]
    public Fix64 M01 = m01;

    [JsonInclude]
    public Fix64 M02 = m02;

    [JsonInclude]
    public Fix64 M03 = m03;

    [JsonInclude]
    public Fix64 M10 = m10;

    [JsonInclude]
    public Fix64 M11 = m11;

    [JsonInclude]
    public Fix64 M12 = m12;

    [JsonInclude]
    public Fix64 M13 = m13;

    [JsonInclude]
    public Fix64 M20 = m20;

    [JsonInclude]
    public Fix64 M21 = m21;

    [JsonInclude]
    public Fix64 M22 = m22;

    [JsonInclude]
    public Fix64 M23 = m23;

    [JsonInclude]
    public Fix64 M30 = m30;

    [JsonInclude]
    public Fix64 M31 = m31;

    [JsonInclude]
    public Fix64 M32 = m32;

    [JsonInclude]
    public Fix64 M33 = m33;

    /// <summary>
    /// Returns the identity matrix (diagonal elements set to 1).
    /// </summary>
    public static readonly Fix4x4 Identity = new(
        Fix64.One, Fix64.Zero, Fix64.Zero, Fix64.Zero,
        Fix64.Zero, Fix64.One, Fix64.Zero, Fix64.Zero,
        Fix64.Zero, Fix64.Zero, Fix64.One, Fix64.Zero,
        Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One);

    /// <summary>
    /// Returns a matrix with all elements set to zero.
    /// </summary>
    public static readonly Fix4x4 Zero = new(
        Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero,
        Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero,
        Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero,
        Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero);

    #endregion
    #region Constructors

    #endregion

    #region Properties and Methods (Instance)

    [JsonIgnore]
    public readonly bool IsAffine => (M33 == Fix64.One) && (M03 == Fix64.Zero && M13 == Fix64.Zero && M23 == Fix64.Zero);

    /// <inheritdoc cref="ExtractTranslation(Fix4x4)" />
    [JsonIgnore]
    public readonly FixVector3 Translation => ExtractTranslation(this);

    [JsonIgnore]
    public readonly FixVector3 Up => ExtractUp(this);

    /// <inheritdoc cref="ExtractScale(Fix4x4)" />
    [JsonIgnore]
    public readonly FixVector3 Scale => ExtractScale(this);

    /// <inheritdoc cref="ExtractRotation(Fix4x4)" />
    [JsonIgnore]
    public readonly FixQuaternion Rotation => ExtractRotation(this);

    [JsonIgnore]
    public Fix64 this[int index]
    {
        readonly get
        {
            return index switch
            {
                0 => M00,
                1 => M10,
                2 => M20,
                3 => M30,
                4 => M01,
                5 => M11,
                6 => M21,
                7 => M31,
                8 => M02,
                9 => M12,
                10 => M22,
                11 => M32,
                12 => M03,
                13 => M13,
                14 => M23,
                15 => M33,
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
                case 3:
                    M30 = value;
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
                case 7:
                    M31 = value;
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
                case 11:
                    M32 = value;
                    break;
                case 12:
                    M03 = value;
                    break;
                case 13:
                    M13 = value;
                    break;
                case 14:
                    M23 = value;
                    break;
                case 15:
                    M33 = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid matrix index!");
            }
        }
    }

    /// <summary>
    /// Calculates the determinant of a 4x4 matrix.
    /// </summary>
    public readonly Fix64 GetDeterminant()
    {
        if (IsAffine)
        {
            return M00 * (M11 * M22 - M12 * M21)
                 - M01 * (M10 * M22 - M12 * M20)
                 + M02 * (M10 * M21 - M11 * M20);
        }

        // Process as full 4x4 matrix
        Fix64 minor0 = M22 * M33 - M23 * M32;
        Fix64 minor1 = M21 * M33 - M23 * M31;
        Fix64 minor2 = M21 * M32 - M22 * M31;
        Fix64 cofactor0 = M20 * M33 - M23 * M30;
        Fix64 cofactor1 = M20 * M32 - M22 * M30;
        Fix64 cofactor2 = M20 * M31 - M21 * M30;
        return M00 * (M11 * minor0 - M12 * minor1 + M13 * minor2)
            - M01 * (M10 * minor0 - M12 * cofactor0 + M13 * cofactor1)
            + M02 * (M10 * minor1 - M11 * cofactor0 + M13 * cofactor2)
            - M03 * (M10 * minor2 - M11 * cofactor1 + M12 * cofactor2);
    }

    /// <inheritdoc cref="Fix4x4.ResetScaleToIdentity(Fix4x4)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Fix4x4 ResetScaleToIdentity()
    {
        return this = ResetScaleToIdentity(this);
    }

    /// <summary>
    /// Sets the translation, scale, and rotation components onto the matrix.
    /// </summary>
    /// <param name="translation">The translation vector.</param>
    /// <param name="scale">The scale vector.</param>
    /// <param name="rotation">The rotation quaternion.</param>
    public void SetTransform(FixVector3 translation, FixQuaternion rotation, FixVector3 scale)
    {
        this = CreateTransform(translation, rotation, scale);
    }

    #endregion

    #region Static Matrix Generators and Transformations

    /// <summary>
    /// Creates a translation matrix from the specified 3-dimensional vector.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>The translation matrix.</returns>
    public static Fix4x4 CreateTranslation(FixVector3 position)
    {
        Fix4x4 result = default;
        result.M00 = Fix64.One;
        result.M01 = Fix64.Zero;
        result.M02 = Fix64.Zero;
        result.M03 = Fix64.Zero;
        result.M10 = Fix64.Zero;
        result.M11 = Fix64.One;
        result.M12 = Fix64.Zero;
        result.M13 = Fix64.Zero;
        result.M20 = Fix64.Zero;
        result.M21 = Fix64.Zero;
        result.M22 = Fix64.One;
        result.M23 = Fix64.Zero;
        result.M30 = position.X;
        result.M31 = position.Y;
        result.M32 = position.Z;
        result.M33 = Fix64.One;
        return result;
    }

    /// <summary>
    /// Creates a rotation matrix from a quaternion.
    /// </summary>
    /// <param name="rotation">The quaternion representing the rotation.</param>
    /// <returns>A 4x4 matrix representing the rotation.</returns>
    public static Fix4x4 CreateRotation(FixQuaternion rotation)
    {
        Fix3x3 rotationMatrix = rotation.ToMatrix3x3();

        return new Fix4x4(
            rotationMatrix.M00, rotationMatrix.M01, rotationMatrix.M02, Fix64.Zero,
            rotationMatrix.M10, rotationMatrix.M11, rotationMatrix.M12, Fix64.Zero,
            rotationMatrix.M20, rotationMatrix.M21, rotationMatrix.M22, Fix64.Zero,
            Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One
        );
    }

    /// <summary>
    /// Creates a scale matrix from a 3-dimensional vector.
    /// </summary>
    /// <param name="scale">The vector representing the scale along each axis.</param>
    /// <returns>A 4x4 matrix representing the scale transformation.</returns>
    public static Fix4x4 CreateScale(FixVector3 scale)
    {
        return new Fix4x4(
            scale.X, Fix64.Zero, Fix64.Zero, Fix64.Zero,
            Fix64.Zero, scale.Y, Fix64.Zero, Fix64.Zero,
            Fix64.Zero, Fix64.Zero, scale.Z, Fix64.Zero,
            Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One
        );
    }

    /// <summary>
    /// Constructs a transformation matrix from translation, scale, and rotation.
    /// This method ensures that the rotation is properly normalized, applies the scale to the
    /// rotational basis, and sets the translation component separately.
    /// </summary>
    /// <remarks>
    /// - Uses a normalized rotation matrix to maintain numerical stability.
    /// - Applies non-uniform scaling to the rotation before setting translation.
    /// - Preferred when ensuring transformations remain mathematically correct.
    /// - If the rotation is already normalized and combined transformations are needed, consider using <see cref="ScaleRotateTranslate"/>.
    /// </remarks>
    /// <param name="translation">The translation vector.</param>
    /// <param name="scale">The scale vector.</param>
    /// <param name="rotation">The rotation quaternion.</param>
    /// <returns>A transformation matrix incorporating translation, rotation, and scale.</returns>
    public static Fix4x4 CreateTransform(FixVector3 translation, FixQuaternion rotation, FixVector3 scale)
    {
        // Create the rotation matrix and normalize it
        Fix4x4 rotationMatrix = CreateRotation(rotation);
        rotationMatrix = NormalizeRotationMatrix(rotationMatrix);

        // Apply scale directly to the rotation matrix
        rotationMatrix = ApplyScaleToRotation(rotationMatrix, scale);

        // Apply the translation to the combined matrix
        rotationMatrix = SetTranslation(rotationMatrix, translation);

        return rotationMatrix;
    }

    /// <summary>
    /// Constructs a transformation matrix from translation, rotation, and scale by multiplying
    /// separate matrices in the order: Scale * Rotation * Translation.
    /// </summary>
    /// <remarks>
    /// - This method directly multiplies the scale, rotation, and translation matrices.
    /// - Ensures that scale is applied first to preserve correct axis scaling.
    /// - Then rotation is applied so that rotation is not affected by non-uniform scaling.
    /// - Finally, translation moves the object to its correct world position.
    /// </remarks>
    public static Fix4x4 ScaleRotateTranslate(FixVector3 translation, FixQuaternion rotation, FixVector3 scale)
    {
        // Create translation matrix
        Fix4x4 translationMatrix = CreateTranslation(translation);

        // Create rotation matrix using the quaternion
        Fix4x4 rotationMatrix = CreateRotation(rotation);

        // Create scaling matrix
        Fix4x4 scalingMatrix = CreateScale(scale);

        // Combine all transformations
        return (scalingMatrix * rotationMatrix) * translationMatrix;
    }

    /// <summary>
    /// Constructs a transformation matrix from translation, rotation, and scale by multiplying
    /// matrices in the order: Translation * Rotation * Scale (T * R * S).
    /// </summary>
    /// <remarks>
    /// - Use this method when transformations need to be applied **relative to an object's local origin**.
    /// - Example use cases include **animation systems**, **hierarchical transformations**, and **UI transformations**.
    /// - If you need to apply world-space transformations, use <see cref="CreateTransform"/> instead.
    /// </remarks>
    public static Fix4x4 TranslateRotateScale(FixVector3 translation, FixQuaternion rotation, FixVector3 scale)
    {
        // Create translation matrix
        Fix4x4 translationMatrix = CreateTranslation(translation);

        // Create rotation matrix using the quaternion
        Fix4x4 rotationMatrix = CreateRotation(rotation);

        // Create scaling matrix
        Fix4x4 scalingMatrix = CreateScale(scale);

        // Combine all transformations
        return (translationMatrix * rotationMatrix) * scalingMatrix;
    }

    #endregion

    #region Decomposition, Extraction, and Setters

    /// <summary>
    /// Extracts the translation component from the 4x4 matrix.
    /// </summary>
    /// <param name="matrix">The matrix from which to extract the translation.</param>
    /// <returns>A FixedVector3 representing the translation component.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 ExtractTranslation(Fix4x4 matrix)
    {
        return new FixVector3(matrix.M30, matrix.M31, matrix.M32);
    }

    /// <summary>
    /// Extracts the up direction from the 4x4 matrix.
    /// </summary>
    /// <remarks>
    /// This is the surface normal if the matrix represents ground orientation.
    /// </remarks>
    /// <param name="matrix"></param>
    /// <returns>A <see cref="FixVector3"/> representing the up direction.</returns>
    public static FixVector3 ExtractUp(Fix4x4 matrix)
    {
        return new FixVector3(matrix.M10, matrix.M11, matrix.M12).Normalize();
    }

    /// <summary>
    /// Extracts the scaling factors from the matrix by calculating the magnitudes of the basis vectors (non-lossy).
    /// </summary>
    /// <returns>A FixedVector3 representing the precise scale along the X, Y, and Z axes.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 ExtractScale(Fix4x4 matrix)
    {
        return new FixVector3(
            new FixVector3(matrix.M00, matrix.M01, matrix.M02).Magnitude,  // X scale
            new FixVector3(matrix.M10, matrix.M11, matrix.M12).Magnitude,  // Y scale
            new FixVector3(matrix.M20, matrix.M21, matrix.M22).Magnitude   // Z scale
        );
    }

    /// <summary>
    /// Extracts the scaling factors from the matrix by returning the diagonal elements (lossy).
    /// </summary>
    /// <returns>A FixedVector3 representing the scale along X, Y, and Z axes (lossy).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 ExtractLossyScale(Fix4x4 matrix)
    {
        return new FixVector3(matrix.M00, matrix.M11, matrix.M22);
    }

    /// <summary>
    /// Extracts the rotation component from the 4x4 matrix by normalizing the rotation matrix.
    /// </summary>
    /// <param name="matrix">The matrix from which to extract the rotation.</param>
    /// <returns>A FixedQuaternion representing the rotation component.</returns>
    public static FixQuaternion ExtractRotation(Fix4x4 matrix)
    {
        FixVector3 scale = ExtractScale(matrix);

        // prevent divide by zero exception
        Fix64 scaleX = scale.X == Fix64.Zero ? Fix64.One : scale.X;
        Fix64 scaleY = scale.Y == Fix64.Zero ? Fix64.One : scale.Y;
        Fix64 scaleZ = scale.Z == Fix64.Zero ? Fix64.One : scale.Z;

        Fix4x4 normalizedMatrix = new(
            matrix.M00 / scaleX, matrix.M01 / scaleY, matrix.M02 / scaleZ, Fix64.Zero,
            matrix.M10 / scaleX, matrix.M11 / scaleY, matrix.M12 / scaleZ, Fix64.Zero,
            matrix.M20 / scaleX, matrix.M21 / scaleY, matrix.M22 / scaleZ, Fix64.Zero,
            Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One
        );

        return FixQuaternion.FromMatrix(normalizedMatrix);
    }

    /// <summary>
    /// Decomposes a 4x4 matrix into its translation, scale, and rotation components.
    /// </summary>
    /// <param name="matrix">The 4x4 matrix to decompose.</param>
    /// <param name="scale">The extracted scale component.</param>
    /// <param name="rotation">The extracted rotation component as a quaternion.</param>
    /// <param name="translation">The extracted translation component.</param>
    /// <returns>True if decomposition was successful, otherwise false.</returns>
    public static bool Decompose(
        Fix4x4 matrix,
        out FixVector3 scale,
        out FixQuaternion rotation,
        out FixVector3 translation)
    {
        // Extract scale by calculating the magnitudes of the basis vectors
        scale = ExtractScale(matrix);

        // prevent divide by zero exception
        scale = new FixVector3(
             scale.X == Fix64.Zero ? Fix64.One : scale.X,
             scale.Y == Fix64.Zero ? Fix64.One : scale.Y,
             scale.Z == Fix64.Zero ? Fix64.One : scale.Z);

        // normalize rotation and scaling
        Fix4x4 normalizedMatrix = ApplyScaleToRotation(matrix, FixVector3.One / scale);

        // Extract translation
        translation = new FixVector3(normalizedMatrix.M30, normalizedMatrix.M31, normalizedMatrix.M32);

        // Check the determinant to ensure correct handedness
        Fix64 determinant = normalizedMatrix.GetDeterminant();
        if (determinant < Fix64.Zero)
        {
            // Adjust for left-handed coordinate system by flipping one of the axes
            scale.X = -scale.X;
            normalizedMatrix.M00 = -normalizedMatrix.M00;
            normalizedMatrix.M01 = -normalizedMatrix.M01;
            normalizedMatrix.M02 = -normalizedMatrix.M02;
        }

        // Extract the rotation component from the orthogonalized matrix
        rotation = FixQuaternion.FromMatrix(normalizedMatrix);

        return true;
    }

    /// <summary>
    /// Sets the translation component of the 4x4 matrix.
    /// </summary>
    /// <param name="matrix">The matrix to modify.</param>
    /// <param name="translation">The new translation vector.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix4x4 SetTranslation(Fix4x4 matrix, FixVector3 translation)
    {
        matrix.M30 = translation.X;
        matrix.M31 = translation.Y;
        matrix.M32 = translation.Z;
        return matrix;
    }

    /// <summary>
    /// Sets the scale component of the 4x4 matrix by assigning the provided scale vector to the matrix's diagonal elements.
    /// </summary>
    /// <param name="matrix">The matrix to modify. Typically an identity or transformation matrix.</param>
    /// <param name="scale">The new scale vector to apply along the X, Y, and Z axes.</param>
    /// <remarks>
    /// Best used for applying scale to an identity matrix or resetting the scale on an existing matrix.
    /// For non-uniform scaling in combination with rotation, use <see cref="ApplyScaleToRotation"/>.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix4x4 SetScale(Fix4x4 matrix, FixVector3 scale)
    {
        matrix.M00 = scale.X;
        matrix.M11 = scale.Y;
        matrix.M22 = scale.Z;
        return matrix;
    }

    /// <summary>
    /// Applies non-uniform scaling to the 4x4 matrix by multiplying the scale vector with the rotation matrix's basis vectors.
    /// </summary>
    /// <param name="matrix">The matrix to modify. Should already contain a valid rotation component.</param>
    /// <param name="scale">The scale vector to apply along the X, Y, and Z axes.</param>
    /// <remarks>
    /// Use this method when scaling is required in combination with an existing rotation, ensuring proper axis alignment.
    /// </remarks>
    public static Fix4x4 ApplyScaleToRotation(Fix4x4 matrix, FixVector3 scale)
    {
        // Scale each row of the rotation matrix
        matrix.M00 *= scale.X;
        matrix.M01 *= scale.X;
        matrix.M02 *= scale.X;

        matrix.M10 *= scale.Y;
        matrix.M11 *= scale.Y;
        matrix.M12 *= scale.Y;

        matrix.M20 *= scale.Z;
        matrix.M21 *= scale.Z;
        matrix.M22 *= scale.Z;

        return matrix;
    }

    /// <summary>
    /// Resets the scaling part of the matrix to identity (1,1,1).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix4x4 ResetScaleToIdentity(Fix4x4 matrix)
    {
        matrix.M00 = Fix64.One;  // X scale
        matrix.M11 = Fix64.One;  // Y scale
        matrix.M22 = Fix64.One;  // Z scale

        return matrix;
    }

    /// <summary>
    /// Sets the global scale of an object using a 4x4 transformation matrix.
    /// </summary>
    /// <param name="matrix">The transformation matrix representing the object's global state.</param>
    /// <param name="globalScale">The desired global scale as a vector.</param>
    /// <remarks>
    /// The method extracts the current global scale from the matrix and computes the new local scale 
    /// by dividing the desired global scale by the current global scale. 
    /// The new local scale is then applied to the matrix.
    /// </remarks>
    public static Fix4x4 SetGlobalScale(Fix4x4 matrix, FixVector3 globalScale)
    {
        // normalize the matrix to avoid drift in the rotation component
        matrix = NormalizeRotationMatrix(matrix);

        // Reset the local scaling portion of the matrix
        matrix.ResetScaleToIdentity();

        // Compute the new local scale by dividing the desired global scale by the current scale (which was reset to (1, 1, 1))
        FixVector3 newLocalScale = new(
           globalScale.X / Fix64.One,
           globalScale.Y / Fix64.One,
           globalScale.Z / Fix64.One
        );

        // Apply the new local scale directly to the matrix
        return ApplyScaleToRotation(matrix, newLocalScale);
    }

    /// <summary>
    /// Replaces the rotation component of the 4x4 matrix using the provided quaternion, without affecting the translation component.
    /// </summary>
    /// <param name="matrix">The matrix to modify. The rotation will replace the upper-left 3x3 portion of the matrix.</param>
    /// <param name="rotation">The quaternion representing the new rotation to apply.</param>
    /// <remarks>
    /// This method preserves the matrix's translation component. For complete transformation updates, use <see cref="SetTransform"/>.
    /// </remarks>
    public static Fix4x4 SetRotation(Fix4x4 matrix, FixQuaternion rotation)
    {
        Fix3x3 rotationMatrix = rotation.ToMatrix3x3();

        FixVector3 scale = ExtractScale(matrix);

        // Apply rotation to the upper-left 3x3 matrix

        matrix.M00 = rotationMatrix.M00 * scale.X;
        matrix.M01 = rotationMatrix.M01 * scale.X;
        matrix.M02 = rotationMatrix.M02 * scale.X;

        matrix.M10 = rotationMatrix.M10 * scale.Y;
        matrix.M11 = rotationMatrix.M11 * scale.Y;
        matrix.M12 = rotationMatrix.M12 * scale.Y;

        matrix.M20 = rotationMatrix.M20 * scale.Z;
        matrix.M21 = rotationMatrix.M21 * scale.Z;
        matrix.M22 = rotationMatrix.M22 * scale.Z;

        return matrix;
    }

    /// <summary>
    /// Normalizes the rotation component of a 4x4 matrix by ensuring the basis vectors are orthogonal and unit length.
    /// </summary>
    /// <remarks>
    /// This method recalculates the X, Y, and Z basis vectors from the upper-left 3x3 portion of the matrix, ensuring they are orthogonal and normalized. 
    /// The remaining components of the matrix are reset to maintain a valid transformation structure.
    /// 
    /// Use Cases:
    /// - Ensuring the rotation component remains stable and accurate after multiple transformations.
    /// - Used in 3D transformations to prevent numerical drift from affecting the orientation over time.
    /// - Essential for cases where precise orientation is required, such as animations or physics simulations.
    /// </remarks>
    public static Fix4x4 NormalizeRotationMatrix(Fix4x4 matrix)
    {
        FixVector3 basisX = new FixVector3(matrix.M00, matrix.M01, matrix.M02).Normalize();
        FixVector3 basisY = new FixVector3(matrix.M10, matrix.M11, matrix.M12).Normalize();
        FixVector3 basisZ = new FixVector3(matrix.M20, matrix.M21, matrix.M22).Normalize();

        return new Fix4x4(
            basisX.X, basisX.Y, basisX.Z, Fix64.Zero,
            basisY.X, basisY.Y, basisY.Z, Fix64.Zero,
            basisZ.X, basisZ.Y, basisZ.Z, Fix64.Zero,
            Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One
        );
    }

    #endregion

    #region Static Matrix Operators

    /// <summary>
    /// Inverts the matrix if it is invertible (i.e., if the determinant is not zero).
    /// </summary>
    /// <remarks>
    /// To Invert a FixedMatrix4x4, we need to calculate the inverse for each element. 
    /// This involves computing the cofactor for each element, 
    /// which is the determinant of the submatrix when the row and column of that element are removed, 
    /// multiplied by a sign based on the element's position. 
    /// After computing all cofactors, the result is transposed to get the inverse matrix.
    /// </remarks>
    public static bool Invert(Fix4x4 matrix, out Fix4x4 result)
    {
        if (!matrix.IsAffine)
            return FullInvert(matrix, out result);

        Fix64 det = matrix.GetDeterminant();

        if (det == Fix64.Zero)
        {
            result = Identity;
            return false;
        }

        Fix64 invDet = Fix64.One / det;

        // Invert the 3×3 upper-left rotation/scale matrix
        result = new Fix4x4(
            (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21) * invDet,
            (matrix.M02 * matrix.M21 - matrix.M01 * matrix.M22) * invDet,
            (matrix.M01 * matrix.M12 - matrix.M02 * matrix.M11) * invDet, Fix64.Zero,

            (matrix.M12 * matrix.M20 - matrix.M10 * matrix.M22) * invDet,
            (matrix.M00 * matrix.M22 - matrix.M02 * matrix.M20) * invDet,
            (matrix.M02 * matrix.M10 - matrix.M00 * matrix.M12) * invDet, Fix64.Zero,

            (matrix.M10 * matrix.M21 - matrix.M11 * matrix.M20) * invDet,
            (matrix.M01 * matrix.M20 - matrix.M00 * matrix.M21) * invDet,
            (matrix.M00 * matrix.M11 - matrix.M01 * matrix.M10) * invDet, Fix64.Zero,

            Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One  // Ensure homogeneous coordinate stays valid
        );

        Fix3x3 rotationScaleInverse = new(
            result.M00, result.M01, result.M02,
            result.M10, result.M11, result.M12,
            result.M20, result.M21, result.M22
        );

        // Correct translation component
        FixVector3 transformedTranslation = new(matrix.M30, matrix.M31, matrix.M32);
        transformedTranslation = -Fix3x3.TransformDirection(rotationScaleInverse, transformedTranslation);

        result.M30 = transformedTranslation.X;
        result.M31 = transformedTranslation.Y;
        result.M32 = transformedTranslation.Z;
        result.M33 = Fix64.One;

        return true;
    }

    private static bool FullInvert(Fix4x4 matrix, out Fix4x4 result)
    {
        Fix64 det = matrix.GetDeterminant();

        if (det == Fix64.Zero)
        {
            result = Fix4x4.Identity;
            return false;
        }

        Fix64 invDet = Fix64.One / det;

        // Inversion using cofactors and determinants of 3x3 submatrices
        result = new Fix4x4
        {
            // First row
            M00 = invDet * ((matrix.M11 * matrix.M22 * matrix.M33 + matrix.M12 * matrix.M23 * matrix.M31 + matrix.M13 * matrix.M21 * matrix.M32)
                          - (matrix.M13 * matrix.M22 * matrix.M31 + matrix.M11 * matrix.M23 * matrix.M32 + matrix.M12 * matrix.M21 * matrix.M33)),
            M01 = invDet * ((matrix.M01 * matrix.M23 * matrix.M32 + matrix.M02 * matrix.M21 * matrix.M33 + matrix.M03 * matrix.M22 * matrix.M31)
                          - (matrix.M03 * matrix.M21 * matrix.M32 + matrix.M01 * matrix.M22 * matrix.M33 + matrix.M02 * matrix.M23 * matrix.M31)),
            M02 = invDet * ((matrix.M01 * matrix.M12 * matrix.M33 + matrix.M02 * matrix.M13 * matrix.M31 + matrix.M03 * matrix.M11 * matrix.M32)
                          - (matrix.M03 * matrix.M12 * matrix.M31 + matrix.M01 * matrix.M13 * matrix.M32 + matrix.M02 * matrix.M11 * matrix.M33)),
            M03 = invDet * ((matrix.M01 * matrix.M13 * matrix.M22 + matrix.M02 * matrix.M11 * matrix.M23 + matrix.M03 * matrix.M12 * matrix.M21)
                          - (matrix.M03 * matrix.M11 * matrix.M22 + matrix.M01 * matrix.M12 * matrix.M23 + matrix.M02 * matrix.M13 * matrix.M21)),

            // Second row
            M10 = invDet * ((matrix.M10 * matrix.M23 * matrix.M32 + matrix.M12 * matrix.M20 * matrix.M33 + matrix.M13 * matrix.M22 * matrix.M30)
                          - (matrix.M13 * matrix.M20 * matrix.M32 + matrix.M10 * matrix.M22 * matrix.M33 + matrix.M12 * matrix.M23 * matrix.M30)),
            M11 = invDet * ((matrix.M00 * matrix.M22 * matrix.M33 + matrix.M02 * matrix.M23 * matrix.M30 + matrix.M03 * matrix.M20 * matrix.M32)
                          - (matrix.M03 * matrix.M20 * matrix.M32 + matrix.M00 * matrix.M23 * matrix.M32 + matrix.M02 * matrix.M20 * matrix.M33)),
            M12 = invDet * ((matrix.M00 * matrix.M13 * matrix.M32 + matrix.M02 * matrix.M10 * matrix.M33 + matrix.M03 * matrix.M12 * matrix.M30)
                          - (matrix.M03 * matrix.M10 * matrix.M32 + matrix.M00 * matrix.M12 * matrix.M33 + matrix.M02 * matrix.M13 * matrix.M30)),
            M13 = invDet * ((matrix.M00 * matrix.M12 * matrix.M23 + matrix.M02 * matrix.M13 * matrix.M20 + matrix.M03 * matrix.M10 * matrix.M22)
                          - (matrix.M03 * matrix.M10 * matrix.M22 + matrix.M00 * matrix.M13 * matrix.M22 + matrix.M02 * matrix.M12 * matrix.M20)),

            // Third row
            M20 = invDet * ((matrix.M10 * matrix.M21 * matrix.M33 + matrix.M11 * matrix.M23 * matrix.M30 + matrix.M13 * matrix.M20 * matrix.M31)
                          - (matrix.M13 * matrix.M20 * matrix.M31 + matrix.M10 * matrix.M23 * matrix.M31 + matrix.M11 * matrix.M20 * matrix.M33)),
            M21 = invDet * ((matrix.M00 * matrix.M23 * matrix.M31 + matrix.M01 * matrix.M20 * matrix.M33 + matrix.M03 * matrix.M21 * matrix.M30)
                          - (matrix.M03 * matrix.M20 * matrix.M31 + matrix.M00 * matrix.M21 * matrix.M33 + matrix.M01 * matrix.M23 * matrix.M30)),
            M22 = invDet * ((matrix.M00 * matrix.M11 * matrix.M33 + matrix.M01 * matrix.M13 * matrix.M30 + matrix.M03 * matrix.M10 * matrix.M31)
                          - (matrix.M03 * matrix.M10 * matrix.M31 + matrix.M00 * matrix.M13 * matrix.M31 + matrix.M01 * matrix.M10 * matrix.M33)),
            M23 = invDet * ((matrix.M00 * matrix.M13 * matrix.M21 + matrix.M01 * matrix.M10 * matrix.M23 + matrix.M03 * matrix.M11 * matrix.M20)
                          - (matrix.M03 * matrix.M10 * matrix.M21 + matrix.M00 * matrix.M11 * matrix.M23 + matrix.M01 * matrix.M13 * matrix.M20)),

            // Fourth row
            M30 = invDet * ((matrix.M10 * matrix.M22 * matrix.M31 + matrix.M11 * matrix.M20 * matrix.M32 + matrix.M12 * matrix.M21 * matrix.M30)
                          - (matrix.M12 * matrix.M20 * matrix.M31 + matrix.M10 * matrix.M21 * matrix.M32 + matrix.M11 * matrix.M22 * matrix.M30)),
            M31 = invDet * ((matrix.M00 * matrix.M21 * matrix.M32 + matrix.M01 * matrix.M22 * matrix.M30 + matrix.M02 * matrix.M20 * matrix.M31)
                          - (matrix.M02 * matrix.M20 * matrix.M31 + matrix.M00 * matrix.M22 * matrix.M31 + matrix.M01 * matrix.M20 * matrix.M32)),
            M32 = invDet * ((matrix.M00 * matrix.M12 * matrix.M31 + matrix.M01 * matrix.M10 * matrix.M32 + matrix.M02 * matrix.M11 * matrix.M30)
                          - (matrix.M02 * matrix.M10 * matrix.M31 + matrix.M00 * matrix.M11 * matrix.M32 + matrix.M01 * matrix.M12 * matrix.M30)),
            M33 = invDet * ((matrix.M00 * matrix.M11 * matrix.M22 + matrix.M01 * matrix.M12 * matrix.M20 + matrix.M02 * matrix.M10 * matrix.M21)
                          - (matrix.M02 * matrix.M10 * matrix.M21 + matrix.M00 * matrix.M12 * matrix.M21 + matrix.M01 * matrix.M11 * matrix.M20)),
        };

        return true;
    }

    /// <summary>
    /// Transforms a point from local space to world space using this transformation matrix.
    /// </summary>
    /// <remarks>
    /// This is the same as doing `<see cref="Fix4x4"/> a * <see cref="FixVector3"/> b`
    /// </remarks>
    /// <param name="matrix">The transformation matrix.</param>
    /// <param name="point">The local-space point.</param>
    /// <returns>The transformed point in world space.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 TransformPoint(Fix4x4 matrix, FixVector3 point)
    {
        if (matrix.IsAffine)
        {
            return new FixVector3(
                matrix.M00 * point.X + matrix.M01 * point.Y + matrix.M02 * point.Z + matrix.M30,
                matrix.M10 * point.X + matrix.M11 * point.Y + matrix.M12 * point.Z + matrix.M31,
                matrix.M20 * point.X + matrix.M21 * point.Y + matrix.M22 * point.Z + matrix.M32
            );
        }

        return FullTransformPoint(matrix, point);
    }

    private static FixVector3 FullTransformPoint(Fix4x4 matrix, FixVector3 point)
    {
        // Full 4×4 transformation (needed for perspective projections)
        Fix64 w = matrix.M03 * point.X + matrix.M13 * point.Y + matrix.M23 * point.Z + matrix.M33;
        if (w == Fix64.Zero) w = Fix64.One;  // Prevent divide-by-zero

        return new FixVector3(
            (matrix.M00 * point.X + matrix.M01 * point.Y + matrix.M02 * point.Z + matrix.M30) / w,
            (matrix.M10 * point.X + matrix.M11 * point.Y + matrix.M12 * point.Z + matrix.M31) / w,
            (matrix.M20 * point.X + matrix.M21 * point.Y + matrix.M22 * point.Z + matrix.M32) / w
        );
    }

    /// <summary>
    /// Transforms a point from world space into the local space of the matrix.
    /// </summary>
    /// <param name="matrix">The transformation matrix.</param>
    /// <param name="point">The world-space point.</param>
    /// <returns>The local-space point relative to the transformation matrix.</returns>
    public static FixVector3 InverseTransformPoint(Fix4x4 matrix, FixVector3 point)
    {
        // Invert the transformation matrix
        if (!Invert(matrix, out Fix4x4 inverseMatrix))
            throw new InvalidOperationException("Matrix is not invertible.");

        if (inverseMatrix.IsAffine)
        {
            return new FixVector3(
                inverseMatrix.M00 * point.X + inverseMatrix.M01 * point.Y + inverseMatrix.M02 * point.Z + inverseMatrix.M30,
                inverseMatrix.M10 * point.X + inverseMatrix.M11 * point.Y + inverseMatrix.M12 * point.Z + inverseMatrix.M31,
                inverseMatrix.M20 * point.X + inverseMatrix.M21 * point.Y + inverseMatrix.M22 * point.Z + inverseMatrix.M32
            );
        }

        return FullInverseTransformPoint(inverseMatrix, point);
    }

    private static FixVector3 FullInverseTransformPoint(Fix4x4 matrix, FixVector3 point)
    {
        // Full 4×4 transformation (needed for perspective projections)
        Fix64 w = matrix.M03 * point.X + matrix.M13 * point.Y + matrix.M23 * point.Z + matrix.M33;
        if (w == Fix64.Zero) w = Fix64.One;  // Prevent divide-by-zero

        return new FixVector3(
            (matrix.M00 * point.X + matrix.M01 * point.Y + matrix.M02 * point.Z + matrix.M30) / w,
            (matrix.M10 * point.X + matrix.M11 * point.Y + matrix.M12 * point.Z + matrix.M31) / w,
            (matrix.M20 * point.X + matrix.M21 * point.Y + matrix.M22 * point.Z + matrix.M32) / w
        );
    }

    #endregion

    #region Operators

    /// <summary>
    /// Negates the specified matrix by multiplying all its values by -1.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix4x4 operator -(Fix4x4 value)
    {
        Fix4x4 result = default;
        result.M00 = -value.M00;
        result.M01 = -value.M01;
        result.M02 = -value.M02;
        result.M03 = -value.M03;
        result.M10 = -value.M10;
        result.M11 = -value.M11;
        result.M12 = -value.M12;
        result.M13 = -value.M13;
        result.M20 = -value.M20;
        result.M21 = -value.M21;
        result.M22 = -value.M22;
        result.M23 = -value.M23;
        result.M30 = -value.M30;
        result.M31 = -value.M31;
        result.M32 = -value.M32;
        result.M33 = -value.M33;
        return result;
    }

    /// <summary>
    /// Adds two matrices element-wise.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix4x4 operator +(Fix4x4 lhs, Fix4x4 rhs)
    {
        return new Fix4x4(
            lhs.M00 + rhs.M00, lhs.M01 + rhs.M01, lhs.M02 + rhs.M02, lhs.M03 + rhs.M03,
            lhs.M10 + rhs.M10, lhs.M11 + rhs.M11, lhs.M12 + rhs.M12, lhs.M13 + rhs.M13,
            lhs.M20 + rhs.M20, lhs.M21 + rhs.M21, lhs.M22 + rhs.M22, lhs.M23 + rhs.M23,
            lhs.M30 + rhs.M30, lhs.M31 + rhs.M31, lhs.M32 + rhs.M32, lhs.M33 + rhs.M33);
    }

    /// <summary>
    /// Subtracts two matrices element-wise.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix4x4 operator -(Fix4x4 lhs, Fix4x4 rhs)
    {
        return new Fix4x4(
            lhs.M00 - rhs.M00, lhs.M01 - rhs.M01, lhs.M02 - rhs.M02, lhs.M03 - rhs.M03,
            lhs.M10 - rhs.M10, lhs.M11 - rhs.M11, lhs.M12 - rhs.M12, lhs.M13 - rhs.M13,
            lhs.M20 - rhs.M20, lhs.M21 - rhs.M21, lhs.M22 - rhs.M22, lhs.M23 - rhs.M23,
            lhs.M30 - rhs.M30, lhs.M31 - rhs.M31, lhs.M32 - rhs.M32, lhs.M33 - rhs.M33);
    }

    /// <summary>
    /// Multiplies two 4x4 matrices using standard matrix multiplication.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix4x4 operator *(Fix4x4 lhs, Fix4x4 rhs)
    {
        if (lhs.IsAffine && rhs.IsAffine)
        {
            // Optimized affine multiplication (skips full 4×4 multiplication)
            return new Fix4x4(
                lhs.M00 * rhs.M00 + lhs.M01 * rhs.M10 + lhs.M02 * rhs.M20,
                lhs.M00 * rhs.M01 + lhs.M01 * rhs.M11 + lhs.M02 * rhs.M21,
                lhs.M00 * rhs.M02 + lhs.M01 * rhs.M12 + lhs.M02 * rhs.M22,
                Fix64.Zero,

                lhs.M10 * rhs.M00 + lhs.M11 * rhs.M10 + lhs.M12 * rhs.M20,
                lhs.M10 * rhs.M01 + lhs.M11 * rhs.M11 + lhs.M12 * rhs.M21,
                lhs.M10 * rhs.M02 + lhs.M11 * rhs.M12 + lhs.M12 * rhs.M22,
                Fix64.Zero,

                lhs.M20 * rhs.M00 + lhs.M21 * rhs.M10 + lhs.M22 * rhs.M20,
                lhs.M20 * rhs.M01 + lhs.M21 * rhs.M11 + lhs.M22 * rhs.M21,
                lhs.M20 * rhs.M02 + lhs.M21 * rhs.M12 + lhs.M22 * rhs.M22,
                Fix64.Zero,

                lhs.M30 * rhs.M00 + lhs.M31 * rhs.M10 + lhs.M32 * rhs.M20 + rhs.M30,
                lhs.M30 * rhs.M01 + lhs.M31 * rhs.M11 + lhs.M32 * rhs.M21 + rhs.M31,
                lhs.M30 * rhs.M02 + lhs.M31 * rhs.M12 + lhs.M32 * rhs.M22 + rhs.M32,
                Fix64.One
            );
        }

        // Full 4×4 multiplication (fallback for perspective matrices)
        return new Fix4x4(
            // Upper-left 3×3 matrix multiplication (rotation & scale)
            lhs.M00 * rhs.M00 + lhs.M01 * rhs.M10 + lhs.M02 * rhs.M20 + lhs.M03 * rhs.M30,
            lhs.M00 * rhs.M01 + lhs.M01 * rhs.M11 + lhs.M02 * rhs.M21 + lhs.M03 * rhs.M31,
            lhs.M00 * rhs.M02 + lhs.M01 * rhs.M12 + lhs.M02 * rhs.M22 + lhs.M03 * rhs.M32,
            lhs.M00 * rhs.M03 + lhs.M01 * rhs.M13 + lhs.M02 * rhs.M23 + lhs.M03 * rhs.M33,

            lhs.M10 * rhs.M00 + lhs.M11 * rhs.M10 + lhs.M12 * rhs.M20 + lhs.M13 * rhs.M30,
            lhs.M10 * rhs.M01 + lhs.M11 * rhs.M11 + lhs.M12 * rhs.M21 + lhs.M13 * rhs.M31,
            lhs.M10 * rhs.M02 + lhs.M11 * rhs.M12 + lhs.M12 * rhs.M22 + lhs.M13 * rhs.M32,
            lhs.M10 * rhs.M03 + lhs.M11 * rhs.M13 + lhs.M12 * rhs.M23 + lhs.M13 * rhs.M33,

            lhs.M20 * rhs.M00 + lhs.M21 * rhs.M10 + lhs.M22 * rhs.M20 + lhs.M23 * rhs.M30,
            lhs.M20 * rhs.M01 + lhs.M21 * rhs.M11 + lhs.M22 * rhs.M21 + lhs.M23 * rhs.M31,
            lhs.M20 * rhs.M02 + lhs.M21 * rhs.M12 + lhs.M22 * rhs.M22 + lhs.M23 * rhs.M32,
            lhs.M20 * rhs.M03 + lhs.M21 * rhs.M13 + lhs.M22 * rhs.M23 + lhs.M23 * rhs.M33,

            // Compute new translation
            lhs.M30 * rhs.M00 + lhs.M31 * rhs.M10 + lhs.M32 * rhs.M20 + lhs.M33 * rhs.M30,
            lhs.M30 * rhs.M01 + lhs.M31 * rhs.M11 + lhs.M32 * rhs.M21 + lhs.M33 * rhs.M31,
            lhs.M30 * rhs.M02 + lhs.M31 * rhs.M12 + lhs.M32 * rhs.M22 + lhs.M33 * rhs.M32,
            lhs.M30 * rhs.M03 + lhs.M31 * rhs.M13 + lhs.M32 * rhs.M23 + lhs.M33 * rhs.M33
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Fix4x4 left, Fix4x4 right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Fix4x4 left, Fix4x4 right)
    {
        return !(left == right);
    }

    #endregion

    #region Equality and HashCode Overrides

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj)
    {
        return obj is Fix4x4 x && Equals(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(Fix4x4 other)
    {
        return M00 == other.M00 && M01 == other.M01 && M02 == other.M02 && M03 == other.M03 &&
               M10 == other.M10 && M11 == other.M11 && M12 == other.M12 && M13 == other.M13 &&
               M20 == other.M20 && M21 == other.M21 && M22 == other.M22 && M23 == other.M23 &&
               M30 == other.M30 && M31 == other.M31 && M32 == other.M32 && M33 == other.M33;
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
            hash = hash * 23 + M23.GetHashCode();
            hash = hash * 23 + M30.GetHashCode();
            hash = hash * 23 + M31.GetHashCode();
            hash = hash * 23 + M32.GetHashCode();
            hash = hash * 23 + M33.GetHashCode();
            return hash;
        }
    }

    #endregion
}