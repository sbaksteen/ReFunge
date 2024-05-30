using System.Runtime.InteropServices.JavaScript;
using ReFunge.Data;
using ReFunge.Data.Values;
using ReFunge.Semantics;
using ReFunge.Semantics.Fingerprints;

namespace ReFunge;

using InstructionMap = Dictionary<FungeInt, FungeFunc>;

/// <summary>
/// Represents any of various modes an IP can be in. These modes can be independently active. <seealso cref="MODE"/>
/// </summary>
[Flags]
public enum IPModes
{
    None = 0,

    /// <summary>
    /// Instead of executing the operations associated with <c>FungeSpace</c> cells, the IP instead pushes
    /// the integer values of each cell to the stack until it encounters a cell with the value &quot;.
    /// <seealso cref="CoreInstructions.ToggleStringMode"/>
    /// </summary>
    StringMode = 1,

    /// <summary>
    /// The directional instructions &lt;&gt;^vhl and _|m all add to the IP's delta instead of setting it.
    /// <seealso cref="MODE.ToggleHoverMode"/>
    /// </summary>
    HoverMode = 2,

    /// <summary>
    /// The IP always pushes to the bottom of any <see cref="FungeStack"/>s. This does not affect pushing
    /// new stacks to a <see cref="FungeStackStack"/>.
    /// <seealso cref="MODE.ToggleInvertMode"/>
    /// </summary>
    InvertMode = 4,

    /// <summary>
    /// The IP always pops from the bottom of any <see cref="FungeStack"/>s. This does not affect popping
    /// stacks from a <see cref="FungeStackStack"/>.
    /// <seealso cref="MODE.ToggleQueueMode"/>
    /// </summary>
    QueueMode = 8,

    /// <summary>
    /// The instructions [](){} all cause the cell at the IP's current position to turn into the opposite bracket
    /// in addition to their usual effects. For example, with the IP in switch mode, the [ instruction will set
    /// the cell at the IP's current position to ] and also turn left.
    /// <seealso cref="MODE.ToggleSwitchMode"/>
    /// </summary>
    SwitchMode = 16
}

/// <summary>
/// An IP (instruction pointer) which lives in a <c>FungeSpace</c>. It can execute instructions in the space and
/// interact with the interpreter's global functionality.
/// </summary>
public class FungeIP
{

    

    /// <summary>
    /// The IP's dimensionality. This defines how the IP sees the world, and how it pushes and pops <c>FungeVectors</c>
    /// on its stack.
    /// </summary>
    public int Dim { get; set; }
    
    /// <summary>
    /// The space the IP currently lives in.
    /// </summary>
    public FungeSpace Space { get; set; }
    
    /// <summary>
    /// The stack-stack associated with the IP.
    /// </summary>
    public FungeStackStack StackStack { get; set; }

    /// <summary>
    /// The interpreter this IP is bound to.
    /// </summary>
    public Interpreter Interpreter { get; set; }

    /// <summary>
    /// This determines whether the IP is dead or alive. If an IP is dead, it will not execute instructions, and
    /// it will be removed from the interpreter's IP list at the earliest convenience.
    /// </summary>
    public bool Alive { get; set; } = true;
    
    /// <summary>
    /// If this value is set during execution, the interpreter will immediately stop running after the IP's "turn" is
    /// over.
    /// </summary>
    public bool RequestQuit { get; set; } = false;
    
    /// <summary>
    /// This flags enum determines whether the IP is in any number of modes. See <see cref="IPModes"/> for an overview.
    /// </summary>
    private IPModes Modes { get; set; } = IPModes.None;
    
    private Dictionary<FungeInt, InstancedFingerprint> _instancedFingerprints = new();

    public void ToggleModes(IPModes modes)
    {
        Modes ^= modes;
    }

    public bool StringMode
    {
        get => Modes.HasFlag(IPModes.StringMode);
        set
        {
            if (value)
            {
                Modes |= IPModes.StringMode;
            }
            else
            {
                Modes &= ~IPModes.StringMode;
            }
        }
    }
    
    public bool HoverMode
    {
        get => Modes.HasFlag(IPModes.HoverMode);
        set
        {
            if (value)
            {
                Modes |= IPModes.HoverMode;
            }
            else
            {
                Modes &= ~IPModes.HoverMode;
            }
        }
    }
    
    public bool InvertMode
    {
        get => Modes.HasFlag(IPModes.InvertMode);
        set
        {
            if (value)
            {
                Modes |= IPModes.InvertMode;
            }
            else
            {
                Modes &= ~IPModes.InvertMode;
            }
        }
    }
    
    public bool QueueMode
    {
        get => Modes.HasFlag(IPModes.QueueMode);
        set
        {
            if (value)
            {
                Modes |= IPModes.QueueMode;
            }
            else
            {
                Modes &= ~IPModes.QueueMode;
            }
        }
    }
    
    public bool SwitchMode
    {
        get => Modes.HasFlag(IPModes.SwitchMode);
        set
        {
            if (value)
            {
                Modes |= IPModes.SwitchMode;
            }
            else
            {
                Modes &= ~IPModes.SwitchMode;
            }
        }
    }


    public FungeVector Position = new();
    public FungeVector StorageOffset = new();
    public FungeVector Delta = FungeVector.Right;
    
    private InstructionRegistry InstructionRegistry => Interpreter.InstructionRegistry;

    internal InstructionMap Functions { get; set; }

    public Stack<FungeFunc>[] FingerprintStacks { get; } = new Stack<FungeFunc>[26];

    public FungeInt ID { get; }

    public FungeIP(int id, FungeSpace space, Interpreter interpreter)
    {
        Space = space;
        Dim = space.Dim;
        StackStack = new FungeStackStack();
        Functions = new InstructionMap(interpreter.InstructionRegistry.CoreInstructions);
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
        var dict = FindFingerprint(code);

        foreach (var pair in dict)
        {
            FingerprintStacks[pair.Key - 'A'].Push(pair.Value);
        }
    }

    private InstructionMap FindFingerprint(FungeInt code)
    {
        InstructionMap dict;
        switch (InstructionRegistry.TypeOf(code))
        {
            case FingerprintType.Static:
                dict = InstructionRegistry.GetStaticFingerprint(code);
                break;
            case FingerprintType.InstancedPerInterpreter:
                dict = InstructionRegistry.GetInterpreterFingerprint(code);
                break;
            case FingerprintType.InstancedPerSpace:
                // Not implemented yet
                throw new NotImplementedException();
                break;
            case FingerprintType.InstancedPerIP:
                if (!_instancedFingerprints.TryGetValue(code, out var fingerprint))
                {
                    fingerprint = InstructionRegistry.NewInstance(code, this);
                    _instancedFingerprints[code] = fingerprint;
                }
                dict = fingerprint.Instructions;
                break;
            default:
                throw new InvalidOperationException("Unknown fingerprint type");
        }

        return dict;
    }

    public void UnloadFingerprint(FungeInt code)
    {
        var dict = FindFingerprint(code);
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
        StackStack.TOSS.Push(value, InvertMode);
    }

    public FungeInt PopFromStack()
    {
        return StackStack.TOSS.Pop(QueueMode);
    }
    
    public void PushToSOSS(FungeInt value)
    {
        StackStack.SOSS!.Push(value, InvertMode);
    }
    
    public FungeInt PopFromSOSS()
    {
        return StackStack.SOSS!.Pop(QueueMode);
    }

    public void PushVectorToStack(FungeVector vector)
    {
        StackStack.TOSS.PushVector(vector, Dim, InvertMode);
    }

    public FungeVector PopVectorFromStack()
    {
        return StackStack.TOSS.PopVector(Dim, QueueMode);
    }
    
    public void PushVectorToSOSS(FungeVector vector)
    {
        StackStack.SOSS!.PushVector(vector, Dim, InvertMode);
    }
    
    public FungeVector PopVectorFromSOSS()
    {
        return StackStack.SOSS!.PopVector(Dim, QueueMode);
    }

    public void PushStringToStack(FungeString str)
    {
        StackStack.TOSS.PushString(str, InvertMode);
    }

    public FungeString PopStringFromStack()
    {
        return StackStack.TOSS.PopString(QueueMode);
    }
    
    public void PushStringToSOSS(FungeString str)
    {
        StackStack.SOSS!.PushString(str, InvertMode);
    }
    
    public FungeString PopStringFromSOSS()
    {
        return StackStack.SOSS!.PopString(QueueMode);
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

        while (comment || (Space[newPosition] == ' ' && skipSpaces) || (!StringMode && Space[newPosition] == ';'))
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

    internal FungeVector ReadFileIntoSpace(FungeVector pos, string path, bool binary = false) {
        try
        {
            return Space.LoadFile(pos + StorageOffset, path, binary);
        }
        catch (Exception)
        {
            throw new FungeReflectException();
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
        newIP.MoveToNext();
        return newIP;
    }
}