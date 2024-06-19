using ReFunge.Data;
using ReFunge.Data.Values;
using ReFunge.Semantics;
using ReFunge.Semantics.Fingerprints;
using ReFunge.Semantics.Fingerprints.Core;

namespace ReFunge;

using InstructionMap = Dictionary<FungeInt, FungeInstruction>;

/// <summary>
///     Represents any of various modes an IP can be in. These modes can be independently active.
/// </summary>
/// <seealso cref="MODE" />
[Flags]
public enum IPModes
{
    None = 0,

    /// <summary>
    ///     Instead of executing the operations associated with <see cref="FungeSpace" /> cells, the IP instead pushes
    ///     the integer values of each cell to the stack until it encounters a cell with the value &quot;.
    /// </summary>
    /// <seealso cref="CoreInstructions.ToggleStringMode" />
    StringMode = 1,

    /// <summary>
    ///     The directional instructions &lt;&gt;^vhl and _|m all add to the IP's delta instead of setting it.
    /// </summary>
    /// <seealso cref="MODE.ToggleHoverMode" />
    HoverMode = 2,

    /// <summary>
    ///     The IP always pushes to the bottom of any <see cref="FungeStack" />s. This does not affect pushing
    ///     new stacks to a <see cref="FungeStackStack" />.
    /// </summary>
    /// <seealso cref="MODE.ToggleInvertMode" />
    InvertMode = 4,

    /// <summary>
    ///     The IP always pops from the bottom of any <see cref="FungeStack" />s. This does not affect popping
    ///     stacks from a <see cref="FungeStackStack" />.
    /// </summary>
    /// <seealso cref="MODE.ToggleQueueMode" />
    QueueMode = 8,

    /// <summary>
    ///     The instructions [](){} all cause the cell at the IP's current position to turn into the opposite bracket
    ///     in addition to their usual effects. For example, with the IP in switch mode, the [ instruction will set
    ///     the cell at the IP's current position to ] and also turn left.
    /// </summary>
    /// <seealso cref="MODE.ToggleSwitchMode" />
    SwitchMode = 16
}

/// <summary>
///     An IP (instruction pointer) which lives in a <see cref="FungeSpace" />. It can execute instructions in the space
///     and
///     interact with the interpreter's global functionality.
/// </summary>
public class FungeIP
{
    /// <summary>
    ///     Stores all instanced fingerprints defined for this IP.
    /// </summary>
    /// <remarks>
    ///     For an example of an instanced fingerprint, see <see cref="HRTI" />.
    /// </remarks>
    /// <seealso cref="FingerprintType.InstancedPerIP" />
    private readonly Dictionary<FungeInt, InstancedFingerprint> _instancedFingerprints = new();

    /// <summary>
    ///     The delta of the IP. This is the direction the IP will move in every tick after executing an instruction.
    /// </summary>
    public FungeVector Delta = FungeVector.Right;

    /// <summary>
    ///     The position of the IP in the space.
    /// </summary>
    public FungeVector Position = new();

    /// <summary>
    ///     The storage offset of the IP. This is added to all positions the IP reads from or writes to using the
    ///     <see cref="Get" /> and <see cref="Put" /> methods.
    /// </summary>
    public FungeVector StorageOffset = new();

    /// <summary>
    ///     Creates a new IP with the given ID, <see cref="FungeSpace" />, and <see cref="Interpreter" />.
    ///     The IP is placed at the origin of the space, with a delta pointing to the right, an empty stack-stack,
    ///     and no fingerprints or active modes.
    /// </summary>
    /// <param name="id">A unique identifier for the IP.</param>
    /// <param name="space">The <see cref="FungeSpace" /> in which to create the IP.</param>
    /// <param name="interpreter">The <see cref="Interpreter" /> the IP is associated with.</param>
    public FungeIP(int id, FungeSpace space, Interpreter interpreter)
    {
        Space = space;
        Dim = space.Dim;
        StackStack = new FungeStackStack();
        Functions = new InstructionMap(interpreter.InstructionRegistry.CoreInstructions);
        ID = id;
        for (var i = 0; i < 26; i++) FingerprintStacks[i] = new Stack<FungeInstruction>();
        Interpreter = interpreter;
    }

    /// <summary>
    ///     The IP's dimensionality. This defines how the IP sees the world, and how it pushes and pops
    ///     <see cref="FungeVector" />s on its stack.
    /// </summary>
    public int Dim { get; private set; }

    /// <summary>
    ///     The space the IP currently lives in.
    /// </summary>
    public FungeSpace Space { get; }

    /// <summary>
    ///     The stack-stack associated with the IP.
    /// </summary>
    public FungeStackStack StackStack { get; private init; }

    /// <summary>
    ///     The interpreter this IP is bound to.
    /// </summary>
    public Interpreter Interpreter { get; private init; }

    /// <summary>
    ///     This determines whether the IP is dead or alive. If an IP is dead, it will not execute instructions, and
    ///     it will be removed from the interpreter's IP list at the earliest convenience.
    /// </summary>
    public bool Alive { get; set; } = true;

    /// <summary>
    ///     If this value is set during execution, the interpreter will immediately stop running after the IP's "turn" is
    ///     over.
    /// </summary>
    internal bool RequestQuit { get; set; } = false;

    /// <summary>
    ///     This flags enum determines whether the IP is in any number of modes. See <see cref="IPModes" /> for an overview.
    /// </summary>
    private IPModes Modes { get; set; } = IPModes.None;

    /// <summary>
    ///     Determines whether the IP is in string mode.
    /// </summary>
    /// <remarks>
    ///     In string mode, instead of executing instructions,
    ///     the IP pushes the values of each cell until it encounters a cell with the value &quot;.
    /// </remarks>
    /// <seealso cref="IPModes" />
    public bool StringMode
    {
        get => Modes.HasFlag(IPModes.StringMode);
        set
        {
            if (value)
                Modes |= IPModes.StringMode;
            else
                Modes &= ~IPModes.StringMode;
        }
    }

    /// <summary>
    ///     Determines whether the IP is in hover mode.
    /// </summary>
    /// <remarks>
    ///     In hover mode, the directional instructions &lt;&gt;^vhl and _|m
    ///     all add to the IP's delta instead of setting it.
    /// </remarks>
    /// <seealso cref="IPModes" />
    public bool HoverMode
    {
        get => Modes.HasFlag(IPModes.HoverMode);
        set
        {
            if (value)
                Modes |= IPModes.HoverMode;
            else
                Modes &= ~IPModes.HoverMode;
        }
    }

    /// <summary>
    ///     Determines whether the IP is in invert mode.
    /// </summary>
    /// <remarks>
    ///     In invert mode, the IP always pushes to the bottom of any
    ///     <see cref="FungeStack" />s. This does not affect pushing new stacks to a <see cref="FungeStackStack" />.
    /// </remarks>
    /// <seealso cref="IPModes" />
    public bool InvertMode
    {
        get => Modes.HasFlag(IPModes.InvertMode);
        set
        {
            if (value)
                Modes |= IPModes.InvertMode;
            else
                Modes &= ~IPModes.InvertMode;
        }
    }

    /// <summary>
    ///     Determines whether the IP is in queue mode.
    /// </summary>
    /// <remarks>
    ///     In queue mode, the IP always pops from the bottom of any
    ///     <see cref="FungeStack" />s. This does not affect popping stacks from a <see cref="FungeStackStack" />.
    /// </remarks>
    /// <seealso cref="IPModes" />
    public bool QueueMode
    {
        get => Modes.HasFlag(IPModes.QueueMode);
        set
        {
            if (value)
                Modes |= IPModes.QueueMode;
            else
                Modes &= ~IPModes.QueueMode;
        }
    }

    /// <summary>
    ///     Determines whether the IP is in switch mode.
    /// </summary>
    /// <remarks>
    ///     In switch mode, the instructions [](){} all cause the cell at the
    ///     IP's current position to turn into the opposite bracket in addition to their usual effects.
    ///     For example, with the IP in switch mode, the [ instruction will set the cell at the IP's current position to ]
    ///     and also turn left.
    /// </remarks>
    /// <seealso cref="IPModes" />
    public bool SwitchMode
    {
        get => Modes.HasFlag(IPModes.SwitchMode);
        set
        {
            if (value)
                Modes |= IPModes.SwitchMode;
            else
                Modes &= ~IPModes.SwitchMode;
        }
    }


    /// <summary>
    ///     The registry of instructions available to the interpreter.
    /// </summary>
    private InstructionRegistry InstructionRegistry => Interpreter.InstructionRegistry;

    /// <summary>
    ///     The set of non-fingerprint instructions available to the IP.
    /// </summary>
    private InstructionMap Functions { get; set; }

    /// <summary>
    ///     The fingerprint stacks of the IP. <br />
    ///     Each stack is associated with a value 'A'-'Z'. The top instruction on each
    ///     stack is executed when the IP encounters a command with the corresponding value. <br />
    ///     Here, the stacks are stored in an array, where the index is the value minus 'A'.
    /// </summary>
    /// <seealso cref="LoadFingerprint" />
    /// <seealso cref="UnloadFingerprint" />
    public Stack<FungeInstruction>[] FingerprintStacks { get; } = new Stack<FungeInstruction>[26];

    /// <summary>
    ///     A unique identifier for the IP.
    /// </summary>
    public FungeInt ID { get; }

    /// <summary>
    ///     Toggles the given modes on or off.
    /// </summary>
    /// <param name="modes">The modes to toggle.</param>
    public void ToggleModes(IPModes modes)
    {
        Modes ^= modes;
    }

    /// <summary>
    ///     Executes the operation associated with the given value.
    /// </summary>
    /// <remarks>
    ///     If the value is a letter from 'A' to 'Z', the IP will execute the top instruction on the corresponding
    ///     fingerprint stack. If the stack is empty, the IP will reflect. <br />
    ///     Otherwise, the IP will read its instruction map to find the operation associated with the value. <br />
    ///     If the value is not recognized, the IP will write an error message to the interpreter's error output.
    /// </remarks>
    /// <param name="op">The Funge cell value of the operation to execute.</param>
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
            Reflect();
            Interpreter.WriteError("Unknown command: " + op + " (" + (char)op + ")\n");
        }
    }

    /// <summary>
    ///     Does a full step of execution for this IP. This includes executing the operation at the current position and
    ///     moving the IP to the next position. String mode is handled here.
    /// </summary>
    /// <seealso cref="DoOp" />
    /// <seealso cref="MoveToNext" />
    public void Step()
    {
        if (!Alive) return;
        if (StringMode)
        {
            if (Space[Position] == '"')
                StringMode = false;
            else
                PushToStack(Space[Position]);
        }
        else
        {
            DoOp(Space[Position]);
        }

        MoveToNext();
    }

    /// <summary>
    ///     Loads a fingerprint into the IP's fingerprint stacks. Each stack is only affected if the fingerprint in
    ///     question has an instruction for that stack. <br />
    ///     If the fingerprint is instanced per IP, a new instance will be created if one does not already exist. <br />
    ///     If no fingerprint is found for the given code, an <see cref="ArgumentException" /> is thrown.
    /// </summary>
    /// <seealso cref="UnloadFingerprint" />
    /// <param name="code">The integer code (handprint) associated with the fingerprint.</param>
    /// <exception cref="ArgumentException">Thrown if no fingerprint is found for the given code.</exception>
    /// <exception cref="NotImplementedException">
    ///     Thrown if the fingerprint type is not implemented (currently,
    ///     <see cref="FingerprintType.InstancedPerSpace" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">Thrown if the fingerprint type is invalid.</exception>
    public void LoadFingerprint(FungeInt code)
    {
        var dict = FindFingerprint(code);

        foreach (var pair in dict) FingerprintStacks[pair.Key - 'A'].Push(pair.Value);
    }

    /// <summary>
    ///     Finds the fingerprint associated with the given code and returns its instruction map. <br />
    ///     If the fingerprint is instanced per IP, a new instance will be created if one does not already exist. <br />
    ///     If no fingerprint is found for the given code, an <see cref="ArgumentException" /> is thrown.
    /// </summary>
    /// <param name="code">The integer code (handprint) associated with the fingerprint.</param>
    /// <returns>The instruction map of the fingerprint.</returns>
    /// <exception cref="ArgumentException">Thrown if no fingerprint is found for the given code.</exception>
    /// <exception cref="NotImplementedException">
    ///     Thrown if the fingerprint type is not implemented (currently,
    ///     <see cref="FingerprintType.InstancedPerSpace" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">Thrown if the fingerprint type is invalid.</exception>
    private InstructionMap FindFingerprint(FungeInt code)
    {
        Dictionary<FungeInt, FungeInstruction> dict;
        switch (InstructionRegistry.TypeOf(code))
        {
            case FingerprintType.Static:
                dict = InstructionRegistry.GetStaticFingerprint(code);
                break;
            case FingerprintType.InstancedPerInterpreter:
                dict = Interpreter.Fingerprints[code].Instructions;
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

    /// <summary>
    ///     Unloads a fingerprint from the IP's fingerprint stacks. For each stack, the top instruction is popped if the
    ///     fingerprint in question has an instruction for that stack. <br />
    ///     If the fingerprint is instanced per IP, a new instance will be created if one does not already exist. <br />
    /// </summary>
    /// <param name="code">The integer code (handprint) associated with the fingerprint.</param>
    /// <exception cref="ArgumentException">Thrown if no fingerprint is found for the given code.</exception>
    /// <exception cref="NotImplementedException">
    ///     Thrown if the fingerprint type is not implemented (currently,
    ///     <see cref="FingerprintType.InstancedPerSpace" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">Thrown if the fingerprint type is invalid.</exception>
    public void UnloadFingerprint(FungeInt code)
    {
        var dict = FindFingerprint(code);
        foreach (var pair in dict) FingerprintStacks[pair.Key - 'A'].Pop();
    }

    /// <summary>
    ///     Returns a string representation of the IP.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "IP " + ID + " at " + Position + " with delta " + Delta;
    }

    /// <summary>
    ///     Pushes the given value to the IP's TOSS (top of stack-stack).
    ///     If invert mode is active, the value is pushed to the bottom of the TOSS.
    /// </summary>
    /// <param name="value">The value to push.</param>
    /// <seealso cref="FungeStack.Push" />
    public void PushToStack(FungeInt value)
    {
        StackStack.TOSS.Push(value, InvertMode);
    }

    /// <summary>
    ///     Pops a value from the IP's TOSS (top of stack-stack).
    ///     If queue mode is active, the value is popped from the bottom of the TOSS.
    ///     As with all <see cref="FungeStack" /> operations, an empty stack will pop a zero.
    /// </summary>
    /// <returns>The popped value.</returns>
    /// <seealso cref="FungeStack.Pop" />
    public FungeInt PopFromStack()
    {
        return StackStack.TOSS.Pop(QueueMode);
    }

    /// <summary>
    ///     Pushes the given value to the IP's SOSS (second on stack-stack).
    ///     If invert mode is active, the value is pushed to the bottom of the SOSS.
    /// </summary>
    /// <param name="value">The value to push.</param>
    /// <exception cref="InvalidOperationException">Thrown if the SOSS does not exist.</exception>
    /// <seealso cref="FungeStack.Push" />
    public void PushToSOSS(FungeInt value)
    {
        if (StackStack.SOSS is null) throw new InvalidOperationException("No SOSS to push to.");
        StackStack.SOSS.Push(value, InvertMode);
    }

    /// <summary>
    ///     Pops a value from the IP's SOSS (second on stack-stack).
    ///     If queue mode is active, the value is popped from the bottom of the SOSS.
    ///     As with all <see cref="FungeStack" /> operations, an empty stack will pop a zero.
    /// </summary>
    /// <returns>The popped value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the SOSS does not exist.</exception>
    /// <seealso cref="FungeStack.Pop" />
    public FungeInt PopFromSOSS()
    {
        if (StackStack.SOSS is null) throw new InvalidOperationException("No SOSS to pop from.");
        return StackStack.SOSS.Pop(QueueMode);
    }

    /// <summary>
    ///     Pushes the given vector to the IP's TOSS (top of stack-stack).
    ///     If invert mode is active, the vector is pushed to the bottom of the TOSS, and in reverse order.
    ///     The vector is treated as having the IP's dimensionality, so any components beyond the IP's dimensionality are
    ///     ignored.
    /// </summary>
    /// <param name="vector">The vector to push.</param>
    /// <seealso cref="FungeStack.PushVector" />
    public void PushVectorToStack(FungeVector vector)
    {
        StackStack.TOSS.PushVector(vector, Dim, InvertMode);
    }

    /// <summary>
    ///     Pops a vector from the IP's TOSS (top of stack-stack).
    ///     If queue mode is active, the vector is popped from the bottom of the TOSS, and in reverse order.
    ///     As with all <see cref="FungeStack" /> operations, an empty stack will pop a zero vector.
    /// </summary>
    /// <returns>The popped vector, of the IP's dimensionality.</returns>
    /// <seealso cref="FungeStack.PopVector" />
    public FungeVector PopVectorFromStack()
    {
        return StackStack.TOSS.PopVector(Dim, QueueMode);
    }

    /// <summary>
    ///     Pushes the given vector to the IP's SOSS (second on stack-stack).
    ///     If invert mode is active, the vector is pushed to the bottom of the SOSS, and in reverse order.
    ///     The vector is treated as having the IP's dimensionality, so any components beyond the IP's dimensionality are
    ///     ignored.
    /// </summary>
    /// <param name="vector">The vector to push.</param>
    /// <exception cref="InvalidOperationException">Thrown if the SOSS does not exist.</exception>
    /// <seealso cref="FungeStack.PushVector" />
    public void PushVectorToSOSS(FungeVector vector)
    {
        if (StackStack.SOSS is null) throw new InvalidOperationException("No SOSS to pop from.");
        StackStack.SOSS.PushVector(vector, Dim, InvertMode);
    }

    /// <summary>
    ///     Pops a vector from the IP's SOSS (second on stack-stack).
    ///     If queue mode is active, the vector is popped from the bottom of the SOSS, and in reverse order.
    ///     As with all <see cref="FungeStack" /> operations, an empty stack will pop a zero vector.
    /// </summary>
    /// <returns>The popped vector, of the IP's dimensionality.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the SOSS does not exist.</exception>
    /// <seealso cref="FungeStack.PopVector" />
    public FungeVector PopVectorFromSOSS()
    {
        if (StackStack.SOSS is null) throw new InvalidOperationException("No SOSS to pop from.");
        return StackStack.SOSS.PopVector(Dim, QueueMode);
    }

    /// <summary>
    ///     Pushes the given string to the IP's TOSS (top of stack-stack).
    ///     If invert mode is active, the string is pushed to the bottom of the TOSS, and in reverse order.
    ///     A null terminator (ASCII 0) is pushed after the string.
    /// </summary>
    /// <param name="str">The string to push.</param>
    /// <seealso cref="FungeStack.PushString" />
    public void PushStringToStack(FungeString str)
    {
        StackStack.TOSS.PushString(str, InvertMode);
    }

    /// <summary>
    ///     Pops a string from the IP's TOSS (top of stack-stack).
    ///     If queue mode is active, the string is popped from the bottom of the TOSS, and in reverse order.
    ///     Characters are popped until a null terminator (ASCII 0) is encountered.
    ///     As with all <see cref="FungeStack" /> operations, an empty stack will pop a zero immediately,
    ///     thereby returning an empty string.
    /// </summary>
    /// <returns>The popped string.</returns>
    /// <seealso cref="FungeStack.PopString" />
    public FungeString PopStringFromStack()
    {
        return StackStack.TOSS.PopString(QueueMode);
    }

    /// <summary>
    ///     Pushes the given string to the IP's SOSS (second on stack-stack).
    ///     If invert mode is active, the string is pushed to the bottom of the SOSS, and in reverse order.
    ///     A null terminator (ASCII 0) is pushed after the string.
    /// </summary>
    /// <param name="str">The string to push.</param>
    /// <exception cref="InvalidOperationException">Thrown if the SOSS does not exist.</exception>
    /// <seealso cref="FungeStack.PushString" />
    public void PushStringToSOSS(FungeString str)
    {
        if (StackStack.SOSS is null) throw new InvalidOperationException("No SOSS to pop from.");
        StackStack.SOSS.PushString(str, InvertMode);
    }

    /// <summary>
    ///     Pops a string from the IP's SOSS (second on stack-stack).
    ///     If queue mode is active, the string is popped from the bottom of the SOSS, and in reverse order.
    ///     Characters are popped until a null terminator (ASCII 0) is encountered.
    ///     As with all <see cref="FungeStack" /> operations, an empty stack will pop a zero immediately,
    ///     thereby returning an empty string.
    /// </summary>
    /// <returns>The popped string.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the SOSS does not exist.</exception>
    /// <seealso cref="FungeStack.PopString" />
    public FungeString PopStringFromSOSS()
    {
        if (StackStack.SOSS is null) throw new InvalidOperationException("No SOSS to pop from.");
        return StackStack.SOSS.PopString(QueueMode);
    }

    /// <summary>
    ///     Reflects the IP's delta. Each component of the delta is negated.
    /// </summary>
    internal void Reflect()
    {
        Delta = -Delta;
    }

    /// <summary>
    ///     Moves a vector to the next position using the IP's delta, wrapping around the space if necessary.
    /// </summary>
    /// <seealso cref="FungeSpace.Wrap" />
    /// <param name="position">The starting position.</param>
    /// <returns>The new position.</returns>
    private FungeVector MoveAndWrap(FungeVector position)
    {
        position += Delta;
        if (Space.OutOfBounds(position)) position = Space.Wrap(position, Delta);
        return position;
    }

    /// <summary>
    ///     Determines the next position of the IP, skipping spaces and comments as necessary, as well as wrapping.
    ///     If the IP is in string mode and does not start on a space, it will not skip spaces, providing SGML-style strings.
    /// </summary>
    /// <returns>The next position.</returns>
    public FungeVector NextPosition()
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

    /// <summary>
    ///     Moves the IP to the next position.
    /// </summary>
    /// <seealso cref="NextPosition" />
    public void MoveToNext()
    {
        Position = NextPosition();
    }

    /// <summary>
    ///     Puts the given value at the given position in the IP's space.
    ///     The position is adjusted by the IP's storage offset.
    /// </summary>
    /// <param name="pos">The position to write to.</param>
    /// <param name="value">The value to write.</param>
    public void Put(FungeVector pos, FungeInt value)
    {
        Space[pos + StorageOffset] = value;
    }

    /// <summary>
    ///     Gets the value at the given position in the IP's space.
    ///     The position is adjusted by the IP's storage offset.
    /// </summary>
    /// <param name="pos">The position to read from.</param>
    /// <returns>The value at the given position.</returns>
    public FungeInt Get(FungeVector pos)
    {
        return Space[pos + StorageOffset];
    }

    /// <summary>
    ///     Reads the contents of a file into the IP's space at the given position.
    ///     This is affected by the IP's storage offset, and is otherwise a wrapper around <see cref="FungeSpace.LoadFile" />.
    /// </summary>
    /// <param name="pos">The position to read into.</param>
    /// <param name="path">The path to the file to read.</param>
    /// <param name="binary">Whether to read the file in binary mode.</param>
    /// <returns>The size of the rectangle/cuboid of space affected by the read.</returns>
    /// <exception cref="FungeReflectException">Thrown if any errors occur during the read.</exception>
    public FungeVector ReadFileIntoSpace(FungeVector pos, string path, bool binary = false)
    {
        try
        {
            return Space.LoadFile(pos + StorageOffset, path, binary);
        }
        catch (Exception)
        {
            throw new FungeReflectException();
        }
    }

    /// <summary>
    ///     Writes the contents of the IP's space to a file at the given position.
    ///     This is affected by the IP's storage offset, and is otherwise a wrapper around
    ///     <see cref="FungeSpace.WriteToFile" />.
    /// </summary>
    /// <param name="pos">The position to write from.</param>
    /// <param name="size">The size of the rectangle/cuboid of space to write.</param>
    /// <param name="filename">The path to the file to write.</param>
    /// <param name="linear">
    ///     Whether to write in linear mode, i.e. not writing spaces before newlines,
    ///     newlines before line feeds, and line feeds before file end.
    /// </param>
    /// <exception cref="FungeReflectException">Thrown if any errors occur during the write.</exception>
    public void WriteSpaceToFile(FungeVector pos, FungeVector size, FungeString filename, bool linear = false)
    {
        try
        {
            Space.WriteToFile(pos + StorageOffset, size, filename, linear);
        }
        catch (Exception)
        {
            throw new FungeReflectException();
        }
    }

    /// <summary>
    ///     Creates a new IP with nearly the same state as this one.
    ///     The new IP will have its delta negated, and will be placed at the current position plus this negated delta.
    ///     It will have a deep copy of the stack-stack, as well as the same dimensionality and storage offset.
    ///     The fingerprint stacks are also copied, respecting instanced fingerprints.
    /// </summary>
    /// <param name="id">The ID of the new IP.</param>
    /// <returns>The new IP.</returns>
    public FungeIP Split(int id)
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
            var stack = FingerprintStacks[i];
            foreach (var instruction in stack)
            {
                var code = instruction.SourceFingerprintCode;
                if (code is not null)
                    // Instanced fingerprints are not necessarily shared between IPs, so we need to create new instances
                    // when splitting an IP.
                    // Hence, we use the registry to find corresponding fingerprint instructions for the new IP.
                    newIP.FingerprintStacks[i].Push(newIP.FindFingerprint(code.Value)[i + 'A']);
            }
        }

        newIP.MoveToNext();
        return newIP;
    }

    public void Freeze()
    {
        Frozen = true;
    }

    public bool Frozen { get; set; }

    public void Unfreeze()
    {
        Frozen = false;
    }
}