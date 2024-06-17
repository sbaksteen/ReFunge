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

    public void WriteInteger(int i)
    {
        Output.Write($"{i} ");
    }

    public bool EndOfInput()
    {
        return Input.Peek() == -1;
    }

    public int ReadCharacter()
    {
        return Input.Read();
    }

    public int ReadInteger()
    {
        if (Input.Peek() != '-' && (Input.Peek() < '0' || Input.Peek() > '9')) return 0;
        var c = Input.Read();
        if (c == '-') return -ReadInteger();
        var r = 0;
        while (c >= '0' && c <= '9')
        {
            r = r * 10 + c - '0';
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