using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.Core;

/// <summary>
///     ORTH: Instructions from the Orthogonal programming language. <br />
///     From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/ORTH.markdown)
/// </summary>
[Fingerprint("ORTH")]
public static class ORTH
{
    /// <summary>
    ///     Bitwise AND of two integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the bitwise AND operation.</returns>
    [Instruction('A')]
    public static FungeInt BitwiseAnd(FungeIP _, FungeInt a, FungeInt b)
    {
        return a & b;
    }

    /// <summary>
    ///     Bitwise OR of two integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the bitwise OR operation.</returns>
    [Instruction('O')]
    public static FungeInt BitwiseOr(FungeIP _, FungeInt a, FungeInt b)
    {
        return a | b;
    }

    /// <summary>
    ///     Bitwise XOR of two integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the bitwise XOR operation.</returns>
    [Instruction('E')]
    public static FungeInt BitwiseXor(FungeIP _, FungeInt a, FungeInt b)
    {
        return a ^ b;
    }

    /// <summary>
    ///     Set the X position of the IP. The delta is not affected.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="x">The new X position.</param>
    [Instruction('X')]
    public static void SetXPosition(FungeIP ip, FungeInt x)
    {
        ip.Position = ip.Position.SetCoordinate(0, x);
    }

    /// <summary>
    ///     Set the Y position of the IP. The delta is not affected.
    ///     Reflects if the IP is in 1D.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="y">The new Y position.</param>
    [Instruction('Y', 2)]
    public static void SetYPosition(FungeIP ip, FungeInt y)
    {
        ip.Position = ip.Position.SetCoordinate(1, y);
    }

    /// <summary>
    ///     Sets the X component of the IP's delta.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="dx">The new X delta.</param>
    [Instruction('V')]
    public static void SetXDelta(FungeIP ip, FungeInt dx)
    {
        ip.Delta = ip.Delta.SetCoordinate(0, dx);
    }

    /// <summary>
    ///     Sets the Y component of the IP's delta.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="dy">The new Y delta.</param>
    [Instruction('W', 2)]
    public static void SetYDelta(FungeIP ip, FungeInt dy)
    {
        ip.Delta = ip.Delta.SetCoordinate(1, dy);
    }

    /// <summary>
    ///     ORTH Get: Like <see cref="CoreInstructions.Get" />, but the vector is popped in reverse order from the stack.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="v">The vector to get from.</param>
    /// <returns>The value at the vector.</returns>
    [Instruction('G')]
    public static FungeInt OrthGet(FungeIP ip, FungeVector v)
    {
        return ip.Get(v.Reverse());
    }

    /// <summary>
    ///     ORTH Put: Like <see cref="CoreInstructions.Put" />, but the vector is popped in reverse order from the stack.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="i">The value to put.</param>
    /// <param name="v">The vector to put to.</param>
    [Instruction('P')]
    public static void OrthPut(FungeIP ip, FungeInt i, FungeVector v)
    {
        ip.Put(v.Reverse(), i);
    }

    /// <summary>
    ///     Output a string to the output stream.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="s">The string to output.</param>
    [Instruction('S')]
    public static void OutputString(FungeIP ip, FungeString s)
    {
        ip.Interpreter.WriteString(s);
    }

    /// <summary>
    ///     Skip the next instruction (like <see cref="CoreInstructions.Skip" />) if a popped value is zero.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="i">The value to check.</param>
    [Instruction('Z')]
    public static void SkipIfZero(FungeIP ip, FungeInt i)
    {
        if (i == 0) ip.Position += ip.Delta;
    }
}