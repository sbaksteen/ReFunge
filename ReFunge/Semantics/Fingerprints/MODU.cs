using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("MODU")]
internal static class MODU
{
    // MODU: Modulo arithmetic extension
    // Implements different modulo behaviors found in various languages
    // From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/MODU.markdown)

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

    [Instruction('R')]
    public static FungeInt HalfSignedModulo(FungeIP _, FungeInt a, FungeInt b)
    {
        return b == 0 ? 0 : a % b;
    }

    [Instruction('U')]
    public static FungeInt UnsignedModulo(FungeIP _, FungeInt a, FungeInt b)
    {
        return int.Abs(HalfSignedModulo(_, a, b));
    }
}