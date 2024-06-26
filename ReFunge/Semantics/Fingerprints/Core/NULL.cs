﻿namespace ReFunge.Semantics.Fingerprints.Core;

/// <summary>
///     NULL: The null fingerprint. Clears fingerprint functions. <br />
///     Everything reflects, but explicitly so. <br />
///     From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/NULL.markdown)
/// </summary>
[Fingerprint("NULL")]
internal static class NULL
{
    /// <summary>
    ///     Reflects the IP. Associated with all letters.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
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
    // This is rather silly.
    public static void Reflect(FungeIP ip)
    {
        ip.Reflect();
    }
}