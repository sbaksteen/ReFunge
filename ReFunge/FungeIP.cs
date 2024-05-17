using System.Runtime.InteropServices.JavaScript;
using ReFunge.Data;
using ReFunge.Data.Values;
using ReFunge.Semantics;

namespace ReFunge
{
    using InstructionMap = Dictionary<FungeInt, FungeFunc>;
    internal class FungeIP
    {
        public int Dim { get; set; }
        public FungeSpace Space { get; set; }
        public FungeStackStack StackStack { get; set; }

        public Interpreter Interpreter { get; set; }

        public bool Alive { get; set; } = true;
        public bool RequestQuit { get; set; } = false;

        public bool StringMode { get; set; } = false;

        public FungeVector Position = new();
        public FungeVector StorageOffset = new();
        public FungeVector Delta = FungeVector.Right;

        internal InstructionMap Functions { get; set; } = new(InstructionRegistry.CoreInstructions);

        internal Stack<FungeFunc>[] FingerprintStacks { get; } = new Stack<FungeFunc>[26];

        internal FungeInt ID { get; }

        public FungeIP(int id, FungeSpace space, Interpreter interpreter)
        {
            Space = space;
            Dim = space.Dim;
            StackStack = new FungeStackStack();
            ID = id;
            for (var i = 0; i < 26; i++)
            {
                FingerprintStacks[i] = new Stack<FungeFunc>();
            }
            Interpreter = interpreter;
        }

        public void DoOp(FungeInt op)
        {
            if (op >= 'A' && op <= 'Z')
            {
                // Special case: Fingerprint-specific command
                if (FingerprintStacks[op - 'A'].Count > 0)
                {
                    var func = FingerprintStacks[op - 'A'].Peek();
                    func.Execute(this);
                }
                else
                {
                    Reflect();
                }
                return;
            }
            if (Functions.TryGetValue(op, out var value))
            {
                value.Execute(this);
            }
            else
            {
                Interpreter.WriteError("Unknown command: " + op + " (" + (char)op + ")\n");
            }
        }

        public void Step()
        {
            if (!Alive) return;
            if (StringMode)
            {
                if (Space[Position] == '"')
                {
                    StringMode = false;
                }
                else
                {
                    PushToStack(Space[Position]);
                }
            } else {
                DoOp(Space[Position]);
            }
            MoveToNext();
        }

        public void LoadFingerprint(FungeInt code)
        {
            var dict = InstructionRegistry.GetFingerprint(code);
            foreach (var pair in dict)
            {
                FingerprintStacks[pair.Key - 'A'].Push(pair.Value);
            }
        }

        public void UnloadFingerprint(FungeInt code)
        {
            var dict = InstructionRegistry.GetFingerprint(code);
            foreach (var pair in dict)
            {
                FingerprintStacks[pair.Key - 'A'].Pop();
            }
        }

        public override string ToString()
        {
            return "IP " + ID + " at " + Position + " with delta " + Delta;
        }

        public void PushToStack(FungeInt value)
        {
            StackStack.TOSS.Push(value);
        }

        public FungeInt PopFromStack()
        {
            return StackStack.TOSS.Pop();
        }

        public void PushVectorToStack(FungeVector vector)
        {
            StackStack.TOSS.PushVector(vector, Dim);
        }

        public FungeVector PopVectorFromStack()
        {
            return StackStack.TOSS.PopVector(Dim);
        }

        public void PushStringToStack(FungeString str)
        {
            StackStack.TOSS.PushString(str);
        }

        public FungeString PopStringFromStack()
        {
            return StackStack.TOSS.PopString();
        }

        internal void Reflect()
        {
            Delta = -Delta;
        }

        internal FungeVector MoveAndWrap(FungeVector position)
        {
            position += Delta;
            if (Space.OutOfBounds(position))
            {
                position = Space.Wrap(position, Delta);
            }
            return position;
        }

        internal FungeVector NextPosition()
        {
            var newPosition = Position + Delta;
            var comment = false;
            
            // If we're in string mode and we don't start on a space, we don't skip spaces.
            var skipSpaces = !StringMode || Space[Position] == ' ';

            while (comment || ((Space[newPosition] == ' ' || Space[newPosition] == ';') && skipSpaces))
            {
                // There are no comments in string mode.
                if (Space[newPosition] == ';' && !StringMode) comment = !comment;
                newPosition = MoveAndWrap(newPosition);
            }
            return newPosition;
        }

        internal void MoveToNext()
        {
            Position = NextPosition();
        }

        internal void Put(FungeVector pos, FungeInt value)
        {
            Space[pos + StorageOffset] = value;
        }

        internal FungeInt Get(FungeVector pos)
        {
            return Space[pos + StorageOffset];
        }

        internal FungeVector? ReadFileIntoSpace(FungeVector pos, string path, bool binary = false) {
            try
            {
                return Space.LoadFile(pos + StorageOffset, path, binary);
            }
            catch (Exception)
            {
                Reflect();
                return null;
            }
        }

        internal void WriteSpaceToFile(FungeVector pos, FungeVector size, FungeString filename, bool linear = false)
        {
            try
            {
                Space.WriteToFile(pos + StorageOffset, size, filename, linear);
            }
            catch (Exception)
            {
                Reflect();
            }
        }

        internal FungeIP Split(int id)
        {
            FungeIP newIP = new(id, Space, Interpreter)
            {
                StackStack = StackStack.Clone(),
                Dim = Dim,
                Position = Position,
                Delta = -Delta,
                StorageOffset = StorageOffset,
                Functions = new InstructionMap(Functions),
                Interpreter = Interpreter
            };
            for (var i = 0; i < 26; i++)
            {
                newIP.FingerprintStacks[i] = new Stack<FungeFunc>(FingerprintStacks[i]);
            }
            return newIP;
        }
    }
}
