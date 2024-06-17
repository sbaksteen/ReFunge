using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.Core;

/// <summary>
///     TOYS: Fun extra instructions, many of which deal with mass modification of Funge-space.
///     From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/TOYS.markdown)
/// </summary>
/// <remarks>
///     Several instructions in this fingerprint refer to the concept of a "low-order" and "high-order" iteration over
///     Funge-Space. In a low-order iteration, the first coordinate is iterated over first, followed by the second, and so
///     on, and each coordinate starts at the minimum value and goes to the maximum value. In two dimensions, this is
///     a left-to-right, top-to-bottom iteration. In a high-order iteration, the last coordinate is iterated over first,
///     followed by the second-to-last, and so on, and each coordinate starts at the maximum value and goes to the minimum
///     value. In two dimensions, this is a bottom-to-top, right-to-left iteration.
/// </remarks>
[Fingerprint("TOYS")]
public static class TOYS
{
    private static void CheckBounds(int dim, FungeVector vector)
    {
        for (var i = 0; i < dim; i++)
            if (vector[i] <= 0)
                throw new FungeReflectException(
                    new ArgumentOutOfRangeException(nameof(vector), "Size must be positive"));
    }

    private static void IterateSpace(FungeIP ip, FungeVector source, FungeVector size, FungeVector destination,
        Action<FungeIP, FungeVector, FungeVector> action, bool highOrder = false)
    {
        var current = (highOrder ? source + size : source).ToArray(ip.Dim);
        var end = (highOrder ? destination : destination + size).ToArray(ip.Dim);
        var dest = (highOrder ? destination + size : destination).ToArray(ip.Dim);
        if (highOrder)
            for (var i = 0; i < ip.Dim; i++)
            {
                current[i] -= 1;
                end[i] -= 1;
                dest[i] -= 1;
            }

        CheckBounds(ip.Dim, size);
        var done = false;
        while (!done)
        {
            action(ip, new FungeVector(current), new FungeVector(dest));
            for (var i = highOrder ? ip.Dim - 1 : 0;
                 (highOrder && i >= 0) || (!highOrder && i < ip.Dim);
                 i += highOrder ? -1 : 1)
            {
                current[i] += highOrder ? -1 : 1;
                dest[i] += highOrder ? -1 : 1;
                if (dest[i] != end[i]) break;

                if (i == (highOrder ? 0 : ip.Dim - 1))
                {
                    done = true;
                }
                else
                {
                    current[i] = highOrder ? source[i] + size[i] - 1 : source[i];
                    dest[i] = highOrder ? destination[i] + size[i] - 1 : destination[i];
                }
            }
        }
    }

    private static void CopySpace(FungeIP ip, FungeVector source, FungeVector size, FungeVector destination,
        bool highOrder, bool move)
    {
        IterateSpace(ip, source, size, destination, (ip, src, dest) =>
        {
            ip.Put(dest, ip.Get(src));
            if (move)
                ip.Put(src, new FungeInt(' '));
        }, highOrder);
    }

    /// <summary>
    ///     Copy a region of Funge-space from one location to another. The source region is not modified.
    ///     The copy uses low-order iteration.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="source">The minimum (top-left) corner of the source region.</param>
    /// <param name="size">The size of the region to copy.</param>
    /// <param name="destination">The minimum (top-left) corner of the destination region.</param>
    [Instruction('C')]
    public static void CopyFungeSpaceLow(FungeIP ip, FungeVector source, FungeVector size, FungeVector destination)
    {
        CopySpace(ip, source, size, destination, false, false);
    }

    /// <summary>
    ///     Copy a region of Funge-space from one location to another. The source region is not modified.
    ///     The copy uses high-order iteration.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="source">The minimum (top-left) corner of the source region.</param>
    /// <param name="size">The size of the region to copy.</param>
    /// <param name="destination">The minimum (top-left) corner of the destination region.</param>
    [Instruction('K')]
    public static void CopyFungeSpaceHigh(FungeIP ip, FungeVector source, FungeVector size, FungeVector destination)
    {
        CopySpace(ip, source, size, destination, true, false);
    }

    /// <summary>
    ///     Move a region of Funge-space from one location to another. The source region is cleared.
    ///     The move uses low-order iteration.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="source">The minimum (top-left) corner of the source region.</param>
    /// <param name="size">The size of the region to move.</param>
    /// <param name="destination">The minimum (top-left) corner of the destination region.</param>
    [Instruction('M')]
    public static void MoveFungeSpaceLow(FungeIP ip, FungeVector source, FungeVector size, FungeVector destination)
    {
        CopySpace(ip, source, size, destination, false, true);
    }

    /// <summary>
    ///     Move a region of Funge-space from one location to another. The source region is cleared.
    ///     The move uses high-order iteration.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="source">The minimum (top-left) corner of the source region.</param>
    /// <param name="size">The size of the region to move.</param>
    /// <param name="destination">The minimum (top-left) corner of the destination region.</param>
    [Instruction('V')]
    public static void MoveFungeSpaceHighOrder(FungeIP ip, FungeVector source, FungeVector size,
        FungeVector destination)
    {
        CopySpace(ip, source, size, destination, true, true);
    }

    /// <summary>
    ///     Fill a region of Funge-space with a given value.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="value">The value to fill the region with.</param>
    /// <param name="size">The size of the region to fill.</param>
    /// <param name="destination">The minimum (top-left) corner of the region to fill.</param>
    [Instruction('S')]
    public static void FillFungeSpace(FungeIP ip, FungeInt value, FungeVector size, FungeVector destination)
    {
        IterateSpace(ip, new FungeVector(0), size, destination, (ip, _, dest) => ip.Put(dest, value));
    }

    /// <summary>
    ///     Write a matrix (or higher-dimensional array) to Funge-space. The matrix is popped from the stack one element at
    ///     a time, and placed in the destination region using a low-order iteration.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="size">The size of the matrix to write.</param>
    /// <param name="destination">The minimum (top-left) corner of the region to write to.</param>
    [Instruction('F')]
    public static void WriteMatrixToFungeSpace(FungeIP ip, FungeVector size, FungeVector destination)
    {
        IterateSpace(ip, new FungeVector(0), size, destination,
            (ip, _, dest) => ip.Put(dest, ip.PopFromStack()));
    }

    /// <summary>
    ///     Read a matrix (or higher-dimensional array) from Funge-space. The matrix is read from the source region using a
    ///     low-order iteration, and pushed to the stack in reverse order, such that it can be used with
    ///     <see cref="WriteMatrixToFungeSpace" />.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="size">The size of the matrix to read.</param>
    /// <param name="source">The minimum (top-left) corner of the region to read from.</param>
    [Instruction('G')]
    public static void ReadMatrixFromFungeSpace(FungeIP ip, FungeVector size, FungeVector source)
    {
        List<int> values = [];
        IterateSpace(ip, new FungeVector(0), size, source,
            (ip, _, src) => values.Add(ip.Get(src)));
        for (var i = values.Count - 1; i >= 0; i--) ip.PushToStack(values[i]);
    }

    /// <summary>
    ///     Copies a value on the stack to the top of the stack <paramref name="n" /> times.
    ///     If <paramref name="n" /> is non-positive, the value is simply popped from the stack.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="v">The value to copy.</param>
    /// <param name="n">The number of times to copy the value.</param>
    [Instruction('A')]
    public static void CopyNTimes(FungeIP ip, FungeInt v, FungeInt n)
    {
        for (var i = 0; i < n; i++) ip.PushToStack(v);
    }

    /// <summary>
    ///     Perform a butterfly operation on the top two values of the stack. Two values are popped,
    ///     and two values are pushed: the sum and the difference of the two popped values.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    [Instruction('B')]
    public static void Butterfly(FungeIP ip, FungeInt a, FungeInt b)
    {
        ip.PushToStack(a + b);
        ip.PushToStack(a - b);
    }

    /// <summary>
    ///     Decrement a value by one.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="a">The value to decrement.</param>
    /// <returns>The decremented value.</returns>
    [Instruction('D')]
    public static FungeInt Decrement(FungeIP ip, FungeInt a)
    {
        return a - 1;
    }

    /// <summary>
    ///     Sum all values on the stack.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <returns>The sum of all values on the stack.</returns>
    [Instruction('E')]
    public static FungeInt StackSum(FungeIP ip)
    {
        var sum = 0;
        while (ip.StackStack.TOSS.Size > 0) sum += ip.PopFromStack();
        return sum;
    }

    /// <summary>
    ///     Compute the product of all values on the stack.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <returns>The product of all values on the stack.</returns>
    [Instruction('P')]
    public static FungeInt StackProduct(FungeIP ip)
    {
        var product = 1;
        while (ip.StackStack.TOSS.Size > 0) product *= ip.PopFromStack();
        return product;
    }

    /// <summary>
    ///     Perform a bitwise shift operation on a value. If the shift amount is positive, the value is shifted left.
    ///     If the shift amount is negative, the value is shifted right.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="a">The value to shift.</param>
    /// <param name="b">The amount to shift by.</param>
    /// <returns>The shifted value.</returns>
    [Instruction('H')]
    public static FungeInt BitShift(FungeIP ip, FungeInt a, FungeInt b)
    {
        if (b > 0) return a << b;

        return a >> -b;
    }

    /// <summary>
    ///     Increment a value by one.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="a">The value to increment.</param>
    /// <returns>The incremented value.</returns>
    [Instruction('I')]
    public static FungeInt Increment(FungeIP ip, FungeInt a)
    {
        return a + 1;
    }

    private static void MoveLine(FungeIP ip, FungeInt offset, int dim)
    {
        var pos = ip.Position;
        List<(FungeInt value, FungeVector newCoords)> line = [];
        for (var i = ip.Space.MinBounds[dim]; i <= ip.Space.MaxBounds[dim]; i++)
        {
            var oldPos = pos.SetCoordinate(dim, i);
            var newPos = pos.SetCoordinate(dim, i + offset);
            var temp = ip.Space[oldPos];
            ip.Space[oldPos] = ' ';
            if (temp != ' ') line.Add((temp, newPos));
        }

        foreach (var (value, newCoords) in line) ip.Space[newCoords] = value;
    }

    /// <summary>
    ///     Move a column of Funge-space down by a given offset. If the offset is negative, the column is moved up.
    ///     The column is moved as a whole, with all values in the column moving together. No wrapping occurs. <br />
    ///     Reflects if the IP is in 1D.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="offset">The amount to move the column by.</param>
    [Instruction('J', 2)]
    public static void MoveColumn(FungeIP ip, FungeInt offset)
    {
        MoveLine(ip, offset, 1);
    }

    /// <summary>
    ///     Move a row of Funge-space to the right by a given offset. If the offset is negative, the row is moved to the left.
    ///     The row is moved as a whole, with all values in the row moving together. No wrapping occurs.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="offset">The amount to move the row by.</param>
    [Instruction('O')]
    public static void MoveRow(FungeIP ip, FungeInt offset)
    {
        MoveLine(ip, offset, 0);
    }

    /// <summary>
    ///     Push the value to the left of the IP onto the stack. "Left" is from the perspective of the IP's current direction.
    ///     The IP's delta is not affected.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="value">The value to push.</param>
    [Instruction('L')]
    public static void PushLeft(FungeIP ip, FungeInt value)
    {
        CoreInstructions.TurnLeft(ip);
        ip.PushToStack(ip.Space[ip.Position + ip.Delta]);
        CoreInstructions.TurnRight(ip);
    }

    /// <summary>
    ///     Push the value to the right of the IP onto the stack. "Right" is from the perspective of the IP's current
    ///     direction.
    ///     The IP's delta is not affected.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="value">The value to push.</param>
    [Instruction('R')]
    public static void PushRight(FungeIP ip, FungeInt value)
    {
        CoreInstructions.TurnRight(ip);
        ip.PushToStack(ip.Space[ip.Position + ip.Delta]);
        CoreInstructions.TurnLeft(ip);
    }

    /// <summary>
    ///     Negates a value.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="a">The value to negate.</param>
    /// <returns>The negated value.</returns>
    [Instruction('N')]
    public static FungeInt Negate(FungeIP ip, FungeInt a)
    {
        return -a;
    }

    /// <summary>
    ///     Set the value behind the IP to a given value. "Behind" is from the perspective of the IP's current direction.
    ///     The IP's delta is not affected.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="value">The value to set.</param>
    [Instruction('Q')]
    public static void SetBehind(FungeIP ip, FungeInt value)
    {
        ip.Space[ip.Position - ip.Delta] = value;
    }

    /// <summary>
    ///     Sets the IP's delta to a cardinal direction in a given dimension based on a value.
    ///     If the value is zero, the delta is set to the positive direction in the given dimension.
    ///     If the value is non-zero, the delta is set to the negative direction in the given dimension.
    ///     For example, if the value is zero and the dimension is 0, the delta is set to right.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="value">The value to decide with.</param>
    /// <param name="dim">The dimension to set the delta in.</param>
    /// <exception cref="FungeReflectException">Thrown if the given dimension is negative or greater than the IP's dimension.</exception>
    [Instruction('T')]
    public static void DecideWithDim(FungeIP ip, FungeInt value, FungeInt dim)
    {
        if (ip.Dim < dim + 1 || dim < 0)
            throw new FungeReflectException(new ArgumentOutOfRangeException(nameof(dim), "Dimension out of range"));
        ip.Delta = FungeVector.Cardinal(dim, value == 0 ? 1 : -1);
    }

    /// <summary>
    ///     Sets the IP's delta to a random cardinal direction, and replaces the value at the IP's current position in space
    ///     with the instruction character corresponding to the new delta. Because there are no instructions for directions
    ///     above 3D, this instruction is only defined up to 3D.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <exception cref="FungeReflectException">Thrown if the IP's dimension is greater than 3.</exception>
    [Instruction('U')]
    public static void GoRandomAndReplace(FungeIP ip)
    {
        if (ip.Dim > 3)
            throw new FungeReflectException(new InvalidOperationException("TOYS::U is only defined up to 3D"));
        var randomDim = Random.Shared.Next(0, ip.Dim);
        var randomSign = Random.Shared.Next(0, 2) == 0 ? -1 : 1;
        ip.Delta = FungeVector.Cardinal(randomDim, randomSign);
        ip.Space[ip.Position] = ((randomDim + 1) * randomSign) switch
        {
            1 => '>',
            -1 => '<',
            2 => 'v',
            -2 => '^',
            3 => 'h',
            -3 => 'l',
            _ => ip.Space[ip.Position]
        };
    }

    /// <summary>
    ///     "Wait" until the value at a given position in Funge-space is equal to a given value. <br />
    ///     In reality, the IP stays active, but if the value in space is less than the given value,
    ///     the IP is set one step back in the direction it was moving, and the given value and position are pushed
    ///     back to the stack.
    ///     If the value in space is greater than the given value, the IP is reflected.
    ///     If the value in space is equal to the given value, nothing happens. <br />
    ///     This has the effect of "waiting" until the value in space is equal to the given value.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="value">The value to wait for.</param>
    /// <param name="position">The position in space to read from.</param>
    [Instruction('W')]
    public static void Wait(FungeIP ip, FungeInt value, FungeVector position)
    {
        var c = ip.Get(position);
        if (c == value) return;
        if (c > value)
        {
            ip.Reflect();
            return;
        }

        ip.PushToStack(value);
        ip.PushVectorToStack(position);
        ip.Position -= ip.Delta;
    }

    /// <summary>
    ///     Increment the X position of the IP. The delta is not affected.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('X')]
    public static void IncrementX(FungeIP ip)
    {
        ip.Position += FungeVector.Right;
    }

    /// <summary>
    ///     Increment the Y position of the IP. The delta is not affected. <br />
    ///     Reflects if the IP is in 1D.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('Y', 2)]
    public static void IncrementY(FungeIP ip)
    {
        ip.Position += FungeVector.Down;
    }

    /// <summary>
    ///     Increment the Z position of the IP. The delta is not affected. <br />
    ///     Reflects if the IP is in 1D or 2D.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    [Instruction('Z', 3)]
    public static void IncrementZ(FungeIP ip)
    {
        ip.Position += FungeVector.Forwards;
    }
}