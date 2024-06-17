using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.Core;

/// <summary>
///     MODU: Modulo arithmetic extension
///     Implements different modulo behaviors found in various languages
///     From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/MODU.markdown)
/// </summary>
[Fingerprint("MODU")]
internal static class MODU
{
    /// <summary>
    ///     Signed result modulo operation: The sign of the result is the sign of both operands multiplied. <br />
    ///     Returns zero if the second operand is zero.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the modulo operation.</returns>
    [Instruction('M')]
    public static FungeInt SignedResultModulo(FungeIP _, FungeInt a, FungeInt b)
    {
        return (int)b switch
        {
            0 => 0,
            < 0 => -(a % b),
            _ => a % b
        };
    }

    /// <summary>
    ///     Half-signed modulo operation: The sign of the result is the sign of the first operand. <br />
    ///     Returns zero if the second operand is zero. <br />
    ///     This is the default behavior in most programming languages, including C# and the core % instruction in ReFunge.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the modulo operation.</returns>
    [Instruction('R')]
    public static FungeInt HalfSignedModulo(FungeIP _, FungeInt a, FungeInt b)
    {
        return b == 0 ? 0 : a % b;
    }

    /// <summary>
    ///     Unsigned modulo operation: The result is always positive. <br />
    ///     Returns zero if the second operand is zero.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the modulo operation.</returns>
    [Instruction('U')]
    public static FungeInt UnsignedModulo(FungeIP _, FungeInt a, FungeInt b)
    {
        return int.Abs(HalfSignedModulo(_, a, b));
    }
}