#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Ocluse.LiquidSnow.Numerics.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics;

/// <summary>
/// Represents a 3D vector with fixed-point precision, supporting a wide range of vector operations such as rotation, scaling, interpolation, and projection.
/// </summary>
/// <remarks>
/// The FixedVector3 struct is designed for high-precision applications in 3D space, including games, simulations, and physics engines. 
/// It offers essential operations like addition, subtraction, dot product, cross product, distance calculation, and normalization.
/// 
/// Use Cases:
/// - Modeling 3D positions, directions, and velocities with fixed-point precision.
/// - Performing vector transformations, including rotations using quaternions.
/// - Calculating distances, angles, projections, and interpolation between vectors.
/// - Essential for fixed-point math scenarios where floating-point precision isn't suitable.
/// </remarks>
[Serializable]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public partial struct FixVector3(Fix64 xLong, Fix64 yLong, Fix64 zLong) : IEquatable<FixVector3>, IComparable<FixVector3>, IEqualityComparer<FixVector3>
{
    #region Fields and Constants

    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public Fix64 X = xLong;

    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public Fix64 Y = yLong;

    /// <summary>
    /// The Z component of the vector.
    /// </summary>
    public Fix64 Z = zLong;

    /// <summary>
    /// The upward direction vector (0, 1, 0).
    /// </summary>
    public static readonly FixVector3 Up = new(0, 1, 0);

    /// <summary>
    /// (1, 0, 0)
    /// </summary>
    public static readonly FixVector3 Right = new(1, 0, 0);

    /// <summary>
    /// (0, -1, 0)
    /// </summary>
    public static readonly FixVector3 Down = new(0, -1, 0);

    /// <summary>
    /// (-1, 0, 0)
    /// </summary>
    public static readonly FixVector3 Left = new(-1, 0, 0);

    /// <summary>
    /// The forward direction vector (0, 0, 1).
    /// </summary>
    public static readonly FixVector3 Forward = new(0, 0, 1);

    /// <summary>
    /// (0, 0, -1)
    /// </summary>
    public static readonly FixVector3 Backward = new(0, 0, -1);

    /// <summary>
    /// (1, 1, 1)
    /// </summary>
    public static readonly FixVector3 One = new(1, 1, 1);

    /// <summary>
    /// (-1, -1, -1)
    /// </summary>
    public static readonly FixVector3 Negative = new(-1, -1, -1);

    /// <summary>
    /// (0, 0, 0)
    /// </summary>
    public static readonly FixVector3 Zero = new(0, 0, 0);

    #endregion

    #region Constructors

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3(int xInt, int yInt, int zInt) : this((Fix64)xInt, (Fix64)yInt, (Fix64)zInt) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3(double xDoub, double yDoub, double zDoub) : this((Fix64)xDoub, (Fix64)yDoub, (Fix64)zDoub) { }

    #endregion

    #region Properties and Methods (Instance)

    /// <summary>
    ///  Provides a rotated version of the current vector, where rotation is a 90 degrees rotation around the Y axis in the counter-clockwise direction.
    /// </summary>
    /// <remarks>
    /// These operations rotate the vector 90 degrees around the Y-axis.
    /// Note that the positive direction of rotation is defined by the right-hand rule:
    /// If your right hand's thumb points in the positive Y direction, then your fingers curl in the positive direction of rotation.
    /// </remarks>
    [JsonIgnore]
    public readonly FixVector3 RightHandNormal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Z, Y, -X);
    }

    /// <summary>
    /// Provides a rotated version of the current vector, where rotation is a 90 degrees rotation around the Y axis in the clockwise direction.
    /// </summary>
    [JsonIgnore]
    public readonly FixVector3 LeftHandNormal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(-Z, Y, X);
    }

    /// <inheritdoc cref="GetNormalized(FixVector3)"/>
    [JsonIgnore]
    public readonly FixVector3 Normal
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
    /// Calculates the forward direction vector based on the yaw (x) and pitch (y) angles.
    /// </summary>
    /// <remarks>
    /// This is commonly used to determine the direction an object is facing in 3D space,
    /// where 'x' represents the yaw (horizontal rotation) and 'y' represents the pitch (vertical rotation).
    /// </remarks>
    [JsonIgnore]
    public readonly FixVector3 Direction
    {
        get
        {
            Fix64 temp1 = MathFix.Cos(X) * MathFix.Sin(Y);
            Fix64 temp2 = MathFix.Sin(-X);
            Fix64 temp3 = MathFix.Cos(X) * MathFix.Cos(Y);
            return new FixVector3(temp1, temp2, temp3);
        }
    }

    /// <summary>
    /// Are all components of this vector equal to zero?
    /// </summary>
    /// <returns></returns>
    [JsonIgnore]
    public readonly bool IsZero
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.Equals(Zero);
    }

    /// <summary>
    /// This vector's square magnitude.
    /// If you're doing distance checks, use SqrMagnitude and square the distance you're checking against
    /// If you need to know the actual distance, use MyMagnitude
    /// </summary>
    /// <returns>The magnitude.</returns>
    [JsonIgnore]
    public readonly Fix64 SqrMagnitude
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (X * X) + (Y * Y) + (Z * Z);
    }

    /// <summary>
    /// Returns a long hash of the vector based on its x, y, and z values.
    /// </summary>
    [JsonIgnore]
    public readonly long LongStateHash
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (X.RawValue * 31) + (Y.RawValue * 7) + (Z.RawValue * 11);
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
                2 => Z,
                _ => throw new IndexOutOfRangeException("Invalid FixedVector3 index!"),
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
                case 2:
                    Z = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid FixedVector3 index!");
            }
        }
    }

    /// <summary>
    /// Set x, y and z components of an existing Vector3.
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <param name="newZ"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3 Set(Fix64 newX, Fix64 newY, Fix64 newZ)
    {
        X = newX;
        Y = newY;
        Z = newZ;
        return this;
    }

    /// <summary>
    /// Adds the specified values to the components of the vector in place and returns the modified vector.
    /// </summary>
    /// <param name="amount">The amount to add to the components.</param>
    /// <returns>The modified vector after addition.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3 AddInPlace(Fix64 amount)
    {
        X += amount;
        Y += amount;
        Z += amount;
        return this;
    }

    /// <summary>
    /// Adds the specified values to the components of the vector in place and returns the modified vector.
    /// </summary>
    /// <param name="xAmount">The amount to add to the x component.</param>
    /// <param name="yAmount">The amount to add to the y component.</param>
    /// <param name="zAmount">The amount to add to the z component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3 AddInPlace(Fix64 xAmount, Fix64 yAmount, Fix64 zAmount)
    {
        X += xAmount;
        Y += yAmount;
        Z += zAmount;
        return this;
    }

    /// <summary>
    /// Adds the specified vector components to the corresponding components of the in place vector and returns the modified vector.
    /// </summary>
    /// <param name="other">The other vector to add the components.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3 AddInPlace(FixVector3 other)
    {
        AddInPlace(other.X, other.Y, other.Z);
        return this;
    }

    /// <summary>
    /// Subtracts the specified value from all components of the vector in place and returns the modified vector.
    /// </summary>
    /// <param name="amount">The amount to subtract from each component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3 SubtractInPlace(Fix64 amount)
    {
        X -= amount;
        Y -= amount;
        Z -= amount;
        return this;
    }

    /// <summary>
    /// Subtracts the specified values from the components of the vector in place and returns the modified vector.
    /// </summary>
    /// <param name="xAmount">The amount to subtract from the x component.</param>
    /// <param name="yAmount">The amount to subtract from the y component.</param>
    /// <param name="zAmount">The amount to subtract from the z component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3 SubtractInPlace(Fix64 xAmount, Fix64 yAmount, Fix64 zAmount)
    {
        X -= xAmount;
        Y -= yAmount;
        Z -= zAmount;
        return this;
    }

    /// <summary>
    /// Subtracts the specified vector from the components of the vector in place and returns the modified vector.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3 SubtractInPlace(FixVector3 other)
    {
        SubtractInPlace(other.X, other.Y, other.Z);
        return this;
    }

    /// <summary>
    /// Scales the components of the vector by the specified scalar factor in place and returns the modified vector.
    /// </summary>
    /// <param name="scaleFactor">The scalar factor to multiply each component by.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3 ScaleInPlace(Fix64 scaleFactor)
    {
        X *= scaleFactor;
        Y *= scaleFactor;
        Z *= scaleFactor;
        return this;
    }

    /// <summary>
    /// Scales each component of the vector by the corresponding component of the given vector in place and returns the modified vector.
    /// </summary>
    /// <param name="scale">The vector containing the scale factors for each component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3 ScaleInPlace(FixVector3 scale)
    {
        X *= scale.X;
        Y *= scale.Y;
        Z *= scale.Z;
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
    public FixVector3 Normalize()
    {
        return this = GetNormalized(this);
    }

    /// <summary>
    /// Normalizes this vector in place and outputs its original magnitude.
    /// </summary>
    /// <remarks>
    /// If the vector is zero-length or already normalized, no operation is performed, but the original magnitude will still be output.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixVector3 Normalize(out Fix64 mag)
    {
        mag = GetMagnitude(this);

        // If magnitude is zero, return a zero vector to avoid divide-by-zero errors
        if (mag == Fix64.Zero)
        {
            X = Fix64.Zero;
            Y = Fix64.Zero;
            Z = Fix64.Zero;
            return this;
        }

        // If already normalized, return as-is
        if (mag == Fix64.One)
            return this;

        X /= mag;
        Y /= mag;
        Z /= mag;

        return this;
    }

    /// <summary>
    /// Checks if this vector has been normalized by checking if the magnitude is close to 1.
    /// </summary>
    public readonly bool IsNormalized()
    {
        return MathFix.Abs(Magnitude - Fix64.One) <= Fix64.Epsilon;
    }

    /// <summary>
    /// Computes the distance between this vector and another vector.
    /// </summary>
    /// <param name="otherX">The x component of the other vector.</param>
    /// <param name="otherY">The y component of the other vector.</param>
    /// <param name="otherZ">The z component of the other vector.</param>
    /// <returns>The distance between the two vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 Distance(Fix64 otherX, Fix64 otherY, Fix64 otherZ)
    {
        Fix64 temp1 = X - otherX;
        temp1 *= temp1;
        Fix64 temp2 = Y - otherY;
        temp2 *= temp2;
        Fix64 temp3 = Z - otherZ;
        temp3 *= temp3;
        return MathFix.Sqrt(temp1 + temp2 + temp3);
    }

    /// <summary>
    /// Calculates the squared distance between two vectors, avoiding the need for a square root operation.
    /// </summary>
    /// <returns>The squared distance between the two vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 SqrDistance(Fix64 otherX, Fix64 otherY, Fix64 otherZ)
    {
        Fix64 temp1 = X - otherX;
        temp1 *= temp1;
        Fix64 temp2 = Y - otherY;
        temp2 *= temp2;
        Fix64 temp3 = Z - otherZ;
        temp3 *= temp3;
        return temp1 + temp2 + temp3;
    }

    /// <summary>
    /// Computes the dot product of this vector with another vector specified by its components.
    /// </summary>
    /// <param name="otherX">The x component of the other vector.</param>
    /// <param name="otherY">The y component of the other vector.</param>
    /// <param name="otherZ">The z component of the other vector.</param>
    /// <returns>The dot product of the two vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 Dot(Fix64 otherX, Fix64 otherY, Fix64 otherZ)
    {
        return X * otherX + Y * otherY + Z * otherZ;
    }

    /// <summary>
    /// Computes the cross product magnitude of this vector with another vector.
    /// </summary>
    /// <param name="otherX">The X component of the other vector.</param>
    /// <param name="otherY">The Y component of the other vector.</param>
    /// <param name="otherZ">The Z component of the other vector.</param>
    /// <returns>The cross product magnitude.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Fix64 CrossProduct(Fix64 otherX, Fix64 otherY, Fix64 otherZ)
    {
        return (Y * otherZ - Z * otherY) + (Z * otherX - X * otherZ) + (X * otherY - Y * otherX);
    }

    /// <summary>
    /// Returns the cross vector of this vector with another vector.
    /// </summary>
    /// <returns>A new vector representing the cross product.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly FixVector3 Cross(Fix64 otherX, Fix64 otherY, Fix64 otherZ)
    {
        return new FixVector3(
            Y * otherZ - Z * otherY,
            Z * otherX - X * otherZ,
            X * otherY - Y * otherX);
    }

    #endregion

    #region FixedVector3 Operations

    /// <summary>
    /// Linearly interpolates between two points.
    /// </summary>
    /// <param name="a">Start value, returned when t = 0.</param>
    /// <param name="b">End value, returned when t = 1.</param>
    /// <param name="mag">Value used to interpolate between a and b.</param>
    /// <returns> Interpolated value, equals to a + (b - a) * t.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 Lerp(FixVector3 a, FixVector3 b, Fix64 mag)
    {
        mag = MathFix.Clamp01(mag);
        return new FixVector3(a.X + (b.X - a.X) * mag, a.Y + (b.Y - a.Y) * mag, a.Z + (b.Z - a.Z) * mag);
    }

    /// <summary>
    /// Linearly interpolates between two vectors without clamping the interpolation factor between 0 and 1.
    /// </summary>
    /// <param name="a">The start vector.</param>
    /// <param name="b">The end vector.</param>
    /// <param name="t">The interpolation factor. Values outside the range [0, 1] will cause the interpolation to go beyond the start or end points.</param>
    /// <returns>The interpolated vector.</returns>
    /// <remarks>
    /// Unlike traditional Lerp, this function allows interpolation factors greater than 1 or less than 0, 
    /// which means the resulting vector can extend beyond the endpoints.
    /// </remarks>
    public static FixVector3 UnclampedLerp(FixVector3 a, FixVector3 b, Fix64 t)
    {
        return (b - a) * t + a;
    }

    /// <summary>
    /// Moves from a to b at some speed dependent of a delta time with out passing b.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="speed"></param>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static FixVector3 SpeedLerp(FixVector3 a, FixVector3 b, Fix64 speed, Fix64 dt)
    {
        FixVector3 v = b - a;
        Fix64 dv = speed * dt;
        if (dv > v.Magnitude)
            return b;
        else
            return a + v.Normal * dv;
    }

    /// <summary>
    /// Spherically interpolates between two vectors, moving along the shortest arc on a unit sphere.
    /// </summary>
    /// <param name="start">The starting vector.</param>
    /// <param name="end">The ending vector.</param>
    /// <param name="percent">A value between 0 and 1 that represents the interpolation amount. 0 returns the start vector, and 1 returns the end vector.</param>
    /// <returns>The interpolated vector between the two input vectors.</returns>
    /// <remarks>
    /// Slerp is used to interpolate between two unit vectors on a sphere, providing smooth rotation.
    /// It can be more computationally expensive than linear interpolation (Lerp) but results in smoother, arc-like motion.
    /// </remarks>
    public static FixVector3 Slerp(FixVector3 start, FixVector3 end, Fix64 percent)
    {
        // Dot product - the cosine of the angle between 2 vectors.
        Fix64 dot = Dot(start, end);
        // Clamp it to be in the range of Acos()
        // This may be unnecessary, but floating point
        // precision can be a fickle mistress.
        MathFix.Clamp(dot, -Fix64.One, Fix64.One);
        // Acos(dot) returns the angle between start and end,
        // And multiplying that by percent returns the angle between
        // start and the final result.
        Fix64 theta = MathFix.Acos(dot) * percent;
        FixVector3 RelativeVec = end - start * dot;
        RelativeVec.Normalize();
        // Orthonormal basis
        // The final result.
        return (start * MathFix.Cos(theta)) + (RelativeVec * MathFix.Sin(theta));
    }

    /// <summary>
    /// Normalizes the given vector, returning a unit vector with the same direction.
    /// </summary>
    /// <param name="value">The vector to normalize.</param>
    /// <returns>A normalized (unit) vector with the same direction.</returns>
    public static FixVector3 GetNormalized(FixVector3 value)
    {
        Fix64 mag = GetMagnitude(value);

        // If magnitude is zero, return a zero vector to avoid divide-by-zero errors
        if (mag == Fix64.Zero)
            return new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);

        // If already normalized, return as-is
        if (mag == Fix64.One)
            return value;

        // Normalize it exactly           
        return new FixVector3(
            value.X / mag,
            value.Y / mag,
            value.Z / mag
        );
    }

    /// <summary>
    /// Returns the magnitude (length) of this vector.
    /// </summary>
    /// <param name="vector">The vector whose magnitude is being calculated.</param>
    /// <returns>The magnitude of the vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 GetMagnitude(FixVector3 vector)
    {
        Fix64 mag = (vector.X * vector.X) + (vector.Y * vector.Y) + (vector.Z * vector.Z);

        // If rounding error pushed magnitude slightly above 1, clamp it
        if (mag > Fix64.One && mag <= Fix64.One + Fix64.Epsilon)
            return Fix64.One;

        return mag != Fix64.Zero ? MathFix.Sqrt(mag) : Fix64.Zero;
    }

    /// <summary>
    /// Returns a new <see cref="FixVector3"/> where each component is the absolute value of the corresponding input component.
    /// </summary>
    /// <param name="value">The input vector.</param>
    /// <returns>A vector with absolute values for each component.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 Abs(FixVector3 value)
    {
        return new FixVector3(value.X.Abs(), value.Y.Abs(), value.Z.Abs());
    }

    /// <summary>
    /// Returns a new <see cref="FixVector3"/> where each component is the sign of the corresponding input component.
    /// </summary>
    /// <param name="value">The input vector.</param>
    /// <returns>A vector where each component is -1, 0, or 1 based on the sign of the input.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 Sign(FixVector3 value)
    {
        return new FixVector3(value.X.Sign(), value.Y.Sign(), value.Z.Sign());
    }

    /// <summary>
    /// Clamps each component of the given <see cref="FixVector3"/> within the specified min and max bounds.
    /// </summary>
    /// <param name="value">The vector to clamp.</param>
    /// <param name="min">The minimum bounds.</param>
    /// <param name="max">The maximum bounds.</param>
    /// <returns>A vector with each component clamped between min and max.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 Clamp(FixVector3 value, FixVector3 min, FixVector3 max)
    {
        return new FixVector3(
            MathFix.Clamp(value.X, min.X, max.X),
            MathFix.Clamp(value.Y, min.Y, max.Y),
            MathFix.Clamp(value.Z, min.Z, max.Z)
        );
    }

    /// <summary>
    /// Clamps the given FixedVector3 within the specified magnitude.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maxMagnitude"></param>
    /// <returns></returns>
    public static FixVector3 ClampMagnitude(FixVector3 value, Fix64 maxMagnitude)
    {
        if (value.SqrMagnitude > maxMagnitude * maxMagnitude)
            return value.Normal * maxMagnitude; // Scale vector to max magnitude

        return value;
    }

    /// <summary>
    /// Determines if two vectors are exactly parallel by checking if their cross product is zero.
    /// </summary>
    /// <param name="v1">The first vector.</param>
    /// <param name="v2">The second vector.</param>
    /// <returns>True if the vectors are exactly parallel, false otherwise.</returns>
    public static bool AreParallel(FixVector3 v1, FixVector3 v2)
    {
        return Cross(v1, v2).SqrMagnitude == Fix64.Zero;
    }

    /// <summary>
    /// Determines if two vectors are approximately parallel based on a cosine similarity threshold.
    /// </summary>
    /// <param name="v1">The first normalized vector.</param>
    /// <param name="v2">The second normalized vector.</param>
    /// <param name="cosThreshold">The cosine similarity threshold for near-parallel vectors.</param>
    /// <returns>True if the vectors are nearly parallel, false otherwise.</returns>
    public static bool AreAlmostParallel(FixVector3 v1, FixVector3 v2, Fix64 cosThreshold)
    {
        // Assuming v1 and v2 are already normalized
        Fix64 dot = Dot(v1, v2);

        // Compare dot product directly to the cosine threshold
        return dot >= cosThreshold;
    }

    /// <summary>
    /// Computes the midpoint between two vectors.
    /// </summary>
    /// <param name="v1">The first vector.</param>
    /// <param name="v2">The second vector.</param>
    /// <returns>The midpoint vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 Midpoint(FixVector3 v1, FixVector3 v2)
    {
        return new FixVector3((v1.X + v2.X) * Fix64.Half, (v1.Y + v2.Y) * Fix64.Half, (v1.Z + v2.Z) * Fix64.Half);
    }

    /// <inheritdoc cref="Distance(Fix64, Fix64, Fix64)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Distance(FixVector3 start, FixVector3 end)
    {
        return start.Distance(end.X, end.Y, end.Z);
    }

    /// <inheritdoc cref="SqrDistance(Fix64, Fix64, Fix64)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 SqrDistance(FixVector3 start, FixVector3 end)
    {
        return start.SqrDistance(end.X, end.Y, end.Z);
    }

    /// <summary>
    /// Calculates the closest points on two line segments.
    /// </summary>
    /// <param name="line1Start">The starting point of the first line segment.</param>
    /// <param name="line1End">The ending point of the first line segment.</param>
    /// <param name="line2Start">The starting point of the second line segment.</param>
    /// <param name="line2End">The ending point of the second line segment.</param>
    /// <returns>
    /// A tuple containing two points representing the closest points on each line segment. The first item is the closest point on the first line,
    /// and the second item is the closest point on the second line.
    /// </returns>
    /// <remarks>
    /// This method considers the line segments, not the infinite lines they represent, ensuring that the returned points always lie within the provided segments.
    /// </remarks>
    public static (FixVector3, FixVector3) ClosestPointsOnTwoLines(FixVector3 line1Start, FixVector3 line1End, FixVector3 line2Start, FixVector3 line2End)
    {
        FixVector3 u = line1End - line1Start;
        FixVector3 v = line2End - line2Start;
        FixVector3 w = line1Start - line2Start;

        Fix64 a = Dot(u, u);
        Fix64 b = Dot(u, v);
        Fix64 c = Dot(v, v);
        Fix64 d = Dot(u, w);
        Fix64 e = Dot(v, w);
        Fix64 D = a * c - b * b;

        Fix64 sc, tc;

        // compute the line parameters of the two closest points
        if (D < Fix64.Epsilon)
        {
            // the lines are almost parallel
            sc = Fix64.Zero;
            tc = (b > c ? d / b : e / c); // use the largest denominator
        }
        else
        {
            sc = (b * e - c * d) / D;
            tc = (a * e - b * d) / D;
        }

        // recompute sc if it is outside [0,1]
        if (sc < Fix64.Zero)
        {
            sc = Fix64.Zero;
            tc = (e < Fix64.Zero ? Fix64.Zero : (e > c ? Fix64.One : e / c));
        }
        else if (sc > Fix64.One)
        {
            sc = Fix64.One;
            tc = (e + b < Fix64.Zero ? Fix64.Zero : (e + b > c ? Fix64.One : (e + b) / c));
        }

        // recompute tc if it is outside [0,1]
        if (tc < Fix64.Zero)
        {
            tc = Fix64.Zero;
            sc = (-d < Fix64.Zero ? Fix64.Zero : (-d > a ? Fix64.One : -d / a));
        }
        else if (tc > Fix64.One)
        {
            tc = Fix64.One;
            sc = ((-d + b) < Fix64.Zero ? Fix64.Zero : ((-d + b) > a ? Fix64.One : (-d + b) / a));
        }

        // get the difference of the two closest points
        FixVector3 pointOnLine1 = line1Start + sc * u;
        FixVector3 pointOnLine2 = line2Start + tc * v;

        return (pointOnLine1, pointOnLine2);
    }

    /// <summary>
    /// Finds the closest point on a line segment between points A and B to a given point P.
    /// </summary>
    /// <param name="a">The start of the line segment.</param>
    /// <param name="b">The end of the line segment.</param>
    /// <param name="p">The point to project onto the segment.</param>
    /// <returns>The closest point on the line segment to P.</returns>
    public static FixVector3 ClosestPointOnLineSegment(FixVector3 a, FixVector3 b, FixVector3 p)
    {
        FixVector3 ab = b - a;
        Fix64 t = Dot(p - a, ab) / Dot(ab, ab);
        t = MathFix.Max(Fix64.Zero, MathFix.Min(Fix64.One, t));
        return a + ab * t;
    }

    /// <summary>
    /// Dot Product of two vectors.
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Dot(FixVector3 lhs, FixVector3 rhs)
    {
        return lhs.Dot(rhs.X, rhs.Y, rhs.Z);
    }

    /// <summary>
    /// Multiplies two vectors component-wise.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static FixVector3 Scale(FixVector3 a, FixVector3 b)
    {
        return new FixVector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    }

    /// <summary>
    /// Cross Product of two vectors.
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 Cross(FixVector3 lhs, FixVector3 rhs)
    {
        return lhs.Cross(rhs.X, rhs.Y, rhs.Z);
    }

    /// <inheritdoc cref="CrossProduct(Fix64, Fix64, Fix64)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 CrossProduct(FixVector3 lhs, FixVector3 rhs)
    {
        return lhs.CrossProduct(rhs.X, rhs.Y, rhs.Z);
    }

    /// <summary>
    /// Projects a vector onto another vector.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="onNormal"></param>
    /// <returns></returns>
    public static FixVector3 Project(FixVector3 vector, FixVector3 onNormal)
    {
        Fix64 sqrMag = Dot(onNormal, onNormal);
        if (sqrMag < Fix64.Epsilon)
            return Zero;
        else
        {
            Fix64 dot = Dot(vector, onNormal);
            return new FixVector3(onNormal.X * dot / sqrMag,
                onNormal.Y * dot / sqrMag,
                onNormal.Z * dot / sqrMag);
        }
    }

    /// <summary>
    /// Projects a vector onto a plane defined by a normal orthogonal to the plane.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="planeNormal"></param>
    /// <returns></returns>
    public static FixVector3 ProjectOnPlane(FixVector3 vector, FixVector3 planeNormal)
    {
        Fix64 sqrMag = Dot(planeNormal, planeNormal);
        if (sqrMag < Fix64.Epsilon)
            return vector;
        else
        {
            Fix64 dot = Dot(vector, planeNormal);
            return new FixVector3(vector.X - planeNormal.X * dot / sqrMag,
                vector.Y - planeNormal.Y * dot / sqrMag,
                vector.Z - planeNormal.Z * dot / sqrMag);
        }
    }

    /// <summary>
    /// Computes the angle in degrees between two vectors.
    /// </summary>
    /// <param name="from">The starting vector.</param>
    /// <param name="to">The target vector.</param>
    /// <returns>The angle in degrees between the two vectors.</returns>
    /// <remarks>
    /// This method calculates the angle by using the dot product between the vectors and normalizing the result.
    /// The angle is always the smaller angle between the two vectors on a plane.
    /// </remarks>
    public static Fix64 Angle(FixVector3 from, FixVector3 to)
    {
        Fix64 denominator = MathFix.Sqrt(from.SqrMagnitude * to.SqrMagnitude);

        if (denominator.Abs() < Fix64.Epsilon)
            return Fix64.Zero;

        Fix64 dot = MathFix.Clamp(Dot(from, to) / denominator, -Fix64.One, Fix64.One);

        return MathFix.RadToDeg(MathFix.Acos(dot));
    }

    /// <summary>
    ///  Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The maximized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 Max(FixVector3 value1, FixVector3 value2)
    {
        return new FixVector3((value1.X > value2.X) ? value1.X : value2.X, (value1.Y > value2.Y) ? value1.Y : value2.Y, (value1.Z > value2.Z) ? value1.Z : value2.Z);
    }

    /// <summary>
    /// Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The minimized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 Min(FixVector3 value1, FixVector3 value2)
    {
        return new FixVector3((value1.X < value2.X) ? value1.X : value2.X, (value1.Y < value2.Y) ? value1.Y : value2.Y, (value1.Z < value2.Z) ? value1.Z : value2.Z);
    }

    /// <summary>
    /// Rotates the vector around a given position using a specified quaternion rotation.
    /// </summary>
    /// <param name="source">The vector to rotate.</param>
    /// <param name="position">The position around which the vector is rotated.</param>
    /// <param name="rotation">The quaternion representing the rotation.</param>
    /// <returns>The rotated vector.</returns>
    public static FixVector3 Rotate(FixVector3 source, FixVector3 position, FixQuaternion rotation)
    {
        source -= position; // Translate the vector by the position
        var normalizedRotation = rotation.Normal;
        return (normalizedRotation * source) + position;
    }

    /// <summary>
    /// Applies the inverse of a specified quaternion rotation to the vector around a given position.
    /// </summary>
    /// <param name="source">The vector to rotate.</param>
    /// <param name="position">The position around which the vector is rotated.</param>
    /// <param name="rotation">The quaternion representing the inverse rotation.</param>
    /// <returns>The rotated vector.</returns>
    public static FixVector3 InverseRotate(FixVector3 source, FixVector3 position, FixQuaternion rotation)
    {
        source -= position; // Translate the vector by the position
        var normalizedRotation = rotation.Normal;
        // Undo the rotation
        source = normalizedRotation.Inverse() * source;
        // Add the original position back
        return source + position;
    }

    #endregion

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator +(FixVector3 v1, FixVector3 v2)
    {
        return new FixVector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator +(FixVector3 v1, Fix64 mag)
    {
        return new FixVector3(v1.X + mag, v1.Y + mag, v1.Z + mag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator +(Fix64 mag, FixVector3 v1)
    {
        return v1 + mag;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator +(FixVector3 v1, (int x, int y, int z) v2)
    {
        return new FixVector3(v1.X + v2.x, v1.Y + v2.y, v1.Z + v2.z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator +((int x, int y, int z) v2, FixVector3 v1)
    {
        return v1 + v2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator +(FixVector3 v1, (float x, float y, float z) v2)
    {
        return new FixVector3(v1.X + v2.x, v1.Y + v2.y, v1.Z + v2.z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator +((float x, float y, float z) v1, FixVector3 v2)
    {
        return v2 + v1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator -(FixVector3 v1, FixVector3 v2)
    {
        return new FixVector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator -(FixVector3 v1, Fix64 mag)
    {
        return new FixVector3(v1.X - mag, v1.Y - mag, v1.Z - mag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator -(Fix64 mag, FixVector3 v1)
    {
        return new FixVector3(mag - v1.X, mag - v1.Y, mag - v1.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator -(FixVector3 v1, (int x, int y, int z) v2)
    {
        return new FixVector3(v1.X - v2.x, v1.Y - v2.y, v1.Z - v2.z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator -((int x, int y, int z) v1, FixVector3 v2)
    {
        return new FixVector3(v1.x - v2.X, v1.y - v2.Y, v1.z - v2.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator -(FixVector3 v1, (float x, float y, float z) v2)
    {
        return new FixVector3(v1.X - v2.x, v1.Y - v2.y, v1.Z - v2.z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator -((float x, float y, float z) v1, FixVector3 v2)
    {
        return new FixVector3(v1.x - v2.X, v1.y - v2.Y, v1.z - v2.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator -(FixVector3 v1)
    {
        return new FixVector3(v1.X * -Fix64.One, v1.Y * -Fix64.One, v1.Z * -Fix64.One);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator *(FixVector3 v1, Fix64 mag)
    {
        return new FixVector3(v1.X * mag, v1.Y * mag, v1.Z * mag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator *(Fix64 mag, FixVector3 v1)
    {
        return new FixVector3(v1.X * mag, v1.Y * mag, v1.Z * mag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator *(FixVector3 v1, int mag)
    {
        return new FixVector3(v1.X * mag, v1.Y * mag, v1.Z * mag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator *(int mag, FixVector3 v1)
    {
        return new FixVector3(v1.X * mag, v1.Y * mag, v1.Z * mag);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator *(Fix3x3 matrix, FixVector3 vector)
    {
        return new FixVector3(
            matrix.M00 * vector.X + matrix.M01 * vector.Y + matrix.M02 * vector.Z,
            matrix.M10 * vector.X + matrix.M11 * vector.Y + matrix.M12 * vector.Z,
            matrix.M20 * vector.X + matrix.M21 * vector.Y + matrix.M22 * vector.Z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator *(FixVector3 vector, Fix3x3 matrix)
    {
        return matrix * vector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator *(Fix4x4 matrix, FixVector3 point)
    {
        if (matrix.IsAffine)
        {
            return new FixVector3(
                matrix.M00 * point.X + matrix.M01 * point.Y + matrix.M02 * point.Z + matrix.M03 + matrix.M30,
                matrix.M10 * point.X + matrix.M11 * point.Y + matrix.M12 * point.Z + matrix.M13 + matrix.M31,
                matrix.M20 * point.X + matrix.M21 * point.Y + matrix.M22 * point.Z + matrix.M23 + matrix.M32
            );
        }

        // Full 4×4 transformation
        Fix64 w = matrix.M03 * point.X + matrix.M13 * point.Y + matrix.M23 * point.Z + matrix.M33;
        if (w == Fix64.Zero) w = Fix64.One;  // Prevent divide-by-zero

        return new FixVector3(
            (matrix.M00 * point.X + matrix.M01 * point.Y + matrix.M02 * point.Z + matrix.M03 + matrix.M30) / w,
            (matrix.M10 * point.X + matrix.M11 * point.Y + matrix.M12 * point.Z + matrix.M13 + matrix.M31) / w,
            (matrix.M20 * point.X + matrix.M21 * point.Y + matrix.M22 * point.Z + matrix.M23 + matrix.M32) / w
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator *(FixVector3 vector, Fix4x4 matrix)
    {
        return matrix * vector;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator *(FixVector3 v1, FixVector3 v2)
    {
        return new FixVector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator /(FixVector3 v1, Fix64 div)
    {
        return div == Fix64.Zero ? Zero : new FixVector3(v1.X / div, v1.Y / div, v1.Z / div);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator /(FixVector3 v1, FixVector3 v2)
    {
        return new FixVector3(
            v2.X == Fix64.Zero ? Fix64.Zero : v1.X / v2.X,
            v2.Y == Fix64.Zero ? Fix64.Zero : v1.Y / v2.Y,
            v2.Z == Fix64.Zero ? Fix64.Zero : v1.Z / v2.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator /(FixVector3 v1, int div)
    {
        return div == 0 ? Zero : new FixVector3(v1.X / div, v1.Y / div, v1.Z / div);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator /(Fix64 div, FixVector3 v1)
    {
        return new FixVector3(div / v1.X, div / v1.Y, div / v1.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator *(FixVector3 point, FixQuaternion rotation)
    {
        return rotation * point;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 operator *(FixQuaternion rotation, FixVector3 point)
    {
        Fix64 num1 = rotation.X * 2;
        Fix64 num2 = rotation.Y * 2;
        Fix64 num3 = rotation.Z * 2;
        Fix64 num4 = rotation.X * num1;
        Fix64 num5 = rotation.Y * num2;
        Fix64 num6 = rotation.Z * num3;
        Fix64 num7 = rotation.X * num2;
        Fix64 num8 = rotation.X * num3;
        Fix64 num9 = rotation.Y * num3;
        Fix64 num10 = rotation.W * num1;
        Fix64 num11 = rotation.W * num2;
        Fix64 num12 = rotation.W * num3;
        FixVector3 vector3 = new(
            (Fix64.One - (num5 + num6)) * point.X + (num7 - num12) * point.Y + (num8 + num11) * point.Z,
            (num7 + num12) * point.X + (Fix64.One - (num4 + num6)) * point.Y + (num9 - num10) * point.Z,
            (num8 - num11) * point.X + (num9 + num10) * point.Y + (Fix64.One - (num4 + num5)) * point.Z
        );
        return vector3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(FixVector3 left, FixVector3 right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(FixVector3 left, FixVector3 right)
    {
        return !left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(FixVector3 left, FixVector3 right)
    {
        return left.X > right.X
            && left.Y > right.Y
            && left.Z > right.Z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(FixVector3 left, FixVector3 right)
    {
        return left.X < right.X
            && left.Y < right.Y
            && left.Z < right.Z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(FixVector3 left, FixVector3 right)
    {
        return left.X >= right.X
            && left.Y >= right.Y
            && left.Z >= right.Z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(FixVector3 left, FixVector3 right)
    {
        return left.X <= right.X
            && left.Y <= right.Y
            && left.Z <= right.Z;
    }

    #endregion

    #region Conversion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString()
    {
        return string.Format("({0}, {1}, {2})", X.ToFormattedDouble(), Y.ToFormattedDouble(), Z.ToFormattedDouble());
    }

    /// <summary>
    /// Converts this <see cref="FixVector3"/> to a <see cref="FixVector2"/>, 
    /// dropping the Y component (height) of this vector in the resulting vector.
    /// </summary>
    /// <returns>
    /// A new <see cref="FixVector2"/> where (X, Z) from this <see cref="FixVector3"/> 
    /// become (X, Y) in the resulting vector.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly FixVector2 ToFixedVector2()
    {
        return new FixVector2(X, Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Deconstruct(out float x, out float y, out float z)
    {
        x = this.X.ToPreciseFloat();
        y = this.Y.ToPreciseFloat();
        z = this.Z.ToPreciseFloat();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Deconstruct(out int x, out int y, out int z)
    {
        x = this.X.RoundToInt();
        y = this.Y.RoundToInt();
        z = this.Z.RoundToInt();
    }

    /// <summary>
    /// Converts each component of the vector from radians to degrees.
    /// </summary>
    /// <param name="radians">The vector with components in radians.</param>
    /// <returns>A new vector with components converted to degrees.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 ToDegrees(FixVector3 radians)
    {
        return new FixVector3(
            MathFix.RadToDeg(radians.X),
            MathFix.RadToDeg(radians.Y),
            MathFix.RadToDeg(radians.Z));
    }

    /// <summary>
    /// Converts each component of the vector from degrees to radians.
    /// </summary>
    /// <param name="degrees">The vector with components in degrees.</param>
    /// <returns>A new vector with components converted to radians.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixVector3 ToRadians(FixVector3 degrees)
    {
        return new FixVector3(
            MathFix.DegToRad(degrees.X),
            MathFix.DegToRad(degrees.Y),
            MathFix.DegToRad(degrees.Z));
    }

    #endregion

    #region Equality and HashCode Overrides

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj)
    {
        return obj is FixVector3 other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(FixVector3 other)
    {
        return other.X == X && other.Y == Y && other.Z == Z;
    }

    public readonly bool Equals(FixVector3 x, FixVector3 y)
    {
        return x.Equals(y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        return StateHash;
    }

    public readonly int GetHashCode(FixVector3 obj)
    {
        return obj.GetHashCode();
    }

    public readonly int CompareTo(FixVector3 other)
    {
        return SqrMagnitude.CompareTo(other.SqrMagnitude);
    }

    #endregion
}