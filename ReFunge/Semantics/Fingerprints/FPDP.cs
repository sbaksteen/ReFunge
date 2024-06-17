using System.Globalization;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("FPDP")]
public static class FPDP
{
    [Instruction('A')]
    public static FungeDouble Add(FungeIP _, FungeDouble a, FungeDouble b)
    {
        return a + b;
    }

    [Instruction('B')]
    public static FungeDouble Sine(FungeIP _, FungeDouble a)
    {
        return double.Sin(a);
    }

    [Instruction('C')]
    public static FungeDouble Cosine(FungeIP _, FungeDouble a)
    {
        return double.Cos(a);
    }

    [Instruction('D')]
    public static FungeDouble Divide(FungeIP _, FungeDouble a, FungeDouble b)
    {
        return a / b;
    }

    [Instruction('E')]
    public static FungeDouble ArcSine(FungeIP _, FungeDouble a)
    {
        return double.Asin(a);
    }

    [Instruction('F')]
    public static FungeDouble IntToDouble(FungeIP _, FungeInt a)
    {
        return (double)a;
    }

    [Instruction('G')]
    public static FungeDouble ArcTangent(FungeIP _, FungeDouble a)
    {
        return double.Atan(a);
    }

    [Instruction('H')]
    public static FungeDouble ArcCosine(FungeIP _, FungeDouble a)
    {
        return double.Acos(a);
    }

    [Instruction('I')]
    public static FungeInt DoubleToInt(FungeIP _, FungeDouble a)
    {
        return (int)a;
    }

    [Instruction('K')]
    public static FungeDouble NaturalLog(FungeIP _, FungeDouble a)
    {
        return double.Log(a);
    }

    [Instruction('L')]
    public static FungeDouble LogBase10(FungeIP _, FungeDouble a)
    {
        return double.Log10(a);
    }

    [Instruction('M')]
    public static FungeDouble Multiply(FungeIP _, FungeDouble a, FungeDouble b)
    {
        return a * b;
    }

    [Instruction('N')]
    public static FungeDouble Negate(FungeIP _, FungeDouble a)
    {
        return -a;
    }

    [Instruction('P')]
    public static void Print(FungeIP ip, FungeDouble a)
    {
        ip.Interpreter.WriteString(a + " ");
    }

    [Instruction('Q')]
    public static FungeDouble SquareRoot(FungeIP _, FungeDouble a)
    {
        return double.Sqrt(a);
    }

    [Instruction('R')]
    public static FungeDouble TryParse(FungeIP ip, FungeString str)
    {
        if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)) return result;
        throw new FungeReflectException(new ArgumentException("Invalid double value"));
    }

    [Instruction('S')]
    public static FungeDouble Subtract(FungeIP _, FungeDouble a, FungeDouble b)
    {
        return a - b;
    }

    [Instruction('T')]
    public static FungeDouble Tangent(FungeIP _, FungeDouble a)
    {
        return double.Tan(a);
    }

    [Instruction('V')]
    public static FungeDouble AbsoluteValue(FungeIP _, FungeDouble a)
    {
        return double.Abs(a);
    }

    [Instruction('X')]
    public static FungeDouble NaturalExponent(FungeIP _, FungeDouble a)
    {
        return double.Exp(a);
    }

    [Instruction('Y')]
    public static FungeDouble Power(FungeIP _, FungeDouble a, FungeDouble b)
    {
        return double.Pow(a, b);
    }
    // FPDP: Floating-point (double precision) extension
    // Implements double-precision floating-point arithmetic
    // Allows doubles to be stored on the stack as two stack cells representing the bits of the double

    public readonly record struct FungeDouble(double Value) : IFungeValue<FungeDouble>
    {
        public static FungeDouble PopFromStack(FungeIP ip)
        {
            var longVal = LONG.FungeLong.PopFromStack(ip);
            return new FungeDouble(BitConverter.Int64BitsToDouble(longVal));
        }

        public void PushToStack(FungeIP ip)
        {
            var bits = BitConverter.DoubleToInt64Bits(Value);
            new LONG.FungeLong(bits).PushToStack(ip);
        }

        public static implicit operator FungeDouble(double value)
        {
            return new FungeDouble(value);
        }

        public static implicit operator double(FungeDouble me)
        {
            return me.Value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}