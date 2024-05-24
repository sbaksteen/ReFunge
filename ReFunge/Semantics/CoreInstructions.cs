using ReFunge.Data;
using ReFunge.Data.Values;

namespace ReFunge.Semantics;

internal static class CoreInstructions
{

    [Instruction('+')]
    public static FungeInt Add(FungeIP _, FungeInt a, FungeInt b) => a + b;

    [Instruction('-')]
    public static FungeInt Subtract(FungeIP _, FungeInt a, FungeInt b) => a - b;

    [Instruction('*')]
    public static FungeInt Multiply(FungeIP _, FungeInt a, FungeInt b) => a * b;

    [Instruction('/')]
    public static FungeInt Divide(FungeIP _, FungeInt a, FungeInt b) => b == 0 ? 0 : a / b;

    [Instruction('%')]
    public static FungeInt Modulo(FungeIP _, FungeInt a, FungeInt b) => b == 0 ? 0 : a % b;

    [Instruction('`')]
    public static FungeInt GreaterThan(FungeIP _, FungeInt a, FungeInt b) => a > b ? 1 : 0;

    [Instruction('!')]
    public static FungeInt LogicalNot(FungeIP _, FungeInt v) => v == 0 ? 1 : 0;

    [Instruction('>')]
    public static void GoRight(FungeIP ip)
    {
        if (ip.HoverMode)
        {
            ip.Delta += FungeVector.Right;
        }
        else
        {
            ip.Delta = FungeVector.Right;
        }
    }

    [Instruction('<')]
    public static void GoLeft(FungeIP ip)
    {
        if (ip.HoverMode)
        {
            ip.Delta += FungeVector.Left;
        }
        else
        {
            ip.Delta = FungeVector.Left;
        }
    }

    [Instruction('^')]
    public static void GoUp(FungeIP ip)
    {
        if (ip.Dim < 2) throw new FungeReflectException();
        if (ip.HoverMode)
        {
            ip.Delta += FungeVector.Up;
        }
        else
        {
            ip.Delta = FungeVector.Up;
        }
    }

    [Instruction('v')]
    public static void GoDown(FungeIP ip)
    {
        if (ip.Dim < 2) throw new FungeReflectException();
        if (ip.HoverMode)
        {
            ip.Delta += FungeVector.Down;
        }
        else
        {
            ip.Delta = FungeVector.Down;
        }
    }

    [Instruction('h')]
    public static void GoForwards(FungeIP ip)
    {
        if (ip.Dim < 3) throw new FungeReflectException();
        if (ip.HoverMode)
        {
            ip.Delta += FungeVector.Forwards;
        }
        else
        {
            ip.Delta = FungeVector.Forwards;
        }
    }

    [Instruction('l')]
    public static void GoBackwards(FungeIP ip)
    {
        if (ip.Dim < 3) throw new FungeReflectException();
        if (ip.HoverMode)
        {
            ip.Delta += FungeVector.Backwards;
        }
        else
        {
            ip.Delta = FungeVector.Backwards;
        }
    }

    [Instruction('_')]
    public static void DecideHorizontal(FungeIP ip, FungeInt v)
    {
        if (v == 0)
            GoRight(ip);
        else
            GoLeft(ip);
    }

    [Instruction('|')]
    public static void DecideVertical(FungeIP ip, FungeInt v)
    {
        if (ip.Dim < 2) throw new FungeReflectException();

        if (v == 0)
            GoDown(ip);
        else
            GoUp(ip);
    }

    [Instruction('m')]
    public static void DecideForwards(FungeIP ip, FungeInt v)
    {
        if (ip.Dim < 3) throw new FungeReflectException();

        if (v == 0)
            GoBackwards(ip);
        else
            GoForwards(ip);
    }

    [Instruction('?')]
    public static void GoRandom(FungeIP ip)
    {
        ip.Delta = FungeVector.Cardinal(Random.Shared.Next(ip.Dim), Random.Shared.Next(2) * 2 - 1);
    }

    [Instruction('r')]
    public static void Reflect(FungeIP ip)
    {
        ip.Reflect();
    }

    [Instruction(']')]
    public static void TurnRight(FungeIP ip)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = '[';
        switch (ip.Dim)
        {
            case < 2:
                throw new FungeReflectException();
            case <= 2:
                ip.Delta = new FungeVector(-ip.Delta[1], ip.Delta[0]);
                return;
        }

        var newDelta = new int[ip.Dim];
        for (var i = 0; i < ip.Dim; i++)
        {
            newDelta[i] = ip.Delta[i];
        }

        newDelta[0] = -ip.Delta[1];
        newDelta[1] = ip.Delta[0];
        ip.Delta = new FungeVector(newDelta);
    }

    [Instruction('[')]
    public static void TurnLeft(FungeIP ip)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = ']';
        switch (ip.Dim)
        {
            case < 2:
                throw new FungeReflectException();
            case <= 2:
                ip.Delta = new FungeVector(ip.Delta[1], -ip.Delta[0]);
                return;
        }

        var newDelta = new int[ip.Dim];
        for (var i = 0; i < ip.Dim; i++)
        {
            newDelta[i] = ip.Delta[i];
        }

        newDelta[0] = ip.Delta[1];
        newDelta[1] = -ip.Delta[0];
        ip.Delta = new FungeVector(newDelta);
    }


    [Instruction('w')]
    public static void Compare(FungeIP ip, FungeInt a, FungeInt b)
    {
        if (a < b)
        {
            TurnLeft(ip);
        }
        else if (a > b)
        {
            TurnRight(ip);
        }
    }

    [Instruction(':')]
    public static void Duplicate(FungeIP ip, FungeInt v)
    {
        ip.PushToStack(v);
        ip.PushToStack(v);
    }

    [Instruction('$')]
    public static void Discard(FungeIP ip, FungeInt v) { }

    [Instruction('\\')]
    public static void Swap(FungeIP ip, FungeInt a, FungeInt b)
    {
        ip.PushToStack(b);
        ip.PushToStack(a);
    }

    [Instruction('n')]
    public static void ClearStack(FungeIP ip)
    {
        ip.StackStack.TOSS.Clear();
    }

    public static FungeFunc PushNumber(int number) => 
        new FungeAction(ip => ip.PushToStack(number));

    [Instruction('0')]
    public static readonly FungeFunc Push0 = PushNumber(0);
    [Instruction('1')]
    public static readonly FungeFunc Push1 = PushNumber(1);
    [Instruction('2')]
    public static readonly FungeFunc Push2 = PushNumber(2);
    [Instruction('3')]
    public static readonly FungeFunc Push3 = PushNumber(3);
    [Instruction('4')]
    public static readonly FungeFunc Push4 = PushNumber(4);
    [Instruction('5')]
    public static readonly FungeFunc Push5 = PushNumber(5);
    [Instruction('6')]
    public static readonly FungeFunc Push6 = PushNumber(6);
    [Instruction('7')]
    public static readonly FungeFunc Push7 = PushNumber(7);
    [Instruction('8')]
    public static readonly FungeFunc Push8 = PushNumber(8);
    [Instruction('9')]
    public static readonly FungeFunc Push9 = PushNumber(9);
    [Instruction('a')]
    public static readonly FungeFunc Push10 = PushNumber(10);
    [Instruction('b')]
    public static readonly FungeFunc Push11 = PushNumber(11);
    [Instruction('c')]
    public static readonly FungeFunc Push12 = PushNumber(12);
    [Instruction('d')]
    public static readonly FungeFunc Push13 = PushNumber(13);
    [Instruction('e')]
    public static readonly FungeFunc Push14 = PushNumber(14);
    [Instruction('f')]
    public static readonly FungeFunc Push15 = PushNumber(15);

    [Instruction('"')]
    public static void ToggleStringMode(FungeIP ip)
    {
        ip.StringMode = !ip.StringMode;
    }

    [Instruction('\'')]
    public static FungeInt FetchCharacter(FungeIP ip)
    {
        ip.Position += ip.Delta;
        return ip.Space[ip.Position];
    }
    
    [Instruction('s')]
    public static void Store(FungeIP ip, FungeInt value)
    {
        ip.Position += ip.Delta;
        ip.Space[ip.Position] = value;
    }

    [Instruction('#')]
    public static void Skip(FungeIP ip)
    {
        ip.Position += ip.Delta;
    }

    [Instruction('g')]
    public static FungeInt Get(FungeIP ip, FungeVector pos)
    {
        return ip.Get(pos);
    }

    [Instruction('p')]
    public static void Put(FungeIP ip, FungeInt v, FungeVector pos)
    {
        ip.Put(pos, v);
    }

    [Instruction('~')]
    public static FungeInt Input(FungeIP ip)
    {
        if (ip.Interpreter.EndOfInput())
        {
            throw new FungeReflectException(new InvalidOperationException("End of input reached."));
        }

        return ip.Interpreter.ReadCharacter();
    }

    [Instruction(',')]
    public static void Output(FungeIP ip, FungeInt v)
    {
        ip.Interpreter.WriteCharacter((char)v);
    }

    [Instruction('&')]
    public static FungeInt InputInteger(FungeIP ip)
    {
        if (ip.Interpreter.EndOfInput())
        {
            throw new FungeReflectException(new InvalidOperationException("End of input reached."));
        }

        return ip.Interpreter.ReadInteger();
    }

    [Instruction('.')]
    public static void OutputInteger(FungeIP ip, FungeInt v)
    {
        ip.Interpreter.WriteInteger(v);
    }

    [Instruction('i')]
    public static void InputFile(FungeIP ip, FungeVector pos, FungeInt flags, FungeString filename)
    {
        var size = ip.ReadFileIntoSpace(pos, filename, (flags & 1) != 0);
        ip.PushVectorToStack(size);
        ip.PushVectorToStack(pos);
    }

    [Instruction('o')]
    public static void OutputFile(FungeIP ip, FungeVector size, FungeVector pos, FungeInt flags, FungeString filename)
    {
        ip.WriteSpaceToFile(pos, size, filename, (flags & 1) != 0);
    }

    [Instruction('y')]
    public static void SysInfo(FungeIP ip, FungeInt n)
    {
        var tempStack = new FungeStack();

        // Environment variables
        tempStack.Push(0);
        var env = Environment.GetEnvironmentVariables();
        foreach (System.Collections.DictionaryEntry de in env)
        {
            tempStack.PushString(de.Key + "=" + de.Value);
        }

        // Command line arguments
        tempStack.Push(0);
        tempStack.Push(0);
        var args = Environment.GetCommandLineArgs();
        foreach (var arg in args)
        {
            tempStack.PushString(arg);
        }

        // Sizes of all stacks
        for (var i = ip.StackStack.Size - 1; i >= 0; i--)
        {
            tempStack.Push(ip.StackStack[i].Size);
        }

        // Size of stack stack
        tempStack.Push(ip.StackStack.Size);

        // Current time (hours * 256 * 256 + minutes * 256 + seconds)
        var now = DateTime.Now;
        tempStack.Push(now.Hour * 256 * 256 + now.Minute * 256 + now.Second);

        // Current date ((year - 1900) * 256 * 256 + month * 256 + day)
        tempStack.Push((now.Year - 1900) * 256 * 256 + now.Month * 256 + now.Day);

        // Upper bound of Funge-Space, relative to the lower bound
        tempStack.PushVector(ip.Space.MaxCoords - ip.Space.MinCoords, ip.Dim);
        // Lower bound of Funge-Space
        tempStack.PushVector(ip.Space.MinCoords, ip.Dim);

        // Current storage offset
        tempStack.PushVector(ip.StorageOffset, ip.Dim);
        // Current delta
        tempStack.PushVector(ip.Delta, ip.Dim);
        // Current position
        tempStack.PushVector(ip.Position, ip.Dim);

        // Team number of IP
        tempStack.Push(0);
        // ID of the IP
        tempStack.Push(ip.ID);
        // Number of dimensions
        tempStack.Push(ip.Dim);

        // Path separator character
        tempStack.Push(Path.DirectorySeparatorChar);

        // Behavior of '=': Unimplemented
        tempStack.Push(0);

        // Version number of ReFunge
        Version version = typeof(CoreInstructions).Assembly.GetName().Version!;
        tempStack.Push(version.Major * 256 * 256 + version.Minor * 256 + version.Revision);

        // Implementation handprint: ReFn
        tempStack.Push(new FungeString("ReFn").Handprint);

        // Amount of bytes per cell
        tempStack.Push(4);

        // Misc. flags:
        // LSB (0x01): 1 to indicate that 't' is implemented
        // 0x02: 1 to indicate that 'i' is implemented
        // 0x04: 1 to indicate that 'o' is implemented
        // 0x08: 0 to indicate that '=' is not implemented
        // 0x10: 0 to indicate that input is buffered
        tempStack.Push(0x07);

        if (n > tempStack.Size)
        {
            ip.PushToStack(ip.StackStack.TOSS[n - tempStack.Size - 1]);
        }
        else if (n > 0)
        {
            ip.PushToStack(tempStack[n - 1]);
        }
        else
        {
            for (var i = tempStack.Size - 1; i >= 0; i--)
            {
                ip.PushToStack(tempStack[i]);
            }
        }
    }

    [Instruction('j')]
    public static void JumpForward(FungeIP ip, FungeInt v)
    {
        ip.Position += ip.Delta * v;
    }

    [Instruction('x')]
    public static void SetDelta(FungeIP ip, FungeVector d)
    {
        ip.Delta = d;
    }

    [Instruction('k')]
    public static void Iterate(FungeIP ip, FungeInt n)
    {
        if (n <= 0)
        {
            ip.MoveToNext();
        }

        var op = ip.Space[ip.NextPosition()];
        for (var i = 0; i < n; i++)
        {
            ip.DoOp(op);
        }
    }

    [Instruction('@')]
    public static void Terminate(FungeIP ip)
    {
        ip.Alive = false;
    }

    [Instruction('q')]
    public static void Quit(FungeIP ip, FungeInt r)
    {
        ip.Interpreter.ReturnValue = r;
        ip.RequestQuit = true;
    }

    [Instruction('{')]
    public static void BeginBlock(FungeIP ip, FungeInt n)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = '}';
        ip.StackStack.NewStack();
        if (n > 0)
        {
            int[] newStack = new int[n];
            for (var i = 0; i < n; i++)
            {
                newStack[i] = ip.PopFromSOSS();
            }

            for (var i = n - 1; i >= 0; i--)
            {
                ip.PushToStack(newStack[i]);
            }
        }

        if (n < 0)
        {
            for (var i = 0; i > n; i--)
            {
                ip.PushToSOSS(0);
            }
        }

        ip.PushVectorToSOSS(ip.StorageOffset);
        ip.StorageOffset = ip.Position + ip.Delta;
    }

    [Instruction('}')]
    public static void EndBlock(FungeIP ip, FungeInt n)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = '{';
        if (ip.StackStack.Size == 1)
        {
            throw new FungeReflectException(new InvalidOperationException("}: No SOSS."));
        }

        ip.StorageOffset = ip.PopVectorFromSOSS();
        if (n > 0)
        {
            int[] transfer = new int[n];
            for (var i = 0; i < n; i++)
            {
                transfer[i] = ip.PopFromStack();
            }

            for (var i = n - 1; i >= 0; i--)
            {
                ip.PushToSOSS(transfer[i]);
            }
        }

        for (var i = 0; i > n; i--)
        {
            ip.PopFromSOSS();
        }

        ip.StackStack.RemoveStack();
    }

    [Instruction('u')]
    public static void StackTransfer(FungeIP ip, FungeInt count)
    {
        if (ip.StackStack.Size == 1)
        {
            throw new FungeReflectException(new InvalidOperationException("u: No SOSS."));
        }

        for (var i = 0; i < count; i++)
        {
            ip.PushToStack(ip.PopFromSOSS());
        }

        for (var i = 0; i > count; i--)
        {
            ip.PushToSOSS(ip.PopFromStack());
        }
    }

    [Instruction('z')]
    public static void DoNothing(FungeIP _)
    {
    }

    [Instruction('t')]
    public static void Split(FungeIP ip)
    {
        var newIP = ip.Split(ip.Interpreter.IPID++);
        ip.Interpreter.AddNewIP(newIP, ip);
    }

    [Instruction('(')]
    public static void LoadFingerprint(FungeIP ip, FungeInt n)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = ')';
        var code = 0;
        for (var i = 0; i < n; i++)
        {
            code = code * 256 + ip.PopFromStack();
        }

        try
        {
            ip.LoadFingerprint(code);
            ip.PushToStack(code);
            ip.PushToStack(1);
        }
        catch (Exception e) when (e is KeyNotFoundException or ArgumentException)
        {
            throw new FungeReflectException(e);
        }
    }

    [Instruction(')')]
    public static void UnloadFingerprint(FungeIP ip, FungeInt n)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = '(';
        var code = 0;
        for (var i = 0; i < n; i++)
        {
            code = code * 256 + ip.PopFromStack();
        }

        try
        {
            ip.UnloadFingerprint(code);
        }
        catch (Exception e) when (e is KeyNotFoundException or ArgumentException)
        {
            throw new FungeReflectException(e);
        }
    }
}