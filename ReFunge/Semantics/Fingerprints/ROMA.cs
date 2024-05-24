namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("ROMA")]
internal static class ROMA
{
    // ROMA: Roman numerals.
    // Each letter that corresponds to a number in Roman numerals pushes its value to the stack.

    [Instruction('I')]
    public static FungeFunc Push1 = CoreInstructions.Push1;

    [Instruction('V')]
    public static FungeFunc Push5 = CoreInstructions.Push5;

    [Instruction('X')]
    public static FungeFunc Push10 = CoreInstructions.Push10;

    [Instruction('L')]
    public static FungeFunc Push50 = CoreInstructions.PushNumber(50);

    [Instruction('C')]
    public static FungeFunc Push100 = CoreInstructions.PushNumber(100);

    [Instruction('D')]
    public static FungeFunc Push500 = CoreInstructions.PushNumber(500);

    [Instruction('M')]
    public static FungeFunc Push1000 = CoreInstructions.PushNumber(1000);
}