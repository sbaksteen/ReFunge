using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("ORTH")]
public static class ORTH
{
    // ORTH: Instructions from the Orthogonal programming language.
    // From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/ORTH.markdown)
    
    [Instruction('A')]
    public static FungeInt BitwiseAnd(FungeIP _, FungeInt a, FungeInt b)
    {
        return a & b;
    }

    [Instruction('O')]
    public static FungeInt BitwiseOr(FungeIP _, FungeInt a, FungeInt b)
    {
        return a | b;
    }

    [Instruction('E')]
    public static FungeInt BitwiseXor(FungeIP _, FungeInt a, FungeInt b)
    {
        return a ^ b;
    }

    [Instruction('X')]
    public static void SetXPosition(FungeIP ip, FungeInt x)
    {
        ip.Position = ip.Position.SetCoordinate(0, x);
    }

    [Instruction('Y')]
    public static void SetYPosition(FungeIP ip, FungeInt y)
    {
        if (ip.Dim < 2)
        {
            throw new FungeReflectException();
        }

        ip.Position = ip.Position.SetCoordinate(1, y);
    }

    [Instruction('V')]
    public static void SetXDelta(FungeIP ip, FungeInt dx)
    {
        ip.Delta = ip.Delta.SetCoordinate(0, dx);
    }

    [Instruction('W')]
    public static void SetYDelta(FungeIP ip, FungeInt dy)
    {
        if (ip.Dim < 2)
        {
            throw new FungeReflectException();
        }

        ip.Delta = ip.Delta.SetCoordinate(1, dy);
    }

    [Instruction('G')]
    public static FungeInt OrthGet(FungeIP ip, FungeVector v)
    {
        return ip.Get(v.Reverse());
    }

    [Instruction('P')]
    public static void OrthPut(FungeIP ip, FungeInt i, FungeVector v)
    {
        ip.Put(v.Reverse(), i);
    }

    [Instruction('S')]
    public static void OutputString(FungeIP ip, FungeString s)
    {
        ip.Interpreter.WriteString(s);
    }

    [Instruction('Z')]
    public static void SkipIfZero(FungeIP ip, FungeInt i)
    {
        if (i == 0) ip.Position += ip.Delta;
    }
}