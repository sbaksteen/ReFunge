using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

internal class MODU
{
    // MODU: Modulo arithmetic extension
    // Implements different modulo behaviors found in various languages

    [Instruction('M')] 
    public static FungeFunc SignedResultModulo =
        new FungeFunc<FungeInt, FungeInt, FungeInt>((_, a, b) =>
        {
            if (b == 0) return 0;
            return b < 0 ? -(a % b) : a % b;
        });

    [Instruction('R')] 
    public static FungeFunc HalfSignedModulo = CoreInstructions.Modulo;
    
    [Instruction('U')]
    public static FungeFunc UnsignedModulo =
        new FungeFunc<FungeInt, FungeInt, FungeInt>((_, a, b) => 
            b == 0 ? 0 : int.Abs(a % b));
}