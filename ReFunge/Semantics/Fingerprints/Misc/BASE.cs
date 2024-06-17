using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.Misc;

/// <summary>
///     BASE: Input/output in different number bases.
///     From RC/Funge-98.
/// </summary>
[Fingerprint("BASE")]
public class BASE
{
    /// <summary>
    ///     Output a number in binary. No prefix is added.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="n">The number to output.</param>
    [Instruction('B')]
    public static void OutputBinary(FungeIP ip, FungeInt n)
    {
        ip.Interpreter.WriteInteger(n, 2);
    }

    /// <summary>
    ///     Output a number in hexadecimal. No prefix is added.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="n">The number to output.</param>
    [Instruction('H')]
    public static void OutputHex(FungeIP ip, FungeInt n)
    {
        ip.Interpreter.WriteInteger(n, 16);
    }

    /// <summary>
    ///     Output a number in octal. No prefix is added.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="n">The number to output.</param>
    [Instruction('O')]
    public static void OutputOctal(FungeIP ip, FungeInt n)
    {
        ip.Interpreter.WriteInteger(n, 8);
    }

    /// <summary>
    ///     Output a number in the given base, from 2 to 62. The symbols 0-9, A-Z, and a-z are used for digits, in that order.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="n">The number to output.</param>
    /// <param name="b">The base to output the number in.</param>
    /// <exception cref="FungeReflectException">Thrown if the base is out of range.</exception>
    [Instruction('N')]
    public static void OutputInBase(FungeIP ip, FungeInt n, FungeInt b)
    {
        try
        {
            ip.Interpreter.WriteInteger(n, b);
        }
        catch (ArgumentException e)
        {
            throw new FungeReflectException(e);
        }
    }

    /// <summary>
    ///     Read a number in a specified base from the input stream. The base can be from 2 to 62, using the symbols 0-9, A-Z,
    ///     and a-z for digits.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="b">The base to read the number in.</param>
    /// <returns>The number read from the input stream.</returns>
    /// <exception cref="FungeReflectException">
    ///     Thrown if the input is not a valid number in the given base,
    ///     or if the end of the input stream has been reached.
    /// </exception>
    [Instruction('I')]
    public static FungeInt InputInBase(FungeIP ip, FungeInt b)
    {
        if (ip.Interpreter.EndOfInput()) throw new FungeReflectException(new InvalidOperationException("End of input"));
        try
        {
            return ip.Interpreter.ReadInteger(b);
        }
        catch (ArgumentException e)
        {
            throw new FungeReflectException(e);
        }
    }
}