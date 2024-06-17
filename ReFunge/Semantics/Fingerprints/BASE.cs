using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("BASE")]
public class BASE
{
    [Instruction('B')]
    public static void OutputBinary(FungeIP ip, FungeInt n)
    {
        ip.Interpreter.WriteInteger(n, 2);
    }
    
    [Instruction('H')]
    public static void OutputHex(FungeIP ip, FungeInt n)
    {
        ip.Interpreter.WriteInteger(n, 16);
    }
    
    [Instruction('O')]
    public static void OutputOctal(FungeIP ip, FungeInt n)
    {
        ip.Interpreter.WriteInteger(n, 8);
    }
    
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
    
    [Instruction('I')]
    public static void InputInBase(FungeIP ip, FungeInt b)
    {
        try
        {
            ip.PushToStack(ip.Interpreter.ReadInteger(b));
        }
        catch (ArgumentException e)
        {
            throw new FungeReflectException(e);
        }
    }
}