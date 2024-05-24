using ReFunge.Data;
using ReFunge.Data.Values;

namespace ReFunge.Semantics;

internal static class CoreInstructions
{

    [Instruction('+')]
    public static readonly FungeFunc Add = 
        new FungeFunc<FungeInt, FungeInt, FungeInt>((_, a, b) => a + b);
    [Instruction('-')]
    public static readonly FungeFunc Subtract = 
        new FungeFunc<FungeInt, FungeInt, FungeInt>((_, a, b) => a - b);
    [Instruction('*')]
    public static readonly FungeFunc Multiply = 
        new FungeFunc<FungeInt, FungeInt, FungeInt>((_, a, b) => a * b);
    [Instruction('/')]
    public static readonly FungeFunc Divide = 
        new FungeFunc<FungeInt, FungeInt, FungeInt>((_, a, b) => b == 0 ? 0 : a / b);
    [Instruction('%')]
    public static readonly FungeFunc Modulo = 
        new FungeFunc<FungeInt, FungeInt, FungeInt>((_, a, b) => b == 0 ? 0 : a % b);

    [Instruction('`')]
    public static readonly FungeFunc GreaterThan = 
        new FungeFunc<FungeInt, FungeInt, FungeInt>((_, a, b) => a > b ? 1 : 0);

    [Instruction('!')]
    public static readonly FungeFunc LogicalNot = 
        new FungeFunc<FungeInt, FungeInt>((_, v) => v == 0 ? 1 : 0);

    [Instruction('>')]
    public static readonly FungeFunc GoRight = 
        new FungeAction(ip =>
        {
            if (ip.HoverMode)
            {
                ip.Delta += FungeVector.Right;
            }
            else
            {
                ip.Delta = FungeVector.Right;
            }
        });
    [Instruction('<')]
    public static readonly FungeFunc GoLeft = 
        new FungeAction(ip =>
        {
            if (ip.HoverMode)
            {
                ip.Delta += FungeVector.Left;
            }
            else
            {
                ip.Delta = FungeVector.Left;
            }
        });
    [Instruction('^')]
    public static readonly FungeFunc GoUp = 
        new FungeAction(ip =>
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
        });
    [Instruction('v')]
    public static readonly FungeFunc GoDown = 
        new FungeAction(ip =>
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
        });
    [Instruction('h')]
    public static readonly FungeFunc GoForwards = 
        new FungeAction(ip =>
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
        });
    [Instruction('l')]
    public static readonly FungeFunc GoBackwards = 
        new FungeAction(ip =>
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
        });

    [Instruction('_')]
    public static readonly FungeFunc DecideHorizontal = 
        new FungeAction<FungeInt>((ip, v) =>
        {
            if (v == 0)
                GoRight.Execute(ip);
            else
                GoLeft.Execute(ip);
        });

    [Instruction('|')]
    public static readonly FungeFunc DecideVertical = 
        new FungeAction<FungeInt>((ip, v) =>
        {
            if (ip.Dim < 2) throw new FungeReflectException();

            if (v == 0) 
                GoDown.Execute(ip);
            else
                GoUp.Execute(ip);
        });

    [Instruction('m')]
    public static readonly FungeFunc DecideForwards = 
        new FungeAction<FungeInt>((ip, v) =>
        {
            if (ip.Dim < 3) throw new FungeReflectException();

            if (v == 0)
                GoBackwards.Execute(ip);
            else
                GoForwards.Execute(ip);
        });

    [Instruction('?')]
    public static readonly FungeFunc GoRandom = 
        new FungeAction(ip =>
        {
            ip.Delta = FungeVector.Cardinal(Random.Shared.Next(ip.Dim), Random.Shared.Next(2) * 2 - 1);
        });

    [Instruction('r')]
    public static readonly FungeFunc Reflect = 
        new FungeAction(ip => ip.Reflect());

    [Instruction(']')]
    public static readonly FungeFunc TurnRight = 
        new FungeAction(ip =>
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
        });

    [Instruction('[')]
    public static readonly FungeFunc TurnLeft = 
        new FungeAction(ip =>
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
        });

    [Instruction('w')]
    public static readonly FungeFunc Compare = 
        new FungeAction<FungeInt, FungeInt>((ip, a, b) =>
        {
            if (a < b)
            {
                TurnLeft.Execute(ip);
            }
            else if (a > b)
            {
                TurnRight.Execute(ip);
            }
        });

    [Instruction(':')]
    public static readonly FungeFunc Duplicate = 
        new FungeAction<FungeInt>((ip, v) =>
        {
            ip.PushToStack(v);
            ip.PushToStack(v);
        });
    [Instruction('$')]
    public static readonly FungeFunc Discard = 
        new FungeAction<FungeInt>((_, _) => { });
    [Instruction('\\')]
    public static readonly FungeFunc Swap = 
        new FungeAction<FungeInt, FungeInt>((ip, a, b) =>
        {
            ip.PushToStack(b);
            ip.PushToStack(a);
        });
    [Instruction('n')]
    public static readonly FungeFunc ClearStack = 
        new FungeAction(ip => ip.StackStack.TOSS.Clear());

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
    public static readonly FungeFunc ToggleStringMode = 
        new FungeAction(ip =>
        {
            ip.StringMode = !ip.StringMode;
        });

    [Instruction('\'')]
    public static readonly FungeFunc FetchCharacter = 
        new FungeFunc<FungeInt>(ip =>
        {
            ip.Position += ip.Delta;
            return ip.Space[ip.Position];
        });
    [Instruction('s')]
    public static readonly FungeFunc StoreCharacter = 
        new FungeAction<FungeInt>((ip, v) => 
        {
            ip.Position += ip.Delta;
            ip.Space[ip.Position] = v;
        });

    [Instruction('#')]
    public static readonly FungeFunc Skip = 
        new FungeAction(ip => ip.Position += ip.Delta);

    [Instruction('g')]
    public static readonly FungeFunc Get = 
        new FungeFunc<FungeVector, FungeInt>((ip, pos) => ip.Get(pos));
    [Instruction('p')]
    public static readonly FungeFunc Put = 
        new FungeAction<FungeInt, FungeVector>((ip, v, pos) => ip.Put(pos, v));

    [Instruction('~')]
    public static readonly FungeFunc Input = 
        new FungeFunc<FungeInt>(ip =>
        {
            if (ip.Interpreter.EndOfInput())
            {
                throw new FungeReflectException(new InvalidOperationException("End of input reached."));
            }
            return ip.Interpreter.ReadCharacter();
        });
    [Instruction(',')]
    public static readonly FungeFunc Output = 
        new FungeAction<FungeInt>((ip, v) => ip.Interpreter.WriteCharacter((char)v));

    [Instruction('&')]
    public static readonly FungeFunc InputInteger = 
        new FungeFunc<FungeInt>(ip =>
        {
            if (ip.Interpreter.EndOfInput())
            {
                throw new FungeReflectException(new InvalidOperationException("End of input reached."));
            }
            return ip.Interpreter.ReadInteger();
        });
    [Instruction('.')]
    public static readonly FungeFunc OutputInteger = 
        new FungeAction<FungeInt>((ip, v) => ip.Interpreter.WriteInteger(v));

    [Instruction('i')]
    public static readonly FungeFunc InputFile = 
        new FungeAction<FungeVector, FungeInt, FungeString>((ip, pos, flags, filename) =>
        {
            var size = ip.ReadFileIntoSpace(pos, filename, (flags & 1) != 0);
            ip.PushVectorToStack(size);
            ip.PushVectorToStack(pos);
        });

    [Instruction('o')]
    public static readonly FungeFunc OutputFile = 
        new FungeAction<FungeVector, FungeVector, FungeInt, FungeString>((ip, size, pos, flags, filename) =>
        {
            ip.WriteSpaceToFile(pos, size, filename, (flags & 1) != 0);
        });

    [Instruction('y')]
    public static readonly FungeFunc SysInfo = 
        new FungeAction<FungeInt>((ip, n) =>
        {
            var tempStack = new FungeStack();
            var s = ip.StackStack.TOSS.Size;

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
            tempStack.Push(101); // Current version: 1.01, we can't really push 0.01.

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
                ip.PushToStack(tempStack[n-1]);
            }
            else
            {
                for (var i = tempStack.Size-1; i >= 0; i--)
                {
                    ip.PushToStack(tempStack[i]);
                }
            }
        });

    [Instruction('j')]
    public static readonly FungeFunc JumpForward = 
        new FungeAction<FungeInt>((ip, v) =>
        {
            ip.Position += ip.Delta * v;
        });

    [Instruction('x')]
    public static readonly FungeFunc SetDelta = 
        new FungeAction<FungeVector>((ip, d) =>
        {
            ip.Delta = d;
        });

    [Instruction('k')]
    public static readonly FungeFunc Iterate = 
        new FungeAction<FungeInt>((ip, n) =>
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
        });

    [Instruction('@')]
    public static readonly FungeFunc Terminate = 
        new FungeAction(ip => ip.Alive = false);

    [Instruction('q')]
    public static readonly FungeFunc Quit = 
        new FungeAction<FungeInt>((ip, r) => 
        {
            ip.Interpreter.ReturnValue = r;
            ip.RequestQuit = true;
        });

    [Instruction('{')]
    public static readonly FungeFunc BeginBlock = 
        new FungeAction<FungeInt>((ip, n) =>
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
        });

    [Instruction('}')]
    public static readonly FungeFunc EndBlock = 
        new FungeAction<FungeInt>((ip, n) =>
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
        });

    [Instruction('u')]
    public static readonly FungeFunc StackTransfer = 
        new FungeAction<FungeInt>((ip, count) =>
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
        });

    [Instruction('z')]
    public static readonly FungeFunc DoNothing = 
        new FungeAction(_ => { });

    [Instruction('t')]
    public static readonly FungeFunc Split = 
        new FungeAction(ip =>
        {
            var newIP = ip.Split(ip.Interpreter.IPID++);
            ip.Interpreter.AddNewIP(newIP, ip);
        });

    [Instruction('(')]
    public static readonly FungeFunc LoadFingerprint = 
        new FungeAction<FungeInt>((ip, n) =>
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
            } catch (KeyNotFoundException e)
            {
                throw new FungeReflectException(e);
            }
        });

    [Instruction(')')]
    public static readonly FungeFunc UnloadFingerprint = 
        new FungeAction<FungeInt>((ip, n) =>
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
            catch (KeyNotFoundException e)
            {
                throw new FungeReflectException(e);
            }
        });

}