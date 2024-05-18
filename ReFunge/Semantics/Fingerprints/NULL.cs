namespace ReFunge.Semantics.Fingerprints;

internal class NULL
{
    // NULL: The null fingerprint. Clears fingerprint functions.
    // Everything reflects, but explicitly so.
    // This is rather silly.
    [Instruction('A')]
    [Instruction('B')]
    [Instruction('C')]
    [Instruction('D')]
    [Instruction('E')]
    [Instruction('F')]
    [Instruction('G')]
    [Instruction('H')]
    [Instruction('I')]
    [Instruction('J')]
    [Instruction('K')]
    [Instruction('L')]
    [Instruction('M')]
    [Instruction('N')]
    [Instruction('O')]
    [Instruction('P')]
    [Instruction('Q')]
    [Instruction('R')]
    [Instruction('S')]
    [Instruction('T')]
    [Instruction('U')]
    [Instruction('V')]
    [Instruction('W')]
    [Instruction('X')]
    [Instruction('Y')]
    [Instruction('Z')]
    public static FungeFunc Reflect = CoreInstructions.Reflect;
}