using ReFunge.Data;
using ReFunge.Data.Values;
using ReFunge.Semantics;
using ReFunge.Semantics.Fingerprints;

namespace ReFunge;

/// <summary>
///     Represents a Funge interpreter. This class is responsible for managing the list of IPs, console I/O, and
///     making sure the IPs execute instructions in the correct order.
/// </summary>
public class Interpreter
{
    public TextWriter Error { get; set; }
    public TextWriter Output { get; set; }

    public TextReader Input { get; set; }

    public Dictionary<FungeInt, InstancedFingerprint> Fingerprints { get; } = new();

    /// <summary>
    ///     The ID of the next IP to be created. This is used to assign unique IDs to each IP.
    /// </summary>
    public int IPID;

    /// <summary>
    ///     The list of IPs currently active in the interpreter. This list is updated every tick.
    /// </summary>
    public List<FungeIP> IPList = [];

    // New IPs should always be inserted before the parent
    // If the new IP does not have a parent, insert it at the start
    private readonly List<(FungeIP ip, FungeIP? parent)> NewIPList = [];


    /// <summary>
    ///     The primary Funge space for the interpreter. This is where the main program is loaded and executed.
    /// </summary>
    public FungeSpace PrimarySpace;

    internal bool Quit;

    internal int ReturnValue = 0;

    /// <summary>
    ///     The current tick of the interpreter. This is incremented every time <see cref="DoStep" /> is called.
    /// </summary>
    public long Tick;

    /// <summary>
    ///     Create a new interpreter with the given dimensions and I/O streams.
    ///     A single IP is created in the primary space at the origin, moving right.
    /// </summary>
    /// <param name="dim">The number of dimensions of the Funge space.</param>
    /// <param name="input">The input stream for the interpreter. If null, defaults to <see cref="Console.In" />.</param>
    /// <param name="output">The output stream for the interpreter. If null, defaults to <see cref="Console.Out" />.</param>
    /// <param name="errorOutput">
    ///     The error output stream for the interpreter. If null, defaults to
    ///     <see cref="Console.Error" />.
    /// </param>
    public Interpreter(int dim = 2, TextReader? input = null, TextWriter? output = null, TextWriter? errorOutput = null, string? inFile = null)
    {
        input ??= Console.In;
        output ??= Console.Out;
        errorOutput ??= Console.Error;
        InstructionRegistry = new InstructionRegistry(this);
        Output = output;
        Input = input;
        Error = errorOutput;
        PrimarySpace = new FungeSpace(dim);
        if (inFile is not null)
        {
            PrimarySpace.LoadFile(new FungeVector(), inFile);
        }
        IPList.Add(new FungeIP(IPID++, PrimarySpace, this));
        foreach (var c in InstructionRegistry.InterpreterFingerprints)
        {
            Fingerprints[c] = InstructionRegistry.NewInstance(c, this);
        }
    }

    /// <summary>
    ///     The instruction registry for the interpreter. This is used to look up instructions by their character codes,
    ///     and manage fingerprints.
    /// </summary>
    public InstructionRegistry InstructionRegistry { get; }

    /// <summary>
    ///     Add a new IP to the interpreter. This IP will be added to the list of IPs at the end of the current tick.
    /// </summary>
    /// <param name="ip">The IP to add.</param>
    /// <param name="parent">
    ///     The parent IP of the new IP, if any. If null, the new IP will be added to the start of the list.
    ///     Otherwise, the new IP is added to the list just before its parent.
    /// </param>
    public void AddNewIP(FungeIP ip, FungeIP? parent = null)
    {
        NewIPList.Add((ip, parent));
    }

    /// <summary>
    ///     Write a string to the error output stream.
    /// </summary>
    /// <param name="s">The string to write.</param>
    public void WriteError(string s)
    {
        Error.Write(s);
    }

    /// <summary>
    ///     Write a character to the output stream.
    /// </summary>
    /// <param name="c">The character to write.</param>
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

    /// <summary>
    ///     Write an integer to the output stream in the given base. The integer is written as a string of digits in the
    ///     given base, followed by a space. Base must be between 2 and 62, inclusive. The symbols used for digits are
    ///     0 through 9, A through Z, and a through z, in that order.
    /// </summary>
    /// <param name="i">The integer to write.</param>
    /// <param name="b">The base to write the integer in. Must be between 2 and 62, inclusive.</param>
    /// <exception cref="ArgumentException">Thrown if the base is out of range.</exception>
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

        foreach (var c in stack) Output.Write(c);
        Output.Write(' ');
    }

    /// <summary>
    ///     Check if the input stream has reached the end of the input.
    /// </summary>
    /// <returns>True if the input stream has reached the end of the input, false otherwise.</returns>
    public bool EndOfInput()
    {
        return Input.Peek() == -1;
    }

    /// <summary>
    ///     Read a character from the input stream.
    /// </summary>
    /// <returns>The character read, or -1 if the end of the input stream has been reached.</returns>
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

    /// <summary>
    ///     Read an integer from the input stream in the given base. The integer is read as a string of digits in the given
    ///     base, followed by a space. Base must be between 2 and 62, inclusive. The symbols used for digits are 0 through 9,
    ///     A through Z, and a through z, in that order.
    /// </summary>
    /// <param name="b">The base to read the integer in. Must be between 2 and 62, inclusive.</param>
    /// <returns>The integer read.</returns>
    /// <exception cref="ArgumentException">Thrown if the base is out of range.</exception>
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

    /// <summary>
    ///     Execute one step of the interpreter. Each IP in the list executes one instruction and moves to its next position,
    ///     using the <see cref="FungeIP.Step" /> method. If an IP requests to quit, the interpreter stops executing
    ///     immediately.
    ///     Any new IPs created during the step are added to the list of IPs after all IPs have executed their instructions.
    ///     After this, any IPs that have died are removed from the list.
    /// </summary>
    public void DoStep()
    {
        if (Quit) return;
        foreach (var f in Fingerprints.Values)
        {
            f.EachTick(Tick);
        }
        List<FungeIP> toRemove = [];
        foreach (var ip in IPList)
        {
            if (ip.Frozen) continue;
            if (!ip.Alive)
            {
                toRemove.Add(ip);
                continue;
            }
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

    /// <summary>
    ///     Run the interpreter until an IP requests to quit, or all IPs have died. If an IP has set the return value, this
    ///     method returns that value. Otherwise, it returns 0.
    /// </summary>
    /// <returns>The return value of the interpreter.</returns>
    public int Run()
    {
        while (!Quit && IPList.Count > 0) DoStep();
        return ReturnValue;
    }

    /// <summary>
    ///     Load a program from a file into the primary Funge space at the origin.
    /// </summary>
    /// <param name="filename">The name of the file to load.</param>
    public void Load(string filename)
    {
        PrimarySpace.LoadFile(new FungeVector(), filename);
    }

    /// <summary>
    ///     Write a string to the output stream.
    /// </summary>
    /// <param name="str">The string to write.</param>
    public void WriteString(string str)
    {
        Output.Write(str);
    }
}