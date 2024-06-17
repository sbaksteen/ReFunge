using ReFunge.Data;
using ReFunge.Data.Values;
using ReFunge.Semantics;

namespace ReFunge;

public class Interpreter
{
    internal TextWriter Error;

    internal TextReader Input;

    internal int IPID;
    public List<FungeIP> IPList = [];

    // New IPs should always be inserted before the parent
    // If the new IP does not have a parent, insert it at the start
    internal List<(FungeIP ip, FungeIP? parent)> NewIPList = [];

    internal TextWriter Output;

    public FungeSpace PrimarySpace;

    internal bool Quit;

    internal int ReturnValue = 0;

    public long Tick;

    public Interpreter(int dim = 2, TextReader? input = null, TextWriter? output = null, TextWriter? errorOutput = null)
    {
        input ??= Console.In;
        output ??= Console.Out;
        errorOutput ??= Console.Error;
        InstructionRegistry = new InstructionRegistry(this);
        Output = output;
        Input = input;
        Error = errorOutput;
        PrimarySpace = new FungeSpace(dim);
        IPList.Add(new FungeIP(IPID++, PrimarySpace, this));
    }

    public InstructionRegistry InstructionRegistry { get; }

    public void AddNewIP(FungeIP ip, FungeIP? parent = null)
    {
        NewIPList.Add((ip, parent));
    }

    public void WriteError(string s)
    {
        Error.Write(s);
    }

    public void WriteCharacter(char c)
    {
        Output.Write(c);
    }

    private static char IntToDigit(int i)
    {
        if (i < 0 || i > 62) throw new ArgumentException("Invalid digit", nameof(i));
        if (i < 10) return (char)('0' + i);
        if (i < 36) return (char)('A' + i - 10);
        return (char)('a' + i - 36);
    }

    public void WriteInteger(int i, int b = 10)
    {
        if (b < 2 || b > 62) throw new ArgumentException("Invalid base", nameof(b));
        if (i < 0)
        {
            Output.Write('-');
            i = -i;
        }
        
        var stack = new Stack<char>();
        do
        {
            stack.Push(IntToDigit(i % b));
            i /= b;
        } while (i > 0);

        foreach (var c in stack)
        {
            Output.Write(c);
        }
        Output.Write(' ');
    }

    public bool EndOfInput()
    {
        return Input.Peek() == -1;
    }

    public int ReadCharacter()
    {
        return Input.Read();
    }

    private static bool IsDigit(int c, int b)
    {
        var fromZero = c >= '0' && c <= '0' + Math.Min(b - 1, 9);
        var fromA = b > 10 && c >= 'A' && c <= 'A' + b - 11;
        var froma = b > 36 && c >= 'a' && c <= 'a' + b - 37;
        return fromZero || fromA || froma;
    }
    
    private static int DigitValue(int c)
    {
        switch (c)
        {
            case >= '0' and <= '9':
                return c - '0';
            case >= 'A' and <= 'Z':
                return c - 'A' + 10;
            case >= 'a' and <= 'z':
                return c - 'a' + 36;
            default:
                // Should never happen
                throw new ArgumentException("Invalid digit", nameof(c));
        }
    }

    public int ReadInteger(int b = 10)
    {
        if (b < 2 || b > 62) throw new ArgumentException("Invalid base", nameof(b));
        if (Input.Peek() != '-' && !IsDigit(Input.Peek(), b)) return 0;
        var c = Input.Read();
        if (c == '-') return -ReadInteger();
        var r = 0;
        while (IsDigit(c, b))
        {
            r = r * b + DigitValue(c);
            c = Input.Read();
        }

        return r;
    }

    public void DoStep()
    {
        if (Quit) return;
        List<FungeIP> toRemove = [];
        foreach (var ip in IPList)
        {
            ip.Step();
            if (ip.RequestQuit)
            {
                Quit = true;
                return;
            }

            if (!ip.Alive) toRemove.Add(ip);
        }

        foreach (var (ip, parent) in NewIPList)
        {
            if (parent is null || !IPList.Contains(parent))
            {
                IPList.Insert(0, ip);
                continue;
            }

            IPList.Insert(IPList.IndexOf(parent), ip);
        }

        NewIPList.Clear();
        foreach (var ip in toRemove) IPList.Remove(ip);
        Tick++;
    }

    public int Run()
    {
        while (!Quit && IPList.Count > 0) DoStep();
        return ReturnValue;
    }

    public void Load(string filename)
    {
        PrimarySpace.LoadFile(new FungeVector(), filename);
    }

    public void WriteString(string str)
    {
        Output.Write(str);
    }
}