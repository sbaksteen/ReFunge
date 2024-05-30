using System.Globalization;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("FPSP")]
public static class FPSP
{
    // FPSP: Floating-point (single precision) extension
    // Implements single-precision floating-point arithmetic
    // Allows floats to be stored on the stack as integers representing the bits of the float
    
    public readonly record struct FungeFloat(float Value) : IFungeValue<FungeFloat>
    {
        public static implicit operator FungeFloat(float value) => new(value);

        public static implicit operator float(FungeFloat me) => me.Value;

        public static FungeFloat PopFromStack(FungeIP ip)
        {
            var value = ip.PopFromStack();
            return new FungeFloat(BitConverter.Int32BitsToSingle(value));
        }

        public void PushToStack(FungeIP ip)
        {
            ip.PushToStack(BitConverter.SingleToInt32Bits(Value));
        }

        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }
    
    [Instruction('A')]
    public static FungeFloat Add(FungeIP _, FungeFloat a, FungeFloat b) => a + b;
    
    [Instruction('B')]
    public static FungeFloat Sine(FungeIP _, FungeFloat a) => float.Sin(a);
    
    [Instruction('C')]
    public static FungeFloat Cosine(FungeIP _, FungeFloat a) => float.Cos(a);
    
    [Instruction('D')]
    public static FungeFloat Divide(FungeIP _, FungeFloat a, FungeFloat b) => a / b;
    
    [Instruction('E')]
    public static FungeFloat ArcSine(FungeIP _, FungeFloat a) => float.Asin(a);
    
    [Instruction('F')]
    public static FungeFloat IntToFloat(FungeIP _, FungeInt a) => (float)a;
    
    [Instruction('G')]
    public static FungeFloat ArcTangent(FungeIP _, FungeFloat a) => float.Atan(a);
    
    [Instruction('H')]
    public static FungeFloat ArcCosine(FungeIP _, FungeFloat a) => float.Acos(a);
    
    [Instruction('I')]
    public static FungeInt FloatToInt(FungeIP _, FungeFloat a) => (int)a;
    
    [Instruction('K')]
    public static FungeFloat NaturalLog(FungeIP _, FungeFloat a) => float.Log(a);
    
    [Instruction('L')]
    public static FungeFloat LogBase10(FungeIP _, FungeFloat a) => float.Log10(a);
    
    [Instruction('M')]
    public static FungeFloat Multiply(FungeIP _, FungeFloat a, FungeFloat b) => a * b;
    
    [Instruction('N')]
    public static FungeFloat Negate(FungeIP _, FungeFloat a) => -a;

    [Instruction('P')]
    public static void Print(FungeIP ip, FungeFloat a) => ip.Interpreter.WriteString(a + " ");
    
    [Instruction('Q')]
    public static FungeFloat SquareRoot(FungeIP _, FungeFloat a) => float.Sqrt(a);
    
    [Instruction('R')]
    public static FungeFloat TryParse(FungeIP ip, FungeString str)
    {
        if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }
        throw new FungeReflectException(new ArgumentException("Invalid double value"));
    }
    
    [Instruction('S')]
    public static FungeFloat Subtract(FungeIP _, FungeFloat a, FungeFloat b) => a - b;
    
    [Instruction('T')]
    public static FungeFloat Tangent(FungeIP _, FungeFloat a) => float.Tan(a);
    
    [Instruction('V')]
    public static FungeFloat AbsoluteValue(FungeIP _, FungeFloat a) => float.Abs(a);
    
    [Instruction('X')]
    public static FungeFloat NaturalExponent(FungeIP _, FungeFloat a) => float.Exp(a);
    
    [Instruction('Y')]
    public static FungeFloat Power(FungeIP _, FungeFloat a, FungeFloat b) => float.Pow(a, b);
}