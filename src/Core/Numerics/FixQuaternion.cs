#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using Ocluse.LiquidSnow.Numerics.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Ocluse.LiquidSnow.Numerics;
/// <summary>
/// Represents a quaternion (x, y, z, w) with fixed-point numbers.
/// Quaternions are useful for representing rotations and can be used to perform smooth rotations and avoid gimbal lock.
/// </summary>
/// <remarks>
/// Creates a new FixedQuaternion with the specified components.
/// </remarks>
[Serializable]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct FixQuaternion(Fix64 x, Fix64 y, Fix64 z, Fix64 w) : IEquatable<FixQuaternion>
{
    #region Fields and Constants
    [JsonInclude]
    public Fix64 X = x;

    [JsonInclude]
    public Fix64 Y = y;

    [JsonInclude]
    public Fix64 Z = z;

    [JsonInclude]
    public Fix64 W = w;

    /// <summary>
    /// Identity quaternion (0, 0, 0, 1).
    /// </summary>
    public static readonly FixQuaternion Identity = new(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One);

    /// <summary>
    /// Empty quaternion (0, 0, 0, 0).
    /// </summary>
    public static readonly FixQuaternion Zero = new(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.Zero);

    #endregion

    #region Properties and Methods (Instance)

    /// <summary>
    /// Normalized version of this quaternion.
    /// </summary>
    [JsonIgnore]
    public readonly FixQuaternion Normal
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetNormalized(this);
    }

    /// <summary>
    /// Returns the Euler angles (in degrees) of this quaternion.
    /// </summary>
    [JsonIgnore]
    public FixVector3 EulerAngles
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => ToEulerAngles();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this = FromEulerAnglesInDegrees(value.X, value.Y, value.Z);
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
                3 => W,
                _ => throw new IndexOutOfRangeException("Invalid FixedQuaternion index!"),
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
                case 3:
                    W = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid FixedQuaternion index!");
            }
        }
    }

    /// <summary>
    /// Set x, y, z and w components of an existing Quaternion.
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <param name="newZ"></param>
    /// <param name="newW"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(Fix64 newX, Fix64 newY, Fix64 newZ, Fix64 newW)
    {
        X = newX;
        Y = newY;
        Z = newZ;
        W = newW;
    }

    /// <summary>
    /// Normalizes this quaternion in place.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public FixQuaternion Normalize()
    {
        return this = GetNormalized(this);
    }

    /// <summary>
    /// Returns the conjugate of this quaternion (inverses the rotational effect).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly FixQuaternion Conjugate()
    {
        return new FixQuaternion(-X, -Y, -Z, W);
    }

    /// <summary>
    /// Returns the inverse of this quaternion.
    /// </summary>
    public readonly FixQuaternion Inverse()
    {
        if (this == Identity) return Identity;
        Fix64 norm = X * X + Y * Y + Z * Z + W * W;
        if (norm == Fix64.Zero) return this; // Handle division by zero by returning the same quaternion

        Fix64 invNorm = Fix64.One / norm;
        return new FixQuaternion(X * -invNorm, Y * -invNorm, Z * -invNorm, W * invNorm);
    }

    /// <summary>
    /// Rotates a vector by this quaternion.
    /// </summary>
    public readonly FixVector3 Rotate(FixVector3 v)
    {
        FixQuaternion normalizedQuat = Normal;
        FixQuaternion vQuat = new(v.X, v.Y, v.Z, Fix64.Zero);
        FixQuaternion invQuat = normalizedQuat.Inverse();
        FixQuaternion rotatedVQuat = normalizedQuat * vQuat * invQuat;
        return new FixVector3(rotatedVQuat.X, rotatedVQuat.Y, rotatedVQuat.Z).Normalize();
    }

    /// <summary>
    /// Rotates this quaternion by a given angle around a specified axis (default: Y-axis).
    /// </summary>
    /// <param name="sin">Sine of the rotation angle.</param>
    /// <param name="cos">Cosine of the rotation angle.</param>
    /// <param name="axis">The axis to rotate around (default: FixedVector3.Up).</param>
    /// <returns>A new quaternion representing the rotated result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly FixQuaternion Rotated(Fix64 sin, Fix64 cos, FixVector3? axis = null)
    {
        FixVector3 rotateAxis = axis ?? FixVector3.Up;

        // The rotation angle is the arc tangent of sin and cos
        Fix64 angle = MathFix.Atan2(sin, cos);

        // Construct a quaternion representing a rotation around the axis (default is y aka FixedVector3.up)
        FixQuaternion rotationQuat = FromAxisAngle(rotateAxis, angle);

        // Apply the rotation and return the result
        return rotationQuat * this;
    }

    #endregion

    #region Quaternion Operations

    /// <summary>
    /// Checks if this vector has been normalized by checking if the magnitude is close to 1.
    /// </summary>
    public readonly bool IsNormalized()
    {
        Fix64 mag = GetMagnitude(this);
        return MathFix.Abs(mag - Fix64.One) <= Fix64.Epsilon;
    }

    public static Fix64 GetMagnitude(FixQuaternion q)
    {
        Fix64 mag = (q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z) + (q.W * q.W);
        // If rounding error caused the final magnitude to be slightly above 1, clamp it
        if (mag > Fix64.One && mag <= Fix64.One + Fix64.Epsilon)
            return Fix64.One;

        return mag != Fix64.Zero ? MathFix.Sqrt(mag) : Fix64.Zero;
    }

    /// <summary>
    /// Normalizes the quaternion to a unit quaternion.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixQuaternion GetNormalized(FixQuaternion q)
    {
        Fix64 mag = GetMagnitude(q);

        // If magnitude is zero, return identity quaternion (to avoid divide by zero)
        if (mag == Fix64.Zero)
            return new FixQuaternion(Fix64.Zero, Fix64.Zero, Fix64.Zero, Fix64.One);

        // If already normalized, return as-is
        if (mag == Fix64.One)
            return q;

        // Normalize it exactly
        return new FixQuaternion(
            q.X / mag,
            q.Y / mag,
            q.Z / mag,
            q.W / mag
        );
    }

    /// <summary>
    /// Creates a quaternion that rotates one vector to align with another.
    /// </summary>
    /// <param name="forward">The forward direction vector.</param>
    /// <param name="upwards">The upwards direction vector (optional, default: FixedVector3.Up).</param>
    /// <returns>A quaternion representing the rotation from one direction to another.</returns>
    public static FixQuaternion LookRotation(FixVector3 forward, FixVector3? upwards = null)
    {
        FixVector3 up = upwards ?? FixVector3.Up;

        FixVector3 forwardNormalized = forward.Normal;
        FixVector3 right = FixVector3.Cross(up.Normal, forwardNormalized);
        up = FixVector3.Cross(forwardNormalized, right);

        return FromMatrix(new Fix3x3(right.X, up.X, forwardNormalized.X,
                                        right.Y, up.Y, forwardNormalized.Y,
                                        right.Z, up.Z, forwardNormalized.Z));
    }

    /// <summary>
    /// Converts a rotation matrix into a quaternion representation.
    /// </summary>
    /// <param name="matrix">The rotation matrix to convert.</param>
    /// <returns>A quaternion representing the same rotation as the matrix.</returns>
    public static FixQuaternion FromMatrix(Fix3x3 matrix)
    {
        Fix64 trace = matrix.M00 + matrix.M11 + matrix.M22;

        Fix64 w, x, y, z;

        if (trace > Fix64.Zero)
        {
            Fix64 s = MathFix.Sqrt(trace + Fix64.One);
            w = s * Fix64.Half;
            s = Fix64.Half / s;
            x = (matrix.M21 - matrix.M12) * s;
            y = (matrix.M02 - matrix.M20) * s;
            z = (matrix.M10 - matrix.M01) * s;
        }
        else if (matrix.M00 > matrix.M11 && matrix.M00 > matrix.M22)
        {
            Fix64 s = MathFix.Sqrt(Fix64.One + matrix.M00 - matrix.M11 - matrix.M22);
            x = s * Fix64.Half;
            s = Fix64.Half / s;
            y = (matrix.M10 + matrix.M01) * s;
            z = (matrix.M02 + matrix.M20) * s;
            w = (matrix.M21 - matrix.M12) * s;
        }
        else if (matrix.M11 > matrix.M22)
        {
            Fix64 s = MathFix.Sqrt(Fix64.One + matrix.M11 - matrix.M00 - matrix.M22);
            y = s * Fix64.Half;
            s = Fix64.Half / s;
            z = (matrix.M21 + matrix.M12) * s;
            x = (matrix.M10 + matrix.M01) * s;
            w = (matrix.M02 - matrix.M20) * s;
        }
        else
        {
            Fix64 s = MathFix.Sqrt(Fix64.One + matrix.M22 - matrix.M00 - matrix.M11);
            z = s * Fix64.Half;
            s = Fix64.Half / s;
            x = (matrix.M02 + matrix.M20) * s;
            y = (matrix.M21 + matrix.M12) * s;
            w = (matrix.M10 - matrix.M01) * s;
        }

        return new FixQuaternion(x, y, z, w);
    }

    /// <summary>
    /// Converts a rotation matrix (upper-left 3x3 part of a 4x4 matrix) into a quaternion representation.
    /// </summary>
    /// <param name="matrix">The 4x4 matrix containing the rotation component.</param>
    /// <remarks>Extracts the upper-left 3x3 rotation part of the 4x4</remarks>
    /// <returns>A quaternion representing the same rotation as the matrix.</returns>
    public static FixQuaternion FromMatrix(Fix4x4 matrix)
    {

        var rotationMatrix = new Fix3x3(
            matrix.M00, matrix.M01, matrix.M02,
            matrix.M10, matrix.M11, matrix.M12,
            matrix.M20, matrix.M21, matrix.M22
        );

        return FromMatrix(rotationMatrix);
    }

    /// <summary>
    /// Creates a quaternion representing the rotation needed to align the forward vector with the given direction.
    /// </summary>
    /// <param name="direction">The target direction vector.</param>
    /// <returns>A quaternion representing the rotation to align with the direction.</returns>
    public static FixQuaternion FromDirection(FixVector3 direction)
    {
        // Compute the rotation axis as the cross product of the standard forward vector and the desired direction
        FixVector3 axis = FixVector3.Cross(FixVector3.Forward, direction);
        Fix64 axisLength = axis.Magnitude;

        // If the axis length is very close to zero, it means that the desired direction is almost equal to the standard forward vector
        if (axisLength.Abs() == Fix64.Zero)
            return Identity;  // Return the identity quaternion if no rotation is needed

        // Normalize the rotation axis
        axis = (axis / axisLength).Normal;

        // Compute the angle between the standard forward vector and the desired direction
        Fix64 angle = MathFix.Acos(FixVector3.Dot(FixVector3.Forward, direction));

        // Compute the rotation quaternion from the axis and angle
        return FromAxisAngle(axis, angle);
    }

    /// <summary>
    /// Creates a quaternion representing a rotation around a specified axis by a given angle.
    /// </summary>
    /// <param name="axis">The axis to rotate around (must be normalized).</param>
    /// <param name="angle">The rotation angle in radians.</param>
    /// <returns>A quaternion representing the rotation.</returns>
    public static FixQuaternion FromAxisAngle(FixVector3 axis, Fix64 angle)
    {
        // Check if the axis is a unit vector
        if (!axis.IsNormalized())
            axis = axis.Normalize();

        // Check if the angle is in a valid range (-pi, pi)
        if (angle < -MathFix.PI || angle > MathFix.PI)
            throw new ArgumentOutOfRangeException(nameof(angle), "Angle must be in the range (-pi, pi)");

        Fix64 halfAngle = angle / Fix64.Two;  // Half-angle formula
        Fix64 sinHalfAngle = MathFix.Sin(halfAngle);
        Fix64 cosHalfAngle = MathFix.Cos(halfAngle);

        return new FixQuaternion(axis.X * sinHalfAngle, axis.Y * sinHalfAngle, axis.Z * sinHalfAngle, cosHalfAngle);
    }

    /// <summary>
    /// Assume the input angles are in degrees and converts them to radians before calling <see cref="FromEulerAngles"/> 
    /// </summary>
    /// <param name="pitch"></param>
    /// <param name="yaw"></param>
    /// <param name="roll"></param>
    /// <returns></returns>
    public static FixQuaternion FromEulerAnglesInDegrees(Fix64 pitch, Fix64 yaw, Fix64 roll)
    {
        // Convert input angles from degrees to radians
        pitch = MathFix.DegToRad(pitch);
        yaw = MathFix.DegToRad(yaw);
        roll = MathFix.DegToRad(roll);

        // Call the original method that expects angles in radians
        return FromEulerAngles(pitch, yaw, roll).Normalize();
    }

    /// <summary>
    /// Converts Euler angles (pitch, yaw, roll) to a quaternion and normalizes the result afterwards. Assumes the input angles are in radians.
    /// </summary>
    /// <remarks>
    /// The order of operations is YZX or yaw-roll-pitch, commonly used in applications such as robotics.
    /// </remarks>
    public static FixQuaternion FromEulerAngles(Fix64 pitch, Fix64 yaw, Fix64 roll)
    {
        // Check if the angles are in a valid range (-pi, pi)
        if (pitch < -MathFix.PI || pitch > MathFix.PI ||
            yaw < -MathFix.PI || yaw > MathFix.PI ||
            roll < -MathFix.PI || roll > MathFix.PI)
        {
            throw new ArgumentOutOfRangeException(nameof(pitch), "Euler angles must be in the range (-pi, pi)");
        }

        Fix64 c1 = MathFix.Cos(yaw / Fix64.Two);
        Fix64 s1 = MathFix.Sin(yaw / Fix64.Two);
        Fix64 c2 = MathFix.Cos(roll / Fix64.Two);
        Fix64 s2 = MathFix.Sin(roll / Fix64.Two);
        Fix64 c3 = MathFix.Cos(pitch / Fix64.Two);
        Fix64 s3 = MathFix.Sin(pitch / Fix64.Two);

        Fix64 c1c2 = c1 * c2;
        Fix64 s1s2 = s1 * s2;

        Fix64 w = c1c2 * c3 - s1s2 * s3;
        Fix64 x = c1c2 * s3 + s1s2 * c3;
        Fix64 y = s1 * c2 * c3 + c1 * s2 * s3;
        Fix64 z = c1 * s2 * c3 - s1 * c2 * s3;

        return GetNormalized(new FixQuaternion(x, y, z, w));
    }

    /// <summary>
    /// Computes the logarithm of a quaternion, which represents the rotational displacement.
    /// This is useful for interpolation and angular velocity calculations.
    /// </summary>
    /// <param name="q">The quaternion to compute the logarithm of.</param>
    /// <returns>A FixedVector3 representing the logarithm of the quaternion (axis-angle representation).</returns>
    /// <remarks>
    /// The logarithm of a unit quaternion is given by:
    /// log(q) = (θ * v̂), where:
    /// - θ = 2 * acos(w) is the rotation angle.
    /// - v̂ = (x, y, z) / ||(x, y, z)|| is the unit vector representing the axis of rotation.
    /// If the quaternion is close to identity, the function returns a zero vector to avoid numerical instability.
    /// </remarks>
    public static FixVector3 QuaternionLog(FixQuaternion q)
    {
        // Ensure the quaternion is normalized
        q = q.Normal;

        // Extract vector part
        FixVector3 v = new(q.X, q.Y, q.Z);
        Fix64 vLength = v.Magnitude;

        // If rotation is very small, avoid division by zero
        if (vLength < Fix64.FromRaw(0x00001000L)) // Small epsilon
            return FixVector3.Zero;

        // Compute angle (theta = 2 * acos(w))
        Fix64 theta = Fix64.Two * MathFix.Acos(q.W);

        // Convert to angular velocity
        return (v / vLength) * theta;
    }

    /// <summary>
    /// Computes the angular velocity required to move from `previousRotation` to `currentRotation` over a given time step.
    /// </summary>
    /// <param name="currentRotation">The current orientation as a quaternion.</param>
    /// <param name="previousRotation">The previous orientation as a quaternion.</param>
    /// <param name="deltaTime">The time step over which the rotation occurs.</param>
    /// <returns>A FixedVector3 representing the angular velocity (in radians per second).</returns>
    /// <remarks>
    /// This function calculates the change in rotation over `deltaTime` and converts it into angular velocity.
    /// - First, it computes the relative rotation: `rotationDelta = currentRotation * previousRotation.Inverse()`.
    /// - Then, it applies `QuaternionLog(rotationDelta)` to extract the axis-angle representation.
    /// - Finally, it divides by `deltaTime` to compute the angular velocity.
    /// </remarks>
    public static FixVector3 ToAngularVelocity(
        FixQuaternion currentRotation,
        FixQuaternion previousRotation,
        Fix64 deltaTime)
    {
        FixQuaternion rotationDelta = currentRotation * previousRotation.Inverse();
        FixVector3 angularDisplacement = QuaternionLog(rotationDelta);

        return angularDisplacement / deltaTime; // Convert to angular velocity
    }

    /// <summary>
    /// Performs a simple linear interpolation between the components of the input quaternions
    /// </summary>
    public static FixQuaternion Lerp(FixQuaternion a, FixQuaternion b, Fix64 t)
    {
        t = MathFix.Clamp01(t);

        FixQuaternion result;
        Fix64 oneMinusT = Fix64.One - t;
        result.X = a.X * oneMinusT + b.X * t;
        result.Y = a.Y * oneMinusT + b.Y * t;
        result.Z = a.Z * oneMinusT + b.Z * t;
        result.W = a.W * oneMinusT + b.W * t;

        result.Normalize();

        return result;
    }

    /// <summary>
    ///  Calculates the spherical linear interpolation, which results in a smoother and more accurate rotation interpolation
    /// </summary>
    public static FixQuaternion Slerp(FixQuaternion a, FixQuaternion b, Fix64 t)
    {
        t = MathFix.Clamp01(t);

        Fix64 cosOmega = a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

        // If the dot product is negative, negate one of the input quaternions.
        // This ensures that the interpolation takes the shortest path around the sphere.
        if (cosOmega < Fix64.Zero)
        {
            b.X = -b.X;
            b.Y = -b.Y;
            b.Z = -b.Z;
            b.W = -b.W;
            cosOmega = -cosOmega;
        }

        Fix64 k0, k1;

        // If the quaternions are close, use linear interpolation
        if (cosOmega > Fix64.One - Fix64.Precision)
        {
            k0 = Fix64.One - t;
            k1 = t;
        }
        else
        {
            // Otherwise, use spherical linear interpolation
            Fix64 sinOmega = MathFix.Sqrt(Fix64.One - cosOmega * cosOmega);
            Fix64 omega = MathFix.Atan2(sinOmega, cosOmega);

            k0 = MathFix.Sin((Fix64.One - t) * omega) / sinOmega;
            k1 = MathFix.Sin(t * omega) / sinOmega;
        }

        FixQuaternion result;
        result.X = a.X * k0 + b.X * k1;
        result.Y = a.Y * k0 + b.Y * k1;
        result.Z = a.Z * k0 + b.Z * k1;
        result.W = a.W * k0 + b.W * k1;

        return result;
    }

    /// <summary>
    /// Returns the angle in degrees between two rotations a and b.
    /// </summary>
    /// <param name="a">The first rotation.</param>
    /// <param name="b">The second rotation.</param>
    /// <returns>The angle in degrees between the two rotations.</returns>
    public static Fix64 Angle(FixQuaternion a, FixQuaternion b)
    {
        // Calculate the dot product of the two quaternions
        Fix64 dot = Dot(a, b);

        // Ensure the dot product is in the range of [-1, 1] to avoid floating-point inaccuracies
        dot = MathFix.Clamp(dot, -Fix64.One, Fix64.One);

        // Calculate the angle between the two quaternions using the inverse cosine (arccos)
        // arccos(dot(a, b)) gives us the angle in radians, so we convert it to degrees
        Fix64 angleInRadians = MathFix.Acos(dot);

        // Convert the angle from radians to degrees
        Fix64 angleInDegrees = MathFix.RadToDeg(angleInRadians);

        return angleInDegrees;
    }

    /// <summary>
    /// Creates a quaternion from an angle and axis.
    /// </summary>
    /// <param name="angle">The angle in degrees.</param>
    /// <param name="axis">The axis to rotate around (must be normalized).</param>
    /// <returns>A quaternion representing the rotation.</returns>
    public static FixQuaternion AngleAxis(Fix64 angle, FixVector3 axis)
    {
        // Convert the angle to radians
        angle = angle.ToRadians();

        // Normalize the axis
        axis = axis.Normal;

        // Use the half-angle formula (sin(theta / 2), cos(theta / 2))
        Fix64 halfAngle = angle / Fix64.Two;
        Fix64 sinHalfAngle = MathFix.Sin(halfAngle);
        Fix64 cosHalfAngle = MathFix.Cos(halfAngle);

        return new FixQuaternion(
            axis.X * sinHalfAngle,
            axis.Y * sinHalfAngle,
            axis.Z * sinHalfAngle,
            cosHalfAngle
        );
    }

    /// <summary>
    /// Calculates the dot product of two quaternions.
    /// </summary>
    /// <param name="a">The first quaternion.</param>
    /// <param name="b">The second quaternion.</param>
    /// <returns>The dot product of the two quaternions.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fix64 Dot(FixQuaternion a, FixQuaternion b)
    {
        return a.W * b.W + a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    #endregion

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixQuaternion operator *(FixQuaternion a, FixQuaternion b)
    {
        return new FixQuaternion(
            a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
            a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
            a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W,
            a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixQuaternion operator *(FixQuaternion q, Fix64 scalar)
    {
        return new FixQuaternion(q.X * scalar, q.Y * scalar, q.Z * scalar, q.W * scalar);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixQuaternion operator *(Fix64 scalar, FixQuaternion q)
    {
        return new FixQuaternion(q.X * scalar, q.Y * scalar, q.Z * scalar, q.W * scalar);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixQuaternion operator /(FixQuaternion q, Fix64 scalar)
    {
        return new FixQuaternion(q.X / scalar, q.Y / scalar, q.Z / scalar, q.W / scalar);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixQuaternion operator /(Fix64 scalar, FixQuaternion q)
    {
        return new FixQuaternion(q.X / scalar, q.Y / scalar, q.Z / scalar, q.W / scalar);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FixQuaternion operator +(FixQuaternion q1, FixQuaternion q2)
    {
        return new FixQuaternion(q1.X + q2.X, q1.Y + q2.Y, q1.Z + q2.Z, q1.W + q2.W);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(FixQuaternion left, FixQuaternion right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(FixQuaternion left, FixQuaternion right)
    {
        return !left.Equals(right);
    }

    #endregion

    #region Conversion

    /// <summary>
    /// Converts this FixedQuaternion to Euler angles (pitch, yaw, roll).
    /// </summary>
    /// <remarks>
    /// Handles the case where the pitch angle (asin of sinp) would be out of the range -π/2 to π/2. 
    /// This is known as the gimbal lock situation, where the pitch angle reaches ±90 degrees and we lose one degree of freedom in our rotation (we can't distinguish between yaw and roll). 
    /// In this case, we simply set the pitch to ±90 degrees depending on the sign of sinp.
    /// </remarks>
    /// <returns>A FixedVector3 representing the Euler angles (in degrees) equivalent to this FixedQuaternion in YZX order (yaw, pitch, roll).</returns>
    public readonly FixVector3 ToEulerAngles()
    {
        // roll (x-axis rotation)
        Fix64 sinr_cosp = 2 * (W * X + Y * Z);
        Fix64 cosr_cosp = Fix64.One - 2 * (X * X + Y * Y);
        Fix64 roll = MathFix.Atan2(sinr_cosp, cosr_cosp);

        // pitch (y-axis rotation)
        Fix64 sinp = 2 * (W * Y - Z * X);
        Fix64 pitch;
        if (sinp.Abs() >= Fix64.One)
            pitch = MathFix.CopySign(MathFix.PiOver2, sinp); // use 90 degrees if out of range
        else
            pitch = MathFix.Asin(sinp);

        // yaw (z-axis rotation)
        Fix64 siny_cosp = 2 * (W * Z + X * Y);
        Fix64 cosy_cosp = Fix64.One - 2 * (Y * Y + Z * Z);
        Fix64 yaw = MathFix.Atan2(siny_cosp, cosy_cosp);

        // Convert radians to degrees
        roll = MathFix.RadToDeg(roll);
        pitch = MathFix.RadToDeg(pitch);
        yaw = MathFix.RadToDeg(yaw);

        return new FixVector3(roll, pitch, yaw);
    }

    /// <summary>
    /// Converts this FixedQuaternion to a direction vector.
    /// </summary>
    /// <returns>A FixedVector3 representing the direction equivalent to this FixedQuaternion.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly FixVector3 ToDirection()
    {
        return new FixVector3(
            2 * (X * Z - W * Y),
            2 * (Y * Z + W * X),
            Fix64.One - 2 * (X * X + Y * Y)
        );
    }

    /// <summary>
    /// Converts the quaternion into a 3x3 rotation matrix.
    /// </summary>
    /// <returns>A FixedMatrix3x3 representing the same rotation as the quaternion.</returns>
    public readonly Fix3x3 ToMatrix3x3()
    {
        Fix64 x2 = X * X;
        Fix64 y2 = Y * Y;
        Fix64 z2 = Z * Z;
        Fix64 xy = X * Y;
        Fix64 xz = X * Z;
        Fix64 yz = Y * Z;
        Fix64 xw = X * W;
        Fix64 yw = Y * W;
        Fix64 zw = Z * W;

        Fix3x3 result = new();
        Fix64 scale = Fix64.One * 2;

        result.M00 = Fix64.One - scale * (y2 + z2);
        result.M01 = scale * (xy - zw);
        result.M02 = scale * (xz + yw);

        result.M10 = scale * (xy + zw);
        result.M11 = Fix64.One - scale * (x2 + z2);
        result.M12 = scale * (yz - xw);

        result.M20 = scale * (xz - yw);
        result.M21 = scale * (yz + xw);
        result.M22 = Fix64.One - scale * (x2 + y2);

        return result;
    }

    #endregion

    #region Equality and HashCode Overrides

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly bool Equals(object? obj)
    {
        return obj is FixQuaternion other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(FixQuaternion other)
    {
        return other.X == X && other.Y == Y && other.Z == Z && other.W == W;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2 ^ W.GetHashCode();
    }

    /// <summary>
    /// Returns a formatted string for this quaternion.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override readonly string ToString()
    {
        return $"({X}, {Y}, {Z}, {W})";
    }

    #endregion
}
