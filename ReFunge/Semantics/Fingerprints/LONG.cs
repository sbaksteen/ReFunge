using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("LONG")]
public static class LONG
{
    // LONG: Long integers
    // Implements long integers, taking up two stack cells.
    
    public readonly record struct FungeLong(long Value) : IFungeValue<FungeLong>
    {
        public static implicit operator FungeLong(long value) => new(value);

        public static implicit operator long(FungeLong me) => me.Value;

        public static FungeLong PopFromStack(FungeIP ip)
        {
            var intLow = (uint)(int)ip.PopFromStack();
            var intHigh = (uint)(int)ip.PopFromStack();
            return new FungeLong((long)intHigh << 32 | intLow);
        }

        public void PushToStack(FungeIP ip)
        {
            ip.PushToStack((int)(Value >> 32));
            ip.PushToStack((int)(Value & 0xFFFFFFFF));
        }

        public override string ToString() => Value.ToString();
    }
    
    [Instruction('A')]
    public static FungeLong Add(FungeIP _, FungeLong a, FungeLong b) => a + b;
    
    [Instruction('B')]
    public static FungeLong AbsoluteValue(FungeIP _, FungeLong a) => long.Abs(a);
    
    [Instruction('D')]
    public static FungeLong Divide(FungeIP _, FungeLong a, FungeLong b) => a / b;
    
    [Instruction('E')]
    public static FungeLong IntToLong(FungeIP _, FungeInt a) => (long)a;

    [Instruction('L')]
    public static FungeLong LeftShift(FungeIP _, FungeLong a, FungeInt b)
    {
        if (b < 0)
        {
            throw new FungeReflectException(new ArgumentOutOfRangeException(nameof(b), "Shift amount must be non-negative"));
        }
        return a << b;
    }
    
    [Instruction('M')]
    public static FungeLong Multiply(FungeIP _, FungeLong a, FungeLong b) => a * b;
    
    [Instruction('N')]
    public static FungeLong Negate(FungeIP _, FungeLong a) => -a;

    [Instruction('O')]
    public static FungeLong Modulo(FungeIP _, FungeLong a, FungeLong b)
    {
        if (b == 0) return 0;
        return a % b;
    }

    [Instruction('P')]
    public static void Print(FungeIP ip, FungeLong a)
    {
        ip.Interpreter.WriteString(a + " ");
    }
    
    [Instruction('R')]
    public static FungeLong RightShift(FungeIP _, FungeLong a, FungeInt b)
    {
        if (b < 0)
        {
            throw new FungeReflectException(new ArgumentOutOfRangeException(nameof(b), "Shift amount must be non-negative"));
        }
        return a >> b;
    }
    
    [Instruction('S')]
    public static FungeLong Subtract(FungeIP _, FungeLong a, FungeLong b) => a - b;
    
    [Instruction('Z')]
    public static FungeLong TryParse(FungeIP _, FungeString str)
    {
        if (long.TryParse(str, out var result))
        {
            return result;
        }
        throw new FungeReflectException(new ArgumentException("Invalid long integer format"));
    }
}