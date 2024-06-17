namespace ReFunge.Semantics.Fingerprints.Core;

/// <summary>
///     ROMA: Roman numerals. <br />
///     Each letter that corresponds to a number in Roman numerals pushes its value to the stack. <br />
///     From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/ROMA.markdown)
/// </summary>
[Fingerprint("ROMA")]
internal static class ROMA
{
    /// <summary>
    ///     Pushes 1 to the stack.
    /// </summary>
    [Instruction('I')] public static FungeFunc Push1 = CoreInstructions.Push1;

    /// <summary>
    ///     Pushes 5 to the stack.
    /// </summary>
    [Instruction('V')] public static FungeFunc Push5 = CoreInstructions.Push5;

    /// <summary>
    ///     Pushes 10 to the stack.
    /// </summary>
    [Instruction('X')] public static FungeFunc Push10 = CoreInstructions.Push10;

    /// <summary>
    ///     Pushes 50 to the stack.
    /// </summary>
    [Instruction('L')] public static FungeFunc Push50 = CoreInstructions.PushNumber(50);

    /// <summary>
    ///     Pushes 100 to the stack.
    /// </summary>
    [Instruction('C')] public static FungeFunc Push100 = CoreInstructions.PushNumber(100);

    /// <summary>
    ///     Pushes 500 to the stack.
    /// </summary>
    [Instruction('D')] public static FungeFunc Push500 = CoreInstructions.PushNumber(500);

    /// <summary>
    ///     Pushes 1000 to the stack.
    /// </summary>
    [Instruction('M')] public static FungeFunc Push1000 = CoreInstructions.PushNumber(1000);
}