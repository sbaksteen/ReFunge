using ReFunge.Data.Values;

namespace ReFunge.Semantics
{
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
            new FungeAction(ip => ip.Delta = FungeVector.RIGHT);
        [Instruction('<')]
        public static readonly FungeFunc GoLeft = 
            new FungeAction(ip => ip.Delta = FungeVector.LEFT);
        [Instruction('^')]
        public static readonly FungeFunc GoUp = 
            new FungeAction(ip =>
        {
            if (ip.Dim > 1)
                ip.Delta = FungeVector.UP;
            else
                ip.Reflect();
        });
        [Instruction('v')]
        public static readonly FungeFunc GoDown = 
            new FungeAction(ip =>
        {
            if (ip.Dim > 1)
                ip.Delta = FungeVector.DOWN;
            else
                ip.Reflect();
        });
        [Instruction('h')]
        public static readonly FungeFunc GoForwards = 
            new FungeAction(ip =>
        {
            if (ip.Dim > 2)
                ip.Delta = FungeVector.FORWARDS;
            else
                ip.Reflect();
        });
        [Instruction('l')]
        public static readonly FungeFunc GoBackwards = 
            new FungeAction(ip =>
        {
            if (ip.Dim > 2)
                ip.Delta = FungeVector.BACKWARDS;
            else
                ip.Reflect();
        });

        [Instruction('_')]
        public static readonly FungeFunc DecideHorizontal = 
            new FungeAction<FungeInt>((ip, v) =>
        {
            ip.Delta = v == 0 ? FungeVector.RIGHT : FungeVector.LEFT;
        });

        [Instruction('|')]
        public static readonly FungeFunc DecideVertical = 
            new FungeAction<FungeInt>((ip, v) =>
        {
            if (ip.Dim < 2)
            {
                ip.Reflect();
                return;
            }

            ip.Delta = v == 0 ? FungeVector.DOWN : FungeVector.UP;
        });

        [Instruction('m')]
        public static readonly FungeFunc DecideForwards = 
            new FungeAction<FungeInt>((ip, v) =>
        {
            if (ip.Dim < 3)
            {
                ip.Reflect();
                return;
            }

            ip.Delta = v == 0 ? FungeVector.BACKWARDS : FungeVector.FORWARDS;
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
            if (ip.Dim < 2)
            {
                ip.Reflect();
                return;
            }
            if (ip.Delta.Size <= 2)
            {
                ip.Delta = new FungeVector(-ip.Delta[1], ip.Delta[0]);
                return;
            }
            int[] newDelta = new int[ip.Delta.Size];
            for (int i = 0; i < ip.Dim; i++)
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
            if (ip.Dim < 2)
            {
                ip.Reflect();
                return;
            }
            if (ip.Delta.Size <= 2)
            {
                ip.Delta = new FungeVector(ip.Delta[1], -ip.Delta[0]);
                return;
            }
            int[] newDelta = new int[ip.Delta.Size];
            for (int i = 0; i < ip.Dim; i++)
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
            ip.MoveToNext();
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
            new FungeAction(ip =>
            {
                if (ip.Interpreter.EndOfInput())
                {
                    ip.Reflect();
                    return ;
                }
                ip.PushToStack(ip.Interpreter.ReadCharacter());
            });
        [Instruction(',')]
        public static readonly FungeFunc Output = 
            new FungeAction<FungeInt>((ip, v) => ip.Interpreter.WriteCharacter((char)v));

        [Instruction('&')]
        public static readonly FungeFunc InputInteger = 
            new FungeAction(ip =>
            {
                if (ip.Interpreter.EndOfInput())
                {
                    ip.Reflect();
                    return;
                }
                ip.PushToStack(ip.Interpreter.ReadInteger());
            });
        [Instruction('.')]
        public static readonly FungeFunc OutputInteger = 
            new FungeAction<FungeInt>((ip, v) => ip.Interpreter.WriteInteger(v));

        [Instruction('i')]
        public static readonly FungeFunc InputFile = 
            new FungeAction<FungeVector, FungeInt, FungeString>((ip, pos, flags, filename) =>
        {
            FungeVector? size = ip.ReadFileIntoSpace(pos, filename, (flags & 1) != 0);
            if (size.HasValue)
            {
                ip.PushVectorToStack(size.Value);
                ip.PushVectorToStack(pos);
            }
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
            int s = ip.StackStack.TOSS.Size;

            // Environment variables
            ip.PushToStack(0);
            var env = Environment.GetEnvironmentVariables();
            foreach (System.Collections.DictionaryEntry de in env)
            {
                ip.PushStringToStack(de.Key + "=" + de.Value);
            }

            // Command line arguments
            ip.PushToStack(0);
            var args = Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                ip.PushStringToStack(arg);
            }

            // Sizes of all stacks
            for (var i = ip.StackStack.Size - 1; i >= 0; i--)
            {
                ip.PushToStack(ip.StackStack[i].Size);
            }
            ip.PushToStack(s);

            // Size of stack stack
            ip.PushToStack(ip.StackStack.Size);

            // Current time (hours * 256 * 256 + minutes * 256 + seconds)
            var now = DateTime.Now;
            ip.PushToStack(now.Hour * 256 * 256 + now.Minute * 256 + now.Second);

            // Current date ((year - 1900) * 256 * 256 + month * 256 + day)
            ip.PushToStack((now.Year - 1900) * 256 * 256 + now.Month * 256 + now.Day);

            // Upper bound of Funge-Space, relative to the lower bound
            ip.PushVectorToStack(ip.Space.MaxCoords - ip.Space.MinCoords);
            // Lower bound of Funge-Space
            ip.PushVectorToStack(ip.Space.MinCoords);

            // Current storage offset
            ip.PushVectorToStack(ip.StorageOffset);
            // Current delta
            ip.PushVectorToStack(ip.Delta);
            // Current position
            ip.PushVectorToStack(ip.Position);

            // Team number of IP
            ip.PushToStack(0);
            // ID of the IP
            ip.PushToStack(ip.ID);
            // Number of dimensions
            ip.PushToStack(ip.Dim);

            // Path separator character
            ip.PushToStack(Path.DirectorySeparatorChar);

            // Behavior of '='
            ip.PushToStack(0);

            // Version number of ReFunge
            ip.PushToStack(101); // Current version: 1.01, we can't really push 0.01.

            // Implementation handprint: ReFn
            ip.PushToStack(new FungeString("ReFn").Handprint);

            // Amount of bytes per cell
            ip.PushToStack(4);

            // Misc. flags:
            // LSB (0x01): 1 to indicate that 't' is implemented
            // 0x02: 1 to indicate that 'i' is implemented
            // 0x04: 1 to indicate that 'o' is implemented
            // 0x08: 0 to indicate that '=' is not implemented
            // 0x10: 0 to indicate that input is buffered
            ip.PushToStack(0x07);

            if (n <= 0) return;
            var r = ip.StackStack.TOSS[n-1];
            ip.StackStack.TOSS.Clear();
            ip.PushToStack(r);
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
            for (int i = 0; i < n; i++)
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
            ip.StackStack.NewStack(n);
            ip.PushVectorToStack(ip.StorageOffset);
            ip.StorageOffset = ip.NextPosition();
        });

        [Instruction('}')]
        public static readonly FungeFunc EndBlock = 
            new FungeAction<FungeInt>((ip, n) =>
        {
            if (ip.StackStack.Size == 1)
            {
                ip.Reflect();
                return;
            }
            ip.StorageOffset = ip.StackStack.SOSS!.PopVector(ip.Dim);
            ip.StackStack.RemoveStack(n);
        });

        [Instruction('u')]
        public static readonly FungeFunc StackTransfer = 
            new FungeAction<FungeInt>((ip, count) =>
        {
            if (ip.StackStack.Size == 1)
            {
                ip.Reflect();
                return;
            }
            for (int i = 0; i < count; i++)
            {
                ip.StackStack.TOSS.Push(ip.StackStack.SOSS!.Pop());
            }
            for (int i = 0; i > count; i--)
            {
                ip.StackStack.SOSS!.Push(ip.StackStack.TOSS.Pop());
            }
        });

        [Instruction('z')]
        public static readonly FungeFunc DoNothing = 
            new FungeAction(_ => { });

        [Instruction('t')]
        public static readonly FungeFunc Split = 
            new FungeAction(ip => ip.Split = true);

        [Instruction('(')]
        public static readonly FungeFunc LoadFingerprint = 
            new FungeAction<FungeInt>((ip, n) =>
        {
            int code = 0;
            for (int i = 0; i < n; i++)
            {
                code = code * 256 + ip.PopFromStack();
            }
            try
            {
                ip.LoadFingerprint(code);
                ip.PushToStack(code);
                ip.PushToStack(1);
            } catch (KeyNotFoundException)
            {
                ip.Reflect();
            }
        });

        [Instruction(')')]
        public static readonly FungeFunc UnloadFingerprint = 
            new FungeAction<FungeInt>((ip, n) =>
        {
            int code = 0;
            for (int i = 0; i < n; i++)
            {
                code = code * 256 + ip.PopFromStack();
            }
            try
            {
                ip.UnloadFingerprint(code);
            }
            catch (KeyNotFoundException)
            {
                ip.Reflect();
            }
        });

    }
}
