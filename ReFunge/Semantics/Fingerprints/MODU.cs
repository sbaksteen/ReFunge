using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("MODU")]
internal static class MODU
{
    // MODU: Modulo arithmetic extension
    // Implements different modulo behaviors found in various languages

    [Instruction('M')] 
    public static FungeInt SignedResultModulo(FungeIP _, FungeInt a, FungeInt b) =>
        (int)b switch
        {
            0 => 0,
            < 0 => -(a % b),
            _ => a % b
        };

    [Instruction('R')] 
    public static FungeInt HalfSignedModulo(FungeIP _, FungeInt a, FungeInt b) => b == 0 ? 0 : a % b;
    
    [Instruction('U')]
    public static FungeInt UnsignedModulo(FungeIP _, FungeInt a, FungeInt b) => int.Abs(HalfSignedModulo(_, a, b));
}