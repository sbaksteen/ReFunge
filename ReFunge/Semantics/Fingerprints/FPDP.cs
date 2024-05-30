using System.Globalization;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("FPDP")]
public static class FPDP
{
    // FPDP: Floating-point (double precision) extension
    // Implements double-precision floating-point arithmetic
    // Allows doubles to be stored on the stack as two stack cells representing the bits of the double
    
    public readonly record struct FungeDouble(double Value) : IFungeValue<FungeDouble>
    {
        public static implicit operator FungeDouble(double value) => new(value);

        public static implicit operator double(FungeDouble me) => me.Value;

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

        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }

    [Instruction('A')]
    public static FungeDouble Add(FungeIP _, FungeDouble a, FungeDouble b) => a + b;
    
    [Instruction('B')]
    public static FungeDouble Sine(FungeIP _, FungeDouble a) => double.Sin(a);
    
    [Instruction('C')]
    public static FungeDouble Cosine(FungeIP _, FungeDouble a) => double.Cos(a);
    
    [Instruction('D')]
    public static FungeDouble Divide(FungeIP _, FungeDouble a, FungeDouble b) => a / b;
        
    [Instruction('E')]
    public static FungeDouble ArcSine(FungeIP _, FungeDouble a) => double.Asin(a);
    
    [Instruction('F')]
    public static FungeDouble IntToDouble(FungeIP _, FungeInt a) => (double)a;
    
    [Instruction('G')]
    public static FungeDouble ArcTangent(FungeIP _, FungeDouble a) => double.Atan(a);
    
    [Instruction('H')]
    public static FungeDouble ArcCosine(FungeIP _, FungeDouble a) => double.Acos(a);
    
    [Instruction('I')]
    public static FungeInt DoubleToInt(FungeIP _, FungeDouble a) => (int)a;
    
    [Instruction('K')]
    public static FungeDouble NaturalLog(FungeIP _, FungeDouble a) => double.Log(a);
    
    [Instruction('L')]
    public static FungeDouble LogBase10(FungeIP _, FungeDouble a) => double.Log10(a);
    
    [Instruction('M')]
    public static FungeDouble Multiply(FungeIP _, FungeDouble a, FungeDouble b) => a * b;
    
    [Instruction('N')]
    public static FungeDouble Negate(FungeIP _, FungeDouble a) => -a;
    
    [Instruction('P')]
    public static void Print(FungeIP ip, FungeDouble a)
    {
        ip.Interpreter.WriteString(a + " ");
    }
    
    [Instruction('Q')]
    public static FungeDouble SquareRoot(FungeIP _, FungeDouble a) => double.Sqrt(a);
    
    [Instruction('R')]
    public static FungeDouble TryParse(FungeIP ip, FungeString str)
    {
        if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        throw new FungeReflectException(new ArgumentException("Invalid double value"));
    }
    
    [Instruction('S')]
    public static FungeDouble Subtract(FungeIP _, FungeDouble a, FungeDouble b) => a - b;
    
    [Instruction('T')]
    public static FungeDouble Tangent(FungeIP _, FungeDouble a) => double.Tan(a);
    
    [Instruction('V')]
    public static FungeDouble AbsoluteValue(FungeIP _, FungeDouble a) => double.Abs(a);
    
    [Instruction('X')]
    public static FungeDouble NaturalExponent(FungeIP _, FungeDouble a) => double.Exp(a);
    
    [Instruction('Y')]
    public static FungeDouble Power(FungeIP _, FungeDouble a, FungeDouble b) => double.Pow(a, b);

}