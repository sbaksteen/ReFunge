using System.Numerics;
using ReFunge.Data;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

using FungeFloat = FPSP.FungeFloat;

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

    [Instruction('A')]
    public static FungeVector3D Add(FungeIP _, FungeVector3D a, FungeVector3D b)
    {
        return a + b;
    }

    [Instruction('B')]
    public static FungeVector3D Subtract(FungeIP _, FungeVector3D a, FungeVector3D b)
    {
        return a - b;
    }

    [Instruction('C')]
    public static FungeVector3D Cross(FungeIP _, FungeVector3D a, FungeVector3D b)
    {
        return FungeVector3D.Cross(a, b);
    }

    [Instruction('D')]
    public static FungeFloat Dot(FungeIP _, FungeVector3D a, FungeVector3D b)
    {
        return FungeVector3D.Dot(a, b);
    }

    [Instruction('L')]
    public static FungeFloat Length(FungeIP _, FungeVector3D a)
    {
        return a.Length;
    }

    [Instruction('M')]
    public static FungeVector3D Multiply(FungeIP _, FungeVector3D a, FungeVector3D b)
    {
        return a * b;
    }

    [Instruction('N')]
    public static FungeVector3D Normalize(FungeIP _, FungeVector3D a)
    {
        return FungeVector3D.Normalize(a);
    }

    [Instruction('U')]
    public static void Duplicate(FungeIP ip, FungeVector3D a)
    {
        a.PushToStack(ip);
        a.PushToStack(ip);
    }

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

    [Instruction('Z')]
    public static FungeVector3D ScalarMultiply(FungeIP _, FungeVector3D a, FungeFloat b)
    {
        return a * b;
    }

    [Instruction('P', 2)]
    public static void CopyMatrix(FungeIP ip, FungeVector dest, FungeVector src)
    {
        var space = ip.Space;
        var matrix = MatrixFromSpace(space, src + ip.StorageOffset);
        MatrixToSpace(space, dest + ip.StorageOffset, matrix);
    }

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

    [Instruction('S', 2)]
    public static void ScalingMatrix(FungeIP ip, FungeVector dest, FungeVector3D scale)
    {
        var space = ip.Space;
        var matrix = Matrix4x4.CreateScale(scale.Value);
        MatrixToSpace(space, dest + ip.StorageOffset, matrix);
    }

    [Instruction('T', 2)]
    public static void TranslationMatrix(FungeIP ip, FungeVector dest, FungeVector3D translation)
    {
        var space = ip.Space;
        var matrix = Matrix4x4.CreateTranslation(translation.Value);
        MatrixToSpace(space, dest + ip.StorageOffset, matrix);
    }

    [Instruction('X', 2)]
    public static FungeVector3D VectorMatrixMultiply(FungeIP ip, FungeVector3D vector, FungeVector matrixSrc)
    {
        var space = ip.Space;
        var matrix = MatrixFromSpace(space, matrixSrc + ip.StorageOffset);
        var result = Vector3.Transform(vector.Value, matrix);
        return new FungeVector3D(result);
    }

    [Instruction('Y', 2)]
    public static void MatrixMatrixMultiply(FungeIP ip, FungeVector dest, FungeVector srcA, FungeVector srcB)
    {
        var space = ip.Space;
        var matrixA = MatrixFromSpace(space, srcA + ip.StorageOffset);
        var matrixB = MatrixFromSpace(space, srcB + ip.StorageOffset);
        var result = matrixA * matrixB;
        MatrixToSpace(space, dest + ip.StorageOffset, result);
    }

    public struct FungeVector3D : IFungeValue<FungeVector3D>
    {
        private readonly Vector3 _value;

        public FungeVector3D(Vector3 value)
        {
            _value = value;
        }

        public static FungeVector3D PopFromStack(FungeIP ip)
        {
            var z = FungeFloat.PopFromStack(ip);
            var y = FungeFloat.PopFromStack(ip);
            var x = FungeFloat.PopFromStack(ip);
            return new FungeVector3D(new Vector3(x, y, z));
        }

        public void PushToStack(FungeIP ip)
        {
            new FungeFloat(_value.X).PushToStack(ip);
            new FungeFloat(_value.Y).PushToStack(ip);
            new FungeFloat(_value.Z).PushToStack(ip);
        }

        public static FungeVector3D operator +(FungeVector3D a, FungeVector3D b)
        {
            return new FungeVector3D(a._value + b._value);
        }

        public static FungeVector3D operator -(FungeVector3D a, FungeVector3D b)
        {
            return new FungeVector3D(a._value - b._value);
        }

        public static FungeVector3D operator *(FungeVector3D a, FungeVector3D b)
        {
            return new FungeVector3D(a._value * b._value);
        }

        public static FungeVector3D operator *(FungeVector3D a, FungeFloat b)
        {
            return new FungeVector3D(a._value * b);
        }

        public static FungeVector3D Cross(FungeVector3D a, FungeVector3D b)
        {
            return new FungeVector3D(Vector3.Cross(a._value, b._value));
        }

        public static FungeFloat Dot(FungeVector3D a, FungeVector3D b)
        {
            return new FungeFloat(Vector3.Dot(a._value, b._value));
        }

        public static FungeVector3D Normalize(FungeVector3D a)
        {
            return new FungeVector3D(Vector3.Normalize(a._value));
        }

        public FungeFloat Length => new(_value.Length());
        public FungeFloat X => new(_value.X);
        public FungeFloat Y => new(_value.Y);
        public FungeFloat Z => new(_value.Z);

        public Vector3 Value => _value;
    }
}