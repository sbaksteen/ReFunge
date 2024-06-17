using System.Numerics;
using ReFunge.Data;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.DataTypes;

using FungeFloat = FPSP.FungeFloat;

/// <summary>
///     3DSP: 3D Space Manipulation <br />
///     Allows for linear algebra operations in 3D space. This includes 3D vectors and 4x4 matrices. <br />
///     From RC/Funge-98.
/// </summary>
/// <remarks>
///     Many of the instructions in this fingerprint make use of 4x4 matrices stored in Funge-Space. These matrices are
///     stored as 16 float values in row-major order. Any instruction that requires a matrix will reflect if the IP
///     is only 1D, as the matrices are stored in a 2D manner.
/// </remarks>
[Fingerprint("3DSP")]
public class _3DSP
{
    private static Matrix4x4 MatrixFromSpace(FungeSpace space, FungeVector start)
    {
        var matrix = Matrix4x4.Identity;
        for (var y = 0; y < 4; y++)
        for (var x = 0; x < 4; x++)
        {
            var value = space[start + new FungeVector(x, y)];
            var fValue = BitConverter.Int32BitsToSingle(value);
            matrix[x, y] = fValue;
        }

        return matrix;
    }

    private static void MatrixToSpace(FungeSpace space, FungeVector start, Matrix4x4 matrix)
    {
        for (var y = 0; y < 4; y++)
        for (var x = 0; x < 4; x++)
        {
            var value = matrix[x, y];
            space[start + new FungeVector(x, y)] = BitConverter.SingleToInt32Bits(value);
        }
    }

    /// <summary>
    ///     Add two 3D vectors element-wise.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the addition.</returns>
    [Instruction('A')]
    public static FungeVector3D Add(FungeIP _, FungeVector3D a, FungeVector3D b)
    {
        return a + b;
    }

    /// <summary>
    ///     Subtract two 3D vectors element-wise.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The minuend.</param>
    /// <param name="b">The subtrahend.</param>
    /// <returns>The result of the subtraction.</returns>
    [Instruction('B')]
    public static FungeVector3D Subtract(FungeIP _, FungeVector3D a, FungeVector3D b)
    {
        return a - b;
    }

    /// <summary>
    ///     Cross product of two 3D vectors.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The cross product of the two vectors.</returns>
    [Instruction('C')]
    public static FungeVector3D Cross(FungeIP _, FungeVector3D a, FungeVector3D b)
    {
        return FungeVector3D.Cross(a, b);
    }

    /// <summary>
    ///     Dot product of two 3D vectors.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The dot product of the two vectors.</returns>
    [Instruction('D')]
    public static FungeFloat Dot(FungeIP _, FungeVector3D a, FungeVector3D b)
    {
        return FungeVector3D.Dot(a, b);
    }

    /// <summary>
    ///     Get the length (magnitude) of a 3D vector.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The vector.</param>
    /// <returns>The length of the vector.</returns>
    [Instruction('L')]
    public static FungeFloat Length(FungeIP _, FungeVector3D a)
    {
        return a.Length;
    }

    /// <summary>
    ///     Multiply two 3D vectors element-wise.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the multiplication.</returns>
    [Instruction('M')]
    public static FungeVector3D Multiply(FungeIP _, FungeVector3D a, FungeVector3D b)
    {
        return a * b;
    }

    /// <summary>
    ///     Normalize a 3D vector (divide it by its length).
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The vector to normalize.</param>
    /// <returns>The normalized vector.</returns>
    [Instruction('N')]
    public static FungeVector3D Normalize(FungeIP _, FungeVector3D a)
    {
        return FungeVector3D.Normalize(a);
    }

    /// <summary>
    ///     Duplicate the 3D vector on top of the stack. Pushes two copies of the vector.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="a">The vector to duplicate.</param>
    [Instruction('U')]
    public static void Duplicate(FungeIP ip, FungeVector3D a)
    {
        a.PushToStack(ip);
        a.PushToStack(ip);
    }

    /// <summary>
    ///     Project a 3D vector onto the XY plane. Pushes the X and Y components of the vector as <see cref="FungeFloat" />s.
    ///     Each component is divided by the Z component of the vector, unless the Z component is 0, in which case the
    ///     component is pushed as-is.
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="a"></param>
    [Instruction('V')]
    public static void ViewProject(FungeIP ip, FungeVector3D a)
    {
        var x = new FungeFloat(a.X / a.Z);
        var y = new FungeFloat(a.Y / a.Z);
        if (a.Z == 0)
        {
            x = a.X;
            y = a.Y;
        }

        x.PushToStack(ip);
        y.PushToStack(ip);
    }

    /// <summary>
    ///     Multiply a 3D vector by a scalar.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The vector to multiply.</param>
    /// <param name="b">The scalar to multiply by.</param>
    /// <returns>The result of the multiplication.</returns>
    [Instruction('Z')]
    public static FungeVector3D ScalarMultiply(FungeIP _, FungeVector3D a, FungeFloat b)
    {
        return a * b;
    }

    /// <summary>
    ///     Copy a 4x4 matrix from one location in Funge-Space to another.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="dest">The destination to copy to.</param>
    /// <param name="src">The source location to copy from.</param>
    [Instruction('P', 2)]
    public static void CopyMatrix(FungeIP ip, FungeVector dest, FungeVector src)
    {
        var space = ip.Space;
        var matrix = MatrixFromSpace(space, src + ip.StorageOffset);
        MatrixToSpace(space, dest + ip.StorageOffset, matrix);
    }

    /// <summary>
    ///     Create a 4x4 rotation matrix at the specified location in Funge-Space, around the specified axis and with
    ///     the specified angle.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="dest">The location to store the matrix.</param>
    /// <param name="axis">The axis to rotate around (1 = X, 2 = Y, 3 = Z).</param>
    /// <param name="angle">The angle to rotate by, in degrees.</param>
    [Instruction('R', 2)]
    public static void RotationMatrix(FungeIP ip, FungeVector dest, FungeInt axis, FungeFloat angle)
    {
        var space = ip.Space;
        angle = float.DegreesToRadians(angle);
        var matrix = (int)axis switch
        {
            1 => Matrix4x4.CreateRotationX(angle),
            2 => Matrix4x4.CreateRotationY(angle),
            3 => Matrix4x4.CreateRotationZ(angle),
            _ => Matrix4x4.Identity
        };

        MatrixToSpace(space, dest + ip.StorageOffset, matrix);
    }

    /// <summary>
    ///     Create a 4x4 scaling matrix at the specified location in Funge-Space, with the specified scaling factors.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="dest">The location to store the matrix.</param>
    /// <param name="scale">The scaling factors for each axis.</param>
    [Instruction('S', 2)]
    public static void ScalingMatrix(FungeIP ip, FungeVector dest, FungeVector3D scale)
    {
        var space = ip.Space;
        var matrix = Matrix4x4.CreateScale(scale.Value);
        MatrixToSpace(space, dest + ip.StorageOffset, matrix);
    }

    /// <summary>
    ///     Create a 4x4 translation matrix at the specified location in Funge-Space, with the specified translation.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="dest">The location to store the matrix.</param>
    /// <param name="translation">The translation vector.</param>
    [Instruction('T', 2)]
    public static void TranslationMatrix(FungeIP ip, FungeVector dest, FungeVector3D translation)
    {
        var space = ip.Space;
        var matrix = Matrix4x4.CreateTranslation(translation.Value);
        MatrixToSpace(space, dest + ip.StorageOffset, matrix);
    }

    /// <summary>
    ///     Multiply a 3D vector by a 4x4 matrix in Funge-Space.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="vector">The vector to multiply.</param>
    /// <param name="matrixSrc">The location of the matrix to multiply by.</param>
    /// <returns>The result of the multiplication.</returns>
    [Instruction('X', 2)]
    public static FungeVector3D VectorMatrixMultiply(FungeIP ip, FungeVector3D vector, FungeVector matrixSrc)
    {
        var space = ip.Space;
        var matrix = MatrixFromSpace(space, matrixSrc + ip.StorageOffset);
        var result = Vector3.Transform(vector.Value, matrix);
        return new FungeVector3D(result);
    }

    /// <summary>
    ///     Multiply two 4x4 matrices in Funge-Space.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="dest">The location to store the result.</param>
    /// <param name="srcA">The location of the first matrix.</param>
    /// <param name="srcB">The location of the second matrix.</param>
    [Instruction('Y', 2)]
    public static void MatrixMatrixMultiply(FungeIP ip, FungeVector dest, FungeVector srcA, FungeVector srcB)
    {
        var space = ip.Space;
        var matrixA = MatrixFromSpace(space, srcA + ip.StorageOffset);
        var matrixB = MatrixFromSpace(space, srcB + ip.StorageOffset);
        var result = matrixA * matrixB;
        MatrixToSpace(space, dest + ip.StorageOffset, result);
    }

    /// <summary>
    ///     Represents a 3D vector. This is a simple wrapper around <see cref="Vector3" /> with basic linear algebra
    ///     operations.
    ///     It implements <see cref="IFungeValue{TSelf}" /> for itself, facilitating its use in Funge instructions.
    /// </summary>
    public readonly record struct FungeVector3D : IFungeValue<FungeVector3D>
    {
        private readonly Vector3 _value;

        /// <summary>
        ///     Create a new 3D vector with the given value.
        /// </summary>
        /// <param name="value">The value of the vector.</param>
        public FungeVector3D(Vector3 value)
        {
            _value = value;
        }

        /// <summary>
        ///     The length (magnitude) of the vector.
        /// </summary>
        public FungeFloat Length => new(_value.Length());

        /// <summary>
        ///     The X component of the vector.
        /// </summary>
        public FungeFloat X => new(_value.X);

        /// <summary>
        ///     The Y component of the vector.
        /// </summary>
        public FungeFloat Y => new(_value.Y);

        /// <summary>
        ///     The Z component of the vector.
        /// </summary>
        public FungeFloat Z => new(_value.Z);

        /// <summary>
        ///     The underlying <see cref="Vector3" /> value.
        /// </summary>
        public Vector3 Value => _value;

        /// <summary>
        ///     Pop a 3D vector from the stack. Three floats are popped, then combined into a 3D vector.
        ///     The Z component is popped first, followed by Y and X.
        /// </summary>
        /// <param name="ip">The IP executing the instruction.</param>
        /// <returns>The popped 3D vector.</returns>
        public static FungeVector3D PopFromStack(FungeIP ip)
        {
            var z = FungeFloat.PopFromStack(ip);
            var y = FungeFloat.PopFromStack(ip);
            var x = FungeFloat.PopFromStack(ip);
            return new FungeVector3D(new Vector3(x, y, z));
        }

        /// <summary>
        ///     Push the 3D vector to the stack. The X, Y, and Z components are pushed in that order, as floats.
        /// </summary>
        /// <param name="ip">The IP executing the instruction.</param>
        public void PushToStack(FungeIP ip)
        {
            new FungeFloat(_value.X).PushToStack(ip);
            new FungeFloat(_value.Y).PushToStack(ip);
            new FungeFloat(_value.Z).PushToStack(ip);
        }

        /// <summary>
        ///     Add two 3D vectors element-wise.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns>The result of the addition.</returns>
        public static FungeVector3D operator +(FungeVector3D a, FungeVector3D b)
        {
            return new FungeVector3D(a._value + b._value);
        }

        /// <summary>
        ///     Subtract two 3D vectors element-wise.
        /// </summary>
        /// <param name="a">The minuend.</param>
        /// <param name="b">The subtrahend.</param>
        /// <returns>The result of the subtraction.</returns>
        public static FungeVector3D operator -(FungeVector3D a, FungeVector3D b)
        {
            return new FungeVector3D(a._value - b._value);
        }

        /// <summary>
        ///     Multiply two 3D vectors element-wise.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns>The result of the multiplication</returns>
        public static FungeVector3D operator *(FungeVector3D a, FungeVector3D b)
        {
            return new FungeVector3D(a._value * b._value);
        }

        /// <summary>
        ///     Multiply a 3D vector by a scalar.
        /// </summary>
        /// <param name="a">The vector to multiply.</param>
        /// <param name="b">The scalar to multiply by.</param>
        /// <returns>The result of the multiplication.</returns>
        public static FungeVector3D operator *(FungeVector3D a, FungeFloat b)
        {
            return new FungeVector3D(a._value * b);
        }

        /// <summary>
        ///     The cross product of two 3D vectors.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns>The cross product of the two vectors.</returns>
        public static FungeVector3D Cross(FungeVector3D a, FungeVector3D b)
        {
            return new FungeVector3D(Vector3.Cross(a._value, b._value));
        }

        /// <summary>
        ///     The dot product of two 3D vectors.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public static FungeFloat Dot(FungeVector3D a, FungeVector3D b)
        {
            return new FungeFloat(Vector3.Dot(a._value, b._value));
        }

        /// <summary>
        ///     Normalize a 3D vector.
        /// </summary>
        /// <param name="a">The vector to normalize.</param>
        /// <returns>The normalized vector.</returns>
        public static FungeVector3D Normalize(FungeVector3D a)
        {
            return new FungeVector3D(Vector3.Normalize(a._value));
        }
    }
}