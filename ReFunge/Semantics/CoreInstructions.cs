using System.Collections;
using ReFunge.Data;
using ReFunge.Data.Values;
using ReFunge.Semantics.Fingerprints.Core;

namespace ReFunge.Semantics;

/// <summary>
///     Contains the core instructions of the Funge-98 language. These instructions are always available in any Funge
///     program.
/// </summary>
public static class CoreInstructions
{
    /// <summary>
    ///     Pushes the number 0 to the stack.
    /// </summary>
    [Instruction('0')] public static readonly FungeFunc Push0 = PushNumber(0);

    /// <summary>
    ///     Pushes the number 1 to the stack.
    /// </summary>
    [Instruction('1')] public static readonly FungeFunc Push1 = PushNumber(1);

    /// <summary>
    ///     Pushes the number 2 to the stack.
    /// </summary>
    [Instruction('2')] public static readonly FungeFunc Push2 = PushNumber(2);

    /// <summary>
    ///     Pushes the number 3 to the stack.
    /// </summary>
    [Instruction('3')] public static readonly FungeFunc Push3 = PushNumber(3);

    /// <summary>
    ///     Pushes the number 4 to the stack.
    /// </summary>
    [Instruction('4')] public static readonly FungeFunc Push4 = PushNumber(4);

    /// <summary>
    ///     Pushes the number 5 to the stack.
    /// </summary>
    [Instruction('5')] public static readonly FungeFunc Push5 = PushNumber(5);

    /// <summary>
    ///     Pushes the number 6 to the stack.
    /// </summary>
    [Instruction('6')] public static readonly FungeFunc Push6 = PushNumber(6);

    /// <summary>
    ///     Pushes the number 7 to the stack.
    /// </summary>
    [Instruction('7')] public static readonly FungeFunc Push7 = PushNumber(7);

    /// <summary>
    ///     Pushes the number 8 to the stack.
    /// </summary>
    [Instruction('8')] public static readonly FungeFunc Push8 = PushNumber(8);

    /// <summary>
    ///     Pushes the number 9 to the stack.
    /// </summary>
    [Instruction('9')] public static readonly FungeFunc Push9 = PushNumber(9);

    /// <summary>
    ///     Pushes the number 10 to the stack.
    /// </summary>
    [Instruction('a')] public static readonly FungeFunc Push10 = PushNumber(10);

    /// <summary>
    ///     Pushes the number 11 to the stack.
    /// </summary>
    [Instruction('b')] public static readonly FungeFunc Push11 = PushNumber(11);

    /// <summary>
    ///     Pushes the number 12 to the stack.
    /// </summary>
    [Instruction('c')] public static readonly FungeFunc Push12 = PushNumber(12);

    /// <summary>
    ///     Pushes the number 13 to the stack.
    /// </summary>
    [Instruction('d')] public static readonly FungeFunc Push13 = PushNumber(13);

    /// <summary>
    ///     Pushes the number 14 to the stack.
    /// </summary>
    [Instruction('e')] public static readonly FungeFunc Push14 = PushNumber(14);

    /// <summary>
    ///     Pushes the number 15 to the stack.
    /// </summary>
    [Instruction('f')] public static readonly FungeFunc Push15 = PushNumber(15);

    /// <summary>
    ///     Add two integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the addition.</returns>
    [Instruction('+')]
    public static FungeInt Add(FungeIP _, FungeInt a, FungeInt b)
    {
        return a + b;
    }

    /// <summary>
    ///     Subtract two integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The minuend.</param>
    /// <param name="b">The subtrahend.</param>
    /// <returns>The result of the subtraction.</returns>
    [Instruction('-')]
    public static FungeInt Subtract(FungeIP _, FungeInt a, FungeInt b)
    {
        return a - b;
    }

    /// <summary>
    ///     Multiply two integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the multiplication.</returns>
    [Instruction('*')]
    public static FungeInt Multiply(FungeIP _, FungeInt a, FungeInt b)
    {
        return a * b;
    }

    /// <summary>
    ///     Divide two integers. If the divisor is 0, the result is 0.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The result of the division, or 0 if the divisor is 0.</returns>
    [Instruction('/')]
    public static FungeInt Divide(FungeIP _, FungeInt a, FungeInt b)
    {
        return b == 0 ? 0 : a / b;
    }

    /// <summary>
    ///     Modulo operation on two integers. If the divisor is 0, the result is 0.
    ///     This functions like the remainder operation in C#, with the sign of the result matching the dividend.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The result of the modulo operation, or 0 if the divisor is 0.</returns>
    [Instruction('%')]
    public static FungeInt Modulo(FungeIP _, FungeInt a, FungeInt b)
    {
        return b == 0 ? 0 : a % b;
    }

    /// <summary>
    ///     Greater-than comparison of two integers. Returns 1 if the first operand is greater than the second, 0 otherwise.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the comparison.</returns>
    [Instruction('`')]
    public static FungeInt GreaterThan(FungeIP _, FungeInt a, FungeInt b)
    {
        return a > b ? 1 : 0;
    }

    /// <summary>
    ///     Logical NOT operation on an integer. Returns 1 if the operand is 0, 0 otherwise.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="v">The operand.</param>
    /// <returns>The result of the logical NOT operation.</returns>
    [Instruction('!')]
    public static FungeInt LogicalNot(FungeIP _, FungeInt v)
    {
        return v == 0 ? 1 : 0;
    }

    /// <summary>
    ///     Set the IP's delta to a rightward direction.
    ///     If the IP is in hover mode, the new delta is added to the current delta instead of replacing it.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('>')]
    public static void GoRight(FungeIP ip)
    {
        if (ip.HoverMode)
            ip.Delta += FungeVector.Right;
        else
            ip.Delta = FungeVector.Right;
    }

    /// <summary>
    ///     Set the IP's delta to a leftward direction.
    ///     If the IP is in hover mode, the new delta is added to the current delta instead of replacing it.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('<')]
    public static void GoLeft(FungeIP ip)
    {
        if (ip.HoverMode)
            ip.Delta += FungeVector.Left;
        else
            ip.Delta = FungeVector.Left;
    }

    /// <summary>
    ///     Set the IP's delta to an upward direction. Reflects if the IP is in 1D. <br />
    ///     If the IP is in hover mode, the new delta is added to the current delta instead of replacing it.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('^', 2)]
    public static void GoUp(FungeIP ip)
    {
        if (ip.HoverMode)
            ip.Delta += FungeVector.Up;
        else
            ip.Delta = FungeVector.Up;
    }

    /// <summary>
    ///     Set the IP's delta to a downward direction. Reflects if the IP is in 1D. <br />
    ///     If the IP is in hover mode, the new delta is added to the current delta instead of replacing it.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('v', 2)]
    public static void GoDown(FungeIP ip)
    {
        if (ip.HoverMode)
            ip.Delta += FungeVector.Down;
        else
            ip.Delta = FungeVector.Down;
    }

    /// <summary>
    ///     Set the IP's delta to a forwards direction (positive Z). Reflects if the IP is in 1D or 2D. <br />
    ///     If the IP is in hover mode, the new delta is added to the current delta instead of replacing it.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('h', 3)]
    public static void GoForwards(FungeIP ip)
    {
        if (ip.HoverMode)
            ip.Delta += FungeVector.Forwards;
        else
            ip.Delta = FungeVector.Forwards;
    }

    /// <summary>
    ///     Set the IP's delta to a backwards direction (negative Z). Reflects if the IP is in 1D or 2D. <br />
    ///     If the IP is in hover mode, the new delta is added to the current delta instead of replacing it.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('l', 3)]
    public static void GoBackwards(FungeIP ip)
    {
        if (ip.HoverMode)
            ip.Delta += FungeVector.Backwards;
        else
            ip.Delta = FungeVector.Backwards;
    }

    /// <summary>
    ///     Takes a value from the stack. If it is 0, the IP moves right; otherwise, it moves left.
    /// </summary>
    /// <seealso cref="GoRight" />
    /// <seealso cref="GoLeft" />
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="v">The value to check.</param>
    [Instruction('_')]
    public static void DecideHorizontal(FungeIP ip, FungeInt v)
    {
        if (v == 0)
            GoRight(ip);
        else
            GoLeft(ip);
    }

    /// <summary>
    ///     Takes a value from the stack. If it is 0, the IP moves down; otherwise, it moves up.
    /// </summary>
    /// <seealso cref="GoDown" />
    /// <seealso cref="GoUp" />
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="v">The value to check.</param>
    [Instruction('|', 2)]
    public static void DecideVertical(FungeIP ip, FungeInt v)
    {
        if (v == 0)
            GoDown(ip);
        else
            GoUp(ip);
    }

    /// <summary>
    ///     Takes a value from the stack. If it is 0, the IP moves forwards; otherwise, it moves backwards.
    /// </summary>
    /// <seealso cref="GoForwards" />
    /// <seealso cref="GoBackwards" />
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="v">The value to check.</param>
    [Instruction('m', 3)]
    public static void DecideForwards(FungeIP ip, FungeInt v)
    {
        if (v == 0)
            GoBackwards(ip);
        else
            GoForwards(ip);
    }

    /// <summary>
    ///     Moves the IP in a random cardinal direction. The direction is chosen randomly from the available directions,
    ///     determined by the number of dimensions the IP sees.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('?')]
    public static void GoRandom(FungeIP ip)
    {
        ip.Delta = FungeVector.Cardinal(Random.Shared.Next(ip.Dim), Random.Shared.Next(2) * 2 - 1);
    }

    /// <summary>
    ///     Reflects the IP's delta, such that the IP moves in the opposite direction.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('r')]
    public static void Reflect(FungeIP ip)
    {
        ip.Reflect();
    }

    /// <summary>
    ///     Turns the IP's delta 90 degrees to the right, around the Z axis. Reflects if the IP is in 1D. <br />
    ///     If the IP is in switch mode, a '[' is placed at the current position.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction(']', 2)]
    public static void TurnRight(FungeIP ip)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = '[';
        ip.Delta = ip.Delta.SetCoordinate(0, -ip.Delta[1]).SetCoordinate(1, ip.Delta[0]);
    }

    /// <summary>
    ///     Turns the IP's delta 90 degrees to the left, around the Z axis. Reflects if the IP is in 1D. <br />
    ///     If the IP is in switch mode, a ']' is placed at the current position.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('[', 2)]
    public static void TurnLeft(FungeIP ip)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = ']';
        ip.Delta = ip.Delta.SetCoordinate(0, ip.Delta[1]).SetCoordinate(1, -ip.Delta[0]);
    }

    /// <summary>
    ///     Compares two integers. If the first is less than the second, the IP turns left; if the first is greater, it turns
    ///     right.
    /// </summary>
    /// <seealso cref="TurnLeft" />
    /// <seealso cref="TurnRight" />
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    [Instruction('w', 2)]
    public static void Compare(FungeIP ip, FungeInt a, FungeInt b)
    {
        if (a < b)
            TurnLeft(ip);
        else if (a > b) TurnRight(ip);
    }

    /// <summary>
    ///     Duplicates the top value on the stack, pushing it twice.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="v">The value to duplicate.</param>
    [Instruction(':')]
    public static void Duplicate(FungeIP ip, FungeInt v)
    {
        ip.PushToStack(v);
        ip.PushToStack(v);
    }

    /// <summary>
    ///     Discards the top value on the stack.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="v">The value to discard.</param>
    [Instruction('$')]
    public static void Discard(FungeIP ip, FungeInt v)
    {
    }

    /// <summary>
    ///     Swaps the top two values on the stack, pushing them in reverse order.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="a">The bottom value to swap.</param>
    /// <param name="b">The top value to swap.</param>
    [Instruction('\\')]
    public static void Swap(FungeIP ip, FungeInt a, FungeInt b)
    {
        ip.PushToStack(b);
        ip.PushToStack(a);
    }

    /// <summary>
    ///     Clears the IP's TOSS (top of stack stack), emptying it.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('n')]
    public static void ClearStack(FungeIP ip)
    {
        ip.StackStack.TOSS.Clear();
    }

    /// <summary>
    ///     Returns a <see cref="FungeFunc" /> that pushes a given number to the stack. Used to implement the "Push Number"
    ///     instructions.
    /// </summary>
    /// <seealso cref="Push0" />
    /// <seealso cref="ROMA" />
    /// <param name="number">The number to push.</param>
    /// <returns>A function that pushes the given number to the stack.</returns>
    public static FungeFunc PushNumber(int number)
    {
        return new FungeAction(ip => ip.PushToStack(number));
    }

    /// <summary>
    ///     Toggles string mode. While in string mode, the IP reads characters verbatim instead of executing them, until
    ///     another '"' is encountered.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('"')]
    public static void ToggleStringMode(FungeIP ip)
    {
        ip.ToggleModes(IPModes.StringMode);
    }

    /// <summary>
    ///     Adds the IP's delta to its position, then returns the value in Funge-Space at that position.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <returns>The value in Funge-Space at the new position.</returns>
    [Instruction('\'')]
    public static FungeInt FetchCharacter(FungeIP ip)
    {
        ip.Position += ip.Delta;
        return ip.Space[ip.Position];
    }

    /// <summary>
    ///     Adds the IP's delta to its position, then stores a value in Funge-Space at that position.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="value">The value to store.</param>
    [Instruction('s')]
    public static void Store(FungeIP ip, FungeInt value)
    {
        ip.Position += ip.Delta;
        ip.Space[ip.Position] = value;
    }

    /// <summary>
    ///     Adds the IP's delta to its position, in effect skipping the next cell in its path.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('#')]
    public static void Skip(FungeIP ip)
    {
        ip.Position += ip.Delta;
    }

    /// <summary>
    ///     Get a value from Funge-Space at a given position. This is affected by the IP's storage offset.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="pos">The position to get the value from.</param>
    /// <returns>The value at the given position.</returns>
    [Instruction('g')]
    public static FungeInt Get(FungeIP ip, FungeVector pos)
    {
        return ip.Get(pos);
    }

    /// <summary>
    ///     Put a value into Funge-Space at a given position. This is affected by the IP's storage offset.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="v">The value to put.</param>
    /// <param name="pos">The position to put the value at.</param>
    [Instruction('p')]
    public static void Put(FungeIP ip, FungeInt v, FungeVector pos)
    {
        ip.Put(pos, v);
    }

    /// <summary>
    ///     Get the (ASCII) value of a character from the input stream.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <returns>The value of the character read from the input stream.</returns>
    /// <exception cref="FungeReflectException">Thrown if the end of the input stream has been reached.</exception>
    [Instruction('~')]
    public static FungeInt Input(FungeIP ip)
    {
        if (ip.Interpreter.EndOfInput())
            throw new FungeReflectException(new InvalidOperationException("End of input reached."));

        return ip.Interpreter.ReadCharacter();
    }

    /// <summary>
    ///     Output a character to the output stream.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="v">The ASCII value of the character to output.</param>
    [Instruction(',')]
    public static void Output(FungeIP ip, FungeInt v)
    {
        ip.Interpreter.WriteCharacter((char)v);
    }

    /// <summary>
    ///     Get an integer from the input stream, reading characters until a non-digit character is encountered.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <returns>The integer read from the input stream.</returns>
    /// <exception cref="FungeReflectException">Thrown if the end of the input stream has been reached.</exception>
    [Instruction('&')]
    public static FungeInt InputInteger(FungeIP ip)
    {
        if (ip.Interpreter.EndOfInput())
            throw new FungeReflectException(new InvalidOperationException("End of input reached."));

        return ip.Interpreter.ReadInteger();
    }

    /// <summary>
    ///     Output an integer to the output stream. The integer is written as a string of digits followed by a space.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="v">The integer to output.</param>
    [Instruction('.')]
    public static void OutputInteger(FungeIP ip, FungeInt v)
    {
        ip.Interpreter.WriteInteger(v);
    }

    /// <summary>
    ///     Read the contents of a file into Funge-Space at a given position. This calls
    ///     <see cref="FungeIP.ReadFileIntoSpace" />.
    ///     Pushes the size of the bounding box of space affected by the read, followed by the starting position of the read.
    /// </summary>
    /// <seealso cref="FungeIP.ReadFileIntoSpace" />
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="pos">The position to read the file into.</param>
    /// <param name="flags">
    ///     Flags for reading the file. The only flag is the least significant bit, which is 1 if the file
    ///     should be read in binary mode.
    /// </param>
    /// <param name="filename">The name of the file to read.</param>
    [Instruction('i')]
    public static void InputFile(FungeIP ip, FungeVector pos, FungeInt flags, FungeString filename)
    {
        var size = ip.ReadFileIntoSpace(pos, filename, (flags & 1) != 0);
        ip.PushVectorToStack(size);
        ip.PushVectorToStack(pos);
    }

    /// <summary>
    ///     Write the contents of Funge-Space to a file. This calls <see cref="FungeIP.WriteSpaceToFile" />.
    /// </summary>
    /// <seealso cref="FungeIP.WriteSpaceToFile" />
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="size">The size of the data to write.</param>
    /// <param name="pos">The position to write the data from.</param>
    /// <param name="flags">
    ///     Flags for writing the file. The only flag is the least significant bit, which is 1 if the file
    ///     should be written in linear mode.
    /// </param>
    /// <param name="filename">The name of the file to write to.</param>
    [Instruction('o')]
    public static void OutputFile(FungeIP ip, FungeVector size, FungeVector pos, FungeInt flags, FungeString filename)
    {
        ip.WriteSpaceToFile(pos, size, filename, (flags & 1) != 0);
    }

    /// <summary>
    ///     Get information about the system and the current state of the IP. This information is pushed to the stack.
    ///     See the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/doc/funge98.markdown)
    ///     for details on the information pushed.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="n">
    ///     If this is positive, all but the nth cell of the pushed information is discarded.
    ///     If it is greater than the total number of cells pushed, this can act as a pick instruction.
    /// </param>
    [Instruction('y')]
    public static void SysInfo(FungeIP ip, FungeInt n)
    {
        var tempStack = new FungeStack();

        // Environment variables
        tempStack.Push(0);
        var env = Environment.GetEnvironmentVariables();
        foreach (DictionaryEntry de in env) tempStack.PushString(de.Key + "=" + de.Value);

        // Command line arguments
        tempStack.Push(0);
        tempStack.Push(0);
        var args = Environment.GetCommandLineArgs();
        foreach (var arg in args) tempStack.PushString(arg);

        // Sizes of all stacks
        for (var i = ip.StackStack.Size - 1; i >= 0; i--) tempStack.Push(ip.StackStack[i].Size);

        // Size of stack stack
        tempStack.Push(ip.StackStack.Size);

        // Current time (hours * 256 * 256 + minutes * 256 + seconds)
        var now = DateTime.Now;
        tempStack.Push(now.Hour * 256 * 256 + now.Minute * 256 + now.Second);

        // Current date ((year - 1900) * 256 * 256 + month * 256 + day)
        tempStack.Push((now.Year - 1900) * 256 * 256 + now.Month * 256 + now.Day);

        // Upper bound of Funge-Space, relative to the lower bound
        tempStack.PushVector(ip.Space.MaxBounds - ip.Space.MinBounds, ip.Dim);
        // Lower bound of Funge-Space
        tempStack.PushVector(ip.Space.MinBounds, ip.Dim);

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
        var version = typeof(CoreInstructions).Assembly.GetName().Version!;
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
            ip.PushToStack(ip.StackStack.TOSS[n - tempStack.Size - 1]);
        else if (n > 0)
            ip.PushToStack(tempStack[n - 1]);
        else
            for (var i = tempStack.Size - 1; i >= 0; i--)
                ip.PushToStack(tempStack[i]);
    }

    /// <summary>
    ///     Adds the IP's delta to its position a specified number of times, moving it forward or backward.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="v">The number of times to move the IP. If negative, the IP moves backward.</param>
    [Instruction('j')]
    public static void JumpForward(FungeIP ip, FungeInt v)
    {
        ip.Position += ip.Delta * v;
    }

    /// <summary>
    ///     Set the IP's delta to a given vector.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="d">The new delta vector.</param>
    [Instruction('x')]
    public static void SetDelta(FungeIP ip, FungeVector d)
    {
        ip.Delta = d;
    }

    /// <summary>
    ///     Executes the next instruction in the IP's path - factoring in spaces and wrapping - a specified number of times.
    ///     If the number of iterations is 0 or negative, the IP moves to the location of this instruction, causing it
    ///     to not be executed.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="n">The number of times to execute the next instruction.</param>
    [Instruction('k')]
    public static void Iterate(FungeIP ip, FungeInt n)
    {
        if (n <= 0) ip.MoveToNext();

        var op = ip.Space[ip.NextPosition()];
        for (var i = 0; i < n; i++) ip.DoOp(op);
    }

    /// <summary>
    ///     Kills the IP, stopping execution.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('@')]
    public static void Terminate(FungeIP ip)
    {
        ip.Alive = false;
    }

    /// <summary>
    ///     Quits the program, causing the interpreter to return the given return value.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="r">The return value to return to the interpreter.</param>
    [Instruction('q')]
    public static void Quit(FungeIP ip, FungeInt r)
    {
        ip.Interpreter.ReturnValue = r;
        ip.RequestQuit = true;
    }

    /// <summary>
    ///     Creates a new stack on the IP's stack stack, transferring the specified number of values to it from the current
    ///     stack. <br />
    ///     If the number is positive, the values are popped from the current stack and pushed to the new stack. <br />
    ///     If the number is negative, an amount of zeros equal to the absolute value of the number is pushed to the old stack.
    ///     <br />
    ///     The IP's storage offset is pushed to the old stack, and set to the current position plus the delta. <br />
    ///     If the IP is in switch mode, a '}' is placed at the current position.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="n">The number of values to transfer to the new stack.</param>
    [Instruction('{')]
    public static void BeginBlock(FungeIP ip, FungeInt n)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = '}';
        ip.StackStack.NewStack();
        if (n > 0)
        {
            var newStack = new int[n];
            for (var i = 0; i < n; i++) newStack[i] = ip.PopFromSOSS();

            for (var i = n - 1; i >= 0; i--) ip.PushToStack(newStack[i]);
        }

        if (n < 0)
            for (var i = 0; i > n; i--)
                ip.PushToSOSS(0);

        ip.PushVectorToSOSS(ip.StorageOffset);
        ip.StorageOffset = ip.Position + ip.Delta;
    }

    /// <summary>
    ///     Removes the top stack from the IP's stack stack, transferring the specified number of values to the new top stack.
    ///     <br />
    ///     If the number is positive, the values are popped from the old top stack and pushed to the new top stack. <br />
    ///     If the number is negative, an amount of values equal to the absolute value of the number is popped from the old top
    ///     stack. <br />
    ///     The IP's storage offset is set to a vector popped from the new top stack. <br />
    ///     If the IP is in switch mode, a '{' is placed at the current position.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="n">The number of values to transfer to the new top stack.</param>
    /// <exception cref="FungeReflectException">Thrown if there is only one stack on the stack stack.</exception>
    [Instruction('}')]
    public static void EndBlock(FungeIP ip, FungeInt n)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = '{';
        if (ip.StackStack.Size == 1) throw new FungeReflectException(new InvalidOperationException("}: No SOSS."));

        ip.StorageOffset = ip.PopVectorFromSOSS();
        if (n > 0)
        {
            var transfer = new int[n];
            for (var i = 0; i < n; i++) transfer[i] = ip.PopFromStack();

            for (var i = n - 1; i >= 0; i--) ip.PushToSOSS(transfer[i]);
        }

        for (var i = 0; i > n; i--) ip.PopFromSOSS();

        ip.StackStack.RemoveStack();
    }

    /// <summary>
    ///     Transfers values between the IP's TOSS (top of stack stack) and SOSS (second on stack stack). <br />
    ///     If the count is positive, values are popped from the TOSS and pushed to the SOSS. <br />
    ///     If the count is negative, values are popped from the SOSS and pushed to the TOSS. <br />
    ///     If the count is 0, no values are transferred.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="count">The number of values to transfer.</param>
    /// <exception cref="FungeReflectException">Thrown if there is only one stack on the stack stack.</exception>
    [Instruction('u')]
    public static void StackTransfer(FungeIP ip, FungeInt count)
    {
        if (ip.StackStack.Size == 1) throw new FungeReflectException(new InvalidOperationException("u: No SOSS."));

        for (var i = 0; i < count; i++) ip.PushToStack(ip.PopFromSOSS());

        for (var i = 0; i > count; i--) ip.PushToSOSS(ip.PopFromStack());
    }

    /// <summary>
    ///     Does nothing.
    /// </summary>
    /// <param name="_">The IP to do nothing with.</param>
    [Instruction('z')]
    public static void DoNothing(FungeIP _)
    {
    }

    /// <summary>
    ///     Splits the IP, creating a child IP moving in the opposite direction, which is added to the interpreter such
    ///     that it will execute just before the current IP's next turn. The child IP inherits most other properties from the
    ///     parent.
    ///     For details, see <see cref="FungeIP.Split" />.
    /// </summary>
    /// <seealso cref="FungeIP.Split" />
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('t')]
    public static void Split(FungeIP ip)
    {
        var newIP = ip.Split(ip.Interpreter.IPID++);
        ip.Interpreter.AddNewIP(newIP, ip);
    }

    /// <summary>
    ///     Pops a given amount of values from the stack, constructing a fingerprint code from them,
    ///     then loads the fingerprint with that code. <br />
    ///     If the IP is in switch mode, a ')' is placed at the current position. <br />
    ///     For details, see <see cref="FungeIP.LoadFingerprint" />.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="n">The number of values to pop from the stack.</param>
    /// <exception cref="FungeReflectException">Thrown if the fingerprint is not found.</exception>
    [Instruction('(')]
    public static void LoadFingerprint(FungeIP ip, FungeInt n)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = ')';
        var code = 0;
        for (var i = 0; i < n; i++) code = code * 256 + ip.PopFromStack();

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

    /// <summary>
    ///     Pops a given amount of values from the stack, constructing a fingerprint code from them,
    ///     then unloads the fingerprint with that code. <br />
    ///     If the IP is in switch mode, a '(' is placed at the current position. <br />
    ///     For details, see <see cref="FungeIP.UnloadFingerprint" />.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="n">The number of values to pop from the stack.</param>
    /// <exception cref="FungeReflectException">Thrown if the fingerprint is not found.</exception>
    [Instruction(')')]
    public static void UnloadFingerprint(FungeIP ip, FungeInt n)
    {
        if (ip.SwitchMode) ip.Space[ip.Position] = '(';
        var code = 0;
        for (var i = 0; i < n; i++) code = code * 256 + ip.PopFromStack();

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