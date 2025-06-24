using Ocluse.LiquidSnow.Numerics.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

/// <summary>
/// Represents a 2D vector with fixed-point precision, offering a range of mathematical operations
/// and transformations such as rotation, scaling, reflection, and interpolation.
/// </summary>
/// <remarks>
/// The FixedVector2 struct is designed for applications that require precise numerical operations, 
/// such as games, simulations, or physics engines. It provides methods for common vector operations
/// like addition, subtraction, dot product, cross product, distance calculations, and rotation.
/// 
/// Use Cases:
/// - Modeling 2D positions, directions, and velocities in fixed-point math environments.
/// - Performing vector transformations, including rotations and reflections.
/// - Handling interpolation and distance calculations in physics or simulation systems.
/// - Useful for fixed-point math scenarios where floating-point precision is insufficient or not desired.
/// </remarks>
[Serializable]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public partial struct FixVector2(Fix64 xFixed, Fix64 yFixed) : IEquatable<FixVector2>, IComparable<FixVector2>, IEqualityComparer<FixVector2>
{
    #region Fields and Constants

    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public Fix64 X = xFixed;

    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public Fix64 Y = yFixed;

    /// <summary>
    /// (1, 0)
    /// </summary>
    public static readonly FixVector2 DefaultRotation = new(1, 0);

    /// <summary>
    /// (0, 1)
    /// </summary>
    public static readonly FixVector2 Forward = new(0, 1);

    /// <summary>
    /// (1, 0)
    /// </summary>
    public static readonly FixVector2 Right = new(1, 0);

    /// <summary>
    /// (0, -1)
    /// </summary>
    public static readonly FixVector2 Down = new(0, -1);

    /// <summary>
    /// (-1, 0)
    /// </summary>
    public static readonly FixVector2 Left = new(-1, 0);

    /// <summary>
    /// (1, 1)
    /// </summary>
    public static readonly FixVector2 One = new(1, 1);

    /// <summary>
    /// (-1, -1)
    /// </summary>
    public static readonly FixVector2 Negative = new(-1, -1);

    /// <summary>
    /// (0, 0)
    /// </summary>
    public static readonly FixVector2 Zero = new(0, 0);

    #endregion

    #region Constructors

    [MethodImpl(MethodImplOptions.AggressiveInlining)]

    public FixVector2(int xInt, int yInt) : this((Fix64)xInt, (Fix64)yInt) { }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2(float xFloat, float yFloat) : this((Fix64)xFloat, (Fix64)yFloat) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2(double xDoub, double yDoub) : this((Fix64)xDoub, (Fix64)yDoub) { }

    #endregion

    #region Properties and Methods (Instance)

    /// <summary>
    /// Rotates the vector to the right (90 degrees clockwise).
    /// </summary>
    [JsonIgnore]
    public readonly FixVector2 RotatedRight
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Y, -X);
    }

    /// <summary>
    /// Rotates the vector to the left (90 degrees counterclockwise).
    /// </summary>
    [JsonIgnore]
    public readonly FixVector2 RotatedLeft
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(-Y, X);
    }

    /// <summary>
    /// Gets the right-hand (counter-clockwise) normal vector.
    /// </summary>
    [JsonIgnore]
    public readonly FixVector2 RightHandNormal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(-Y, X);
    }

    /// <summary>
    /// Gets the left-hand (clockwise) normal vector.
    /// </summary>
    [JsonIgnore]
    public readonly FixVector2 LeftHandNormal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Y, -X);
    }

    /// <inheritdoc cref="GetNormalized(FixVector2)"/>
    [JsonIgnore]
    public readonly FixVector2 Normal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetNormalized(this);
    }

    /// <summary>
    /// Returns the actual length of this vector (RO).
    /// </summary>
    [JsonIgnore]
    public readonly Fix64 Magnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetMagnitude(this);
    }

    /// <summary>
    /// Returns the square magnitude of the vector (avoids calculating the square root).
    /// </summary>
    [JsonIgnore]
    public readonly Fix64 SqrMagnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => X * X + Y * Y;
    }

    /// <summary>
    /// Returns a long hash of the vector based on its x and y values.
    /// </summary>
    [JsonIgnore]
    public readonly long LongStateHash
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => X.RawValue * 31 + Y.RawValue * 7;
    }

    /// <summary>
    /// Returns a hash of the vector based on its state.
    /// </summary>
    [JsonIgnore]
    public readonly int StateHash
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)(LongStateHash % int.MaxValue);
    }

    [JsonIgnore]
    public Fix64 this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                _ => throw new IndexOutOfRangeException("Invalid FixedVector2 index!"),
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            switch (index)
            {
                case 0:
                    X = value;
                    break;
                case 1:
                    Y = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid FixedVector2 index!");
            }
        }
    }

    /// <summary>
    /// Set x, y and z components of an existing Vector3.
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(Fix64 newX, Fix64 newY)
    {
        X = newX;
        Y = newY;
    }

    /// <summary>
    /// Adds the specified values to the components of the vector in place and returns the modified vector.
    /// </summary>
    /// <param name="amount">The amount to add to the components.</param>
    /// <returns>The modified vector after addition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 AddInPlace(Fix64 amount)
    {
        X += amount;
        Y += amount;
        return this;
    }

    /// <summary>
    /// Adds the specified values to the components of the vector in place and returns the modified vector.
    /// </summary>
    /// <param name="xAmount">The amount to add to the x component.</param>
    /// <param name="yAmount">The amount to add to the y component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 AddInPlace(Fix64 xAmount, Fix64 yAmount)
    {
        X += xAmount;
        Y += yAmount;
        return this;
    }

    /// <inheritdoc cref="AddInPlace(Fix64, Fix64)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 AddInPlace(FixVector2 other)
    {
        AddInPlace(other.X, other.Y);
        return this;
    }

    /// <summary>
    /// Subtracts the specified value from all components of the vector in place and returns the modified vector.
    /// </summary>
    /// <param name="amount">The amount to subtract from each component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 SubtractInPlace(Fix64 amount)
    {
        X -= amount;
        Y -= amount;
        return this;
    }

    /// <summary>
    /// Subtracts the specified values from the components of the vector in place and returns the modified vector.
    /// </summary>
    /// <param name="xAmount">The amount to subtract from the x component.</param>
    /// <param name="yAmount">The amount to subtract from the y component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 SubtractInPlace(Fix64 xAmount, Fix64 yAmount)
    {
        X -= xAmount;
        Y -= yAmount;
        return this;
    }

    /// <summary>
    /// Subtracts the specified vector from the components of the vector in place and returns the modified vector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 SubtractInPlace(FixVector2 other)
    {
        SubtractInPlace(other.X, other.Y);
        return this;
    }

    /// <summary>
    /// Scales the components of the vector by the specified scalar factor in place and returns the modified vector.
    /// </summary>
    /// <param name="scaleFactor">The scalar factor to multiply each component by.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 ScaleInPlace(Fix64 scaleFactor)
    {
        X *= scaleFactor;
        Y *= scaleFactor;
        return this;
    }

    /// <summary>
    /// Scales each component of the vector by the corresponding component of the given vector in place and returns the modified vector.
    /// </summary>
    /// <param name="scale">The vector containing the scale factors for each component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 ScaleInPlace(FixVector2 scale)
    {
        X *= scale.X;
        Y *= scale.Y;
        return this;
    }

    /// <summary>
    /// Normalizes this vector in place, making its magnitude (length) equal to 1, and returns the modified vector.
    /// </summary>
    /// <remarks>
    /// If the vector is zero-length or already normalized, no operation is performed. 
    /// This method modifies the current vector in place and supports method chaining.
    /// </remarks>
    /// <returns>The normalized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 Normalize()
    {
        return this = GetNormalized(this);
    }

    /// <summary>
    /// Normalizes this vector in place and outputs its original magnitude.
    /// </summary>
    /// <param name="mag">The original magnitude of the vector before normalization.</param>
    /// <remarks>
    /// If the vector is zero-length or already normalized, no operation is performed, but the original magnitude will still be output.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 Normalize(out Fix64 mag)
    {
        mag = GetMagnitude(this);

        // If magnitude is zero, return a zero vector to avoid divide-by-zero errors
        if (mag == Fix64.Zero)
        {
            X = Fix64.Zero;
            Y = Fix64.Zero;
            return this;
        }

        // If already normalized, return as-is
        if (mag == Fix64.One)
            return this;

        X /= mag;
        Y /= mag;

        return this;
    }

    /// <summary>
    /// Linearly interpolates this vector toward the target vector by the specified amount.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 LerpInPlace(FixVector2 target, Fix64 amount)
    {
        LerpInPlace(target.X, target.Y, amount);
        return this;
    }

    /// <summary>
    /// Linearly interpolates this vector toward the target values by the specified amount.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector2 LerpInPlace(Fix64 targetx, Fix64 targety, Fix64 amount)
    {
        if (amount >= Fix64.One)
        {
            X = targetx;
            Y = targety;
        }
        else if (amount > Fix64.Zero)
        {
            X = targetx * amount + X * (Fix64.One - amount);
            Y = targety * amount + Y * (Fix64.One - amount);
        }
        return this;
    }

    /// <summary>
    /// Linearly interpolates between two vectors.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 Lerp(FixVector2 a, FixVector2 b, Fix64 amount)
    {
        amount = MathFix.Clamp01(amount);
        return new FixVector2(a.X + (b.X - a.X) * amount, a.Y + (b.Y - a.Y) * amount);
    }

    /// <summary>
    /// Returns a new vector that is the result of linear interpolation toward the target by the specified amount.
    /// </summary>
    public readonly FixVector2 Lerped(FixVector2 target, Fix64 amount)
    {
        FixVector2 vec = this;
        vec.LerpInPlace(target.X, target.Y, amount);
        return vec;
    }

    /// <summary>
    /// Rotates this vector by the specified cosine and sine values (counter-clockwise).
    /// </summary>
    public FixVector2 RotateInPlace(Fix64 cos, Fix64 sin)
    {
        Fix64 temp1 = X * cos - Y * sin;
        Y = X * sin + Y * cos;
        X = temp1;
        return this;
    }

    /// <summary>
    /// Returns a new vector that is the result of rotating this vector by the specified cosine and sine values.
    /// </summary>
    public readonly FixVector2 Rotated(Fix64 cos, Fix64 sin)
    {
        FixVector2 vec = this;
        vec.RotateInPlace(cos, sin);
        return vec;
    }

    /// <summary>
    /// Rotates this vector using another vector representing the cosine and sine of the rotation angle.
    /// </summary>
    /// <param name="rotation">The vector containing the cosine and sine values for rotation.</param>
    /// <returns>A new vector representing the result of the rotation.</returns>
    public readonly FixVector2 Rotated(FixVector2 rotation)
    {
        return Rotated(rotation.X, rotation.Y);
    }

    /// <summary>
    /// Rotates this vector in the inverse direction using cosine and sine values.
    /// </summary>
    /// <param name="cos">The cosine of the rotation angle.</param>
    /// <param name="sin">The sine of the rotation angle.</param>
    public void RotateInverse(Fix64 cos, Fix64 sin)
    {
        RotateInPlace(cos, -sin);
    }

    /// <summary>
    /// Rotates this vector 90 degrees to the right (clockwise).
    /// </summary>
    public FixVector2 RotateRightInPlace()
    {
        Fix64 temp1 = X;
        X = Y;
        Y = -temp1;
        return this;
    }

    /// <summary>
    /// Rotates this vector 90 degrees to the left (counterclockwise).
    /// </summary>
    public FixVector2 RotateLeftInPlace()
    {
        Fix64 temp1 = X;
        X = -Y;
        Y = temp1;
        return this;
    }

    /// <summary>
    /// Reflects this vector across the specified axis vector.
    /// </summary>
    public FixVector2 ReflectInPlace(FixVector2 axis)
    {
        return ReflectInPlace(axis.X, axis.Y);
    }

    /// <summary>
    /// Reflects this vector across the specified x and y axis.
    /// </summary>
    public FixVector2 ReflectInPlace(Fix64 axisX, Fix64 axisY)
    {
        Fix64 projection = Dot(axisX, axisY);
        return ReflectInPlace(axisX, axisY, projection);
    }

    /// <summary>
    /// Reflects this vector across the specified axis using the provided projection of this vector onto the axis.
    /// </summary>
    /// /// <param name="axisX">The x component of the axis to reflect across.</param>
    /// <param name="axisY">The y component of the axis to reflect across.</param>
    /// <param name="projection">The precomputed projection of this vector onto the reflection axis.</param>
    public FixVector2 ReflectInPlace(Fix64 axisX, Fix64 axisY, Fix64 projection)
    {
        Fix64 temp1 = axisX * projection;
        Fix64 temp2 = axisY * projection;
        X = temp1 + temp1 - X;
        Y = temp2 + temp2 - Y;
        return this;
    }

    /// <summary>
    /// Reflects this vector across the specified x and y axis.
    /// </summary>
    /// <returns>A new vector representing the result of the reflection.</returns>
    public readonly FixVector2 Reflected(Fix64 axisX, Fix64 axisY)
    {
        FixVector2 vec = this;
        vec.ReflectInPlace(axisX, axisY);
        return vec;
    }

    /// <summary>
    /// Reflects this vector across the specified axis vector.
    /// </summary>
    /// <returns>A new vector representing the result of the reflection.</returns>
    public readonly FixVector2 Reflected(FixVector2 axis)
    {
        return Reflected(axis.X, axis.Y);
    }

    /// <summary>
    /// Returns the dot product of this vector with another vector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 Dot(Fix64 otherX, Fix64 otherY)
    {
        return X * otherX + Y * otherY;
    }

    /// <summary>
    /// Returns the dot product of this vector with another vector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 Dot(FixVector2 other)
    {
        return Dot(other.X, other.Y);
    }

    /// <summary>
    /// Computes the cross product magnitude of this vector with another vector.
    /// </summary>
    /// <param name="otherX">The X component of the other vector.</param>
    /// <param name="otherY">The Y component of the other vector.</param>
    /// <returns>The cross product magnitude.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 CrossProduct(Fix64 otherX, Fix64 otherY)
    {
        return X * otherY - Y * otherX;
    }

    /// <inheritdoc cref="CrossProduct(Fix64, Fix64)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 CrossProduct(FixVector2 other)
    {
        return CrossProduct(other.X, other.Y);
    }

    /// <summary>
    /// Returns the distance between this vector and another vector specified by its components.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 Distance(Fix64 otherX, Fix64 otherY)
    {
        Fix64 temp1 = X - otherX;
        temp1 *= temp1;
        Fix64 temp2 = Y - otherY;
        temp2 *= temp2;
        return MathFix.Sqrt(temp1 + temp2);
    }

    /// <summary>
    /// Returns the distance between this vector and another vector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 Distance(FixVector2 other)
    {
        return Distance(other.X, other.Y);
    }

    /// <summary>
    /// Calculates the squared distance between two vectors, avoiding the need for a square root operation.
    /// </summary>
    /// <returns>The squared distance between the two vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 SqrDistance(Fix64 otherX, Fix64 otherY)
    {
        Fix64 temp1 = X - otherX;
        temp1 *= temp1;
        Fix64 temp2 = Y - otherY;
        temp2 *= temp2;
        return temp1 + temp2;
    }

    /// <summary>
    /// Calculates the squared distance between two vectors, avoiding the need for a square root operation.
    /// </summary>
    /// <returns>The squared distance between the two vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 SqrDistance(FixVector2 other)
    {
        return SqrDistance(other.X, other.Y);
    }

    #endregion

    #region FixedVector2 Operations

    /// <summary>
    /// Normalizes the given vector, returning a unit vector with the same direction.
    /// </summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>A normalized (unit) vector with the same direction.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 GetNormalized(FixVector2 value)
    {
        Fix64 mag = GetMagnitude(value);

        if (mag == Fix64.Zero)
            return new FixVector2(Fix64.Zero, Fix64.Zero);

        // If already normalized, return as-is
        if (mag == Fix64.One)
            return value;

        // Normalize it exactly
        return new FixVector2(
            value.X / mag,
            value.Y / mag
        );
    }

    /// <summary>
    /// Returns the magnitude (length) of the given vector.
    /// </summary>
    /// <param name="vector">The vector to compute the magnitude of.</param>
    /// <returns>The magnitude (length) of the vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 GetMagnitude(FixVector2 vector)
    {
        Fix64 mag = (vector.X * vector.X) + (vector.Y * vector.Y);

        // If rounding error pushed magnitude slightly above 1, clamp it
        if (mag > Fix64.One && mag <= Fix64.One + Fix64.Epsilon)
            return Fix64.One;

        return mag.Abs() > Fix64.Zero ? MathFix.Sqrt(mag) : Fix64.Zero;
    }

    /// <summary>
    /// Returns a new <see cref="FixVector2"/> where each component is the absolute value of the corresponding input component.
    /// </summary>
    /// <param name="value">The input vector.</param>
    /// <returns>A vector with absolute values for each component.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 Abs(FixVector2 value)
    {
        return new FixVector2(value.X.Abs(), value.Y.Abs());
    }

    /// <summary>
    /// Returns a new <see cref="FixVector2"/> where each component is the sign of the corresponding input component.
    /// </summary>
    /// <param name="value">The input vector.</param>
    /// <returns>A vector where each component is -1, 0, or 1 based on the sign of the input.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 Sign(FixVector2 value)
    {
        return new FixVector2(value.X.Sign(), value.Y.Sign());
    }

    /// <summary>
    /// Creates a vector from a given angle in radians.
    /// </summary>
    public static FixVector2 CreateRotation(Fix64 angle)
    {
        return new FixVector2(MathFix.Cos(angle), MathFix.Sin(angle));
    }

    /// <summary>
    /// Computes the distance between two vectors using the Euclidean distance formula.
    /// </summary>
    /// <param name="start">The starting vector.</param>
    /// <param name="end">The ending vector.</param>
    /// <returns>The Euclidean distance between the two vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Distance(FixVector2 start, FixVector2 end)
    {
        return start.Distance(end);
    }

    /// <summary>
    /// Calculates the squared distance between two vectors, avoiding the need for a square root operation.
    /// </summary>
    /// <returns>The squared distance between the two vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 SqrDistance(FixVector2 start, FixVector2 end)
    {
        return start.SqrDistance(end);
    }

    /// <summary>
    /// Calculates the forward direction vector in 2D based on a yaw (angle).
    /// </summary>
    /// <param name="angle">The angle in radians representing the rotation in 2D space.</param>
    /// <returns>A unit vector representing the forward direction.</returns>
    public static FixVector2 ForwardDirection(Fix64 angle)
    {
        Fix64 x = MathFix.Cos(angle); // Forward in the x-direction
        Fix64 y = MathFix.Sin(angle); // Forward in the y-direction
        return new FixVector2(x, y);
    }

    /// <summary>
    /// Dot Product of two vectors.
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Dot(FixVector2 lhs, FixVector2 rhs)
    {
        return lhs.Dot(rhs.X, rhs.Y);
    }

    /// <summary>
    /// Multiplies two vectors component-wise.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static FixVector2 Scale(FixVector2 a, FixVector2 b)
    {
        return new FixVector2(a.X * b.X, a.Y * b.Y);
    }

    /// <summary>
    /// Cross Product of two vectors.
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 CrossProduct(FixVector2 lhs, FixVector2 rhs)
    {
        return lhs.CrossProduct(rhs);
    }

    /// <summary>
    /// Rotates this vector by the specified angle (in radians).
    /// </summary>
    /// <param name="vec">The vector to rotate.</param>
    /// <param name="angleInRadians">The angle in radians.</param>
    /// <returns>The rotated vector.</returns>
    public static FixVector2 Rotate(FixVector2 vec, Fix64 angleInRadians)
    {
        Fix64 cos = MathFix.Cos(angleInRadians);
        Fix64 sin = MathFix.Sin(angleInRadians);
        return new FixVector2(
            vec.X * cos - vec.Y * sin,
            vec.X * sin + vec.Y * cos
        );
    }

    #endregion

    #region Conversion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString()
    {
        return $"({Math.Round((double)X, 2)}, {Math.Round((double)Y, 2)})";
    }

    /// <summary>
    /// Converts this <see cref="FixVector2"/> to a <see cref="FixVector3"/>, 
    /// mapping the Y component of this vector to the Z axis in the resulting vector.
    /// </summary>
    /// <param name="z">The value to assign to the Y axis of the resulting <see cref="FixVector3"/>.</param>
    /// <returns>
    /// A new <see cref="FixVector3"/> where (X, Y) from this <see cref="FixVector2"/> 
    /// become (X, Z) in the resulting vector, with the provided Z parameter assigned to Y.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly FixVector3 ToFixedVector3(Fix64 z)
    {
        return new FixVector3(X, z, Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Deconstruct(out float x, out float y)
    {
        x = this.X.ToPreciseFloat();
        y = this.Y.ToPreciseFloat();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Deconstruct(out int x, out int y)
    {
        x = this.X.RoundToInt();
        y = this.Y.RoundToInt();
    }

    /// <summary>
    /// Converts each component of the vector from radians to degrees.
    /// </summary>
    /// <param name="radians">The vector with components in radians.</param>
    /// <returns>A new vector with components converted to degrees.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 ToDegrees(FixVector2 radians)
    {
        return new FixVector2(
            MathFix.RadToDeg(radians.X),
            MathFix.RadToDeg(radians.Y)
        );
    }

    /// <summary>
    /// Converts each component of the vector from degrees to radians.
    /// </summary>
    /// <param name="degrees">The vector with components in degrees.</param>
    /// <returns>A new vector with components converted to radians.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 ToRadians(FixVector2 degrees)
    {
        return new FixVector2(
            MathFix.DegToRad(degrees.X),
            MathFix.DegToRad(degrees.Y)
        );
    }

    #endregion

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator +(FixVector2 v1, FixVector2 v2)
    {
        return new FixVector2(v1.X + v2.X, v1.Y + v2.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator +(FixVector2 v1, Fix64 mag)
    {
        return new FixVector2(v1.X + mag, v1.Y + mag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator +(Fix64 mag, FixVector2 v1)
    {
        return v1 + mag;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator +(FixVector2 v1, (int x, int y) v2)
    {
        return new FixVector2(v1.X + v2.x, v1.Y + v2.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator +((int x, int y) v2, FixVector2 v1)
    {
        return v1 + v2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator +(FixVector2 v1, (float x, float y) v2)
    {
        return new FixVector2(v1.X + v2.x, v1.Y + v2.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator +((float x, float y) v1, FixVector2 v2)
    {
        return v2 + v1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator -(FixVector2 v1, FixVector2 v2)
    {
        return new FixVector2(v1.X - v2.X, v1.Y - v2.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator -(FixVector2 v1, Fix64 mag)
    {
        return new FixVector2(v1.X - mag, v1.Y - mag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator -(Fix64 mag, FixVector2 v1)
    {
        return new FixVector2(mag - v1.X, mag - v1.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator -(FixVector2 v1, (int x, int y) v2)
    {
        return new FixVector2(v1.X - v2.x, v1.Y - v2.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator -((int x, int y) v1, FixVector2 v2)
    {
        return new FixVector2(v1.x - v2.X, v1.y - v2.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator -(FixVector2 v1, (float x, float y) v2)
    {
        return new FixVector2(v1.X - v2.x, v1.Y - v2.y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator -((float x, float y) v1, FixVector2 v2)
    {
        return new FixVector2(v1.x - v2.X, v1.y - v2.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator -(FixVector2 v1)
    {
        return new FixVector2(v1.X * -Fix64.One, v1.Y * -Fix64.One);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator *(FixVector2 v1, Fix64 mag)
    {
        return new FixVector2(v1.X * mag, v1.Y * mag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator *(FixVector2 v1, FixVector2 v2)
    {
        return new FixVector2(v1.X * v2.X, v1.Y * v2.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector2 operator /(FixVector2 v1, Fix64 div)
    {
        return new FixVector2(v1.X / div, v1.Y / div);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(FixVector2 left, FixVector2 right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(FixVector2 left, FixVector2 right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region Equality, HashCode, and Comparable Overrides

    /// <summary>
    /// Are all components of this vector equal to zero?
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool EqualsZero()
    {
        return this.Equals(Zero);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool NotZero()
    {
        return !EqualsZero();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj)
    {
        return obj is FixVector2 other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(FixVector2 other)
    {
        return other.X == X && other.Y == Y;
    }

    public readonly bool Equals(FixVector2 x, FixVector2 y)
    {
        return x.Equals(y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        return StateHash;
    }

    public readonly int GetHashCode(FixVector2 obj)
    {
        return obj.GetHashCode();
    }

    public readonly int CompareTo(FixVector2 other)
    {
        return SqrMagnitude.CompareTo(other.SqrMagnitude);
    }

    #endregion
}