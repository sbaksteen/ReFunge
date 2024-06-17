using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("TOYS")]
public static class TOYS
{
    // TOYS: Fun extra instructions
    // From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/TOYS.markdown)

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

    [Instruction('C')]
    public static void CopyFungeSpaceLow(FungeIP ip, FungeVector source, FungeVector size, FungeVector destination)
    {
        CopySpace(ip, source, size, destination, false, false);
    }

    [Instruction('K')]
    public static void CopyFungeSpaceHigh(FungeIP ip, FungeVector source, FungeVector size, FungeVector destination)
    {
        CopySpace(ip, source, size, destination, true, false);
    }

    [Instruction('M')]
    public static void MoveFungeSpaceLow(FungeIP ip, FungeVector source, FungeVector size, FungeVector destination)
    {
        CopySpace(ip, source, size, destination, false, true);
    }

    [Instruction('V')]
    public static void MoveFungeSpaceHighOrder(FungeIP ip, FungeVector source, FungeVector size,
        FungeVector destination)
    {
        CopySpace(ip, source, size, destination, true, true);
    }

    [Instruction('S')]
    public static void FillFungeSpace(FungeIP ip, FungeInt value, FungeVector size, FungeVector destination)
    {
        IterateSpace(ip, new FungeVector(0), size, destination, (ip, _, dest) => ip.Put(dest, value));
    }

    [Instruction('F')]
    public static void WriteMatrixToFungeSpace(FungeIP ip, FungeVector size, FungeVector destination)
    {
        IterateSpace(ip, new FungeVector(0), size, destination,
            (ip, _, dest) => ip.Put(dest, ip.PopFromStack()));
    }

    [Instruction('G')]
    public static void ReadMatrixFromFungeSpace(FungeIP ip, FungeVector size, FungeVector source)
    {
        List<int> values = [];
        IterateSpace(ip, new FungeVector(0), size, source,
            (ip, _, src) => values.Add(ip.Get(src)));
        for (var i = values.Count - 1; i >= 0; i--) ip.PushToStack(values[i]);
    }

    [Instruction('A')]
    public static void CopyNTimes(FungeIP ip, FungeInt v, FungeInt n)
    {
        for (var i = 0; i < n; i++) ip.PushToStack(v);
    }

    [Instruction('B')]
    public static void Butterfly(FungeIP ip, FungeInt a, FungeInt b)
    {
        ip.PushToStack(a + b);
        ip.PushToStack(a - b);
    }

    [Instruction('D')]
    public static FungeInt Decrement(FungeIP ip, FungeInt a)
    {
        return a - 1;
    }

    [Instruction('E')]
    public static FungeInt StackSum(FungeIP ip)
    {
        var sum = 0;
        while (ip.StackStack.TOSS.Size > 0) sum += ip.PopFromStack();
        return sum;
    }

    [Instruction('P')]
    public static void StackProduct(FungeIP ip)
    {
        var product = 1;
        while (ip.StackStack.TOSS.Size > 0) product *= ip.PopFromStack();
        ip.PushToStack(product);
    }

    [Instruction('H')]
    public static FungeInt BitShift(FungeIP ip, FungeInt a, FungeInt b)
    {
        if (b > 0) return a << b;

        return a >> -b;
    }

    [Instruction('I')]
    public static FungeInt Increment(FungeIP ip, FungeInt a)
    {
        return a + 1;
    }

    public static void MoveLine(FungeIP ip, FungeInt offset, int dim)
    {
        var pos = ip.Position;
        List<(FungeInt value, FungeVector newCoords)> line = [];
        for (var i = ip.Space.MinCoords[dim]; i <= ip.Space.MaxCoords[dim]; i++)
        {
            var oldPos = pos.SetCoordinate(dim, i);
            var newPos = pos.SetCoordinate(dim, i + offset);
            var temp = ip.Space[oldPos];
            ip.Space[oldPos] = ' ';
            if (temp != ' ') line.Add((temp, newPos));
        }

        foreach (var (value, newCoords) in line) ip.Space[newCoords] = value;
    }

    [Instruction('J', 2)]
    public static void MoveColumn(FungeIP ip, FungeInt offset)
    {
        MoveLine(ip, offset, 1);
    }

    [Instruction('O')]
    public static void MoveRow(FungeIP ip, FungeInt offset)
    {
        MoveLine(ip, offset, 0);
    }

    [Instruction('L')]
    public static void PushLeft(FungeIP ip, FungeInt value)
    {
        CoreInstructions.TurnLeft(ip);
        ip.PushToStack(ip.Space[ip.Position + ip.Delta]);
        CoreInstructions.TurnRight(ip);
    }

    [Instruction('R')]
    public static void PushRight(FungeIP ip, FungeInt value)
    {
        CoreInstructions.TurnRight(ip);
        ip.PushToStack(ip.Space[ip.Position + ip.Delta]);
        CoreInstructions.TurnLeft(ip);
    }

    [Instruction('N')]
    public static FungeInt Negate(FungeIP ip, FungeInt a)
    {
        return -a;
    }

    [Instruction('Q')]
    public static void SetBehind(FungeIP ip, FungeInt value)
    {
        ip.Space[ip.Position - ip.Delta] = value;
    }

    [Instruction('T')]
    public static void DecideWithDim(FungeIP ip, FungeInt value, FungeInt dim)
    {
        if (ip.Dim < dim + 1)
            throw new FungeReflectException(new ArgumentOutOfRangeException(nameof(dim), "Dimension out of range"));
        ip.Delta = FungeVector.Cardinal(dim, value == 0 ? 1 : -1);
    }

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

    [Instruction('X')]
    public static void IncrementX(FungeIP ip)
    {
        ip.Position += FungeVector.Right;
    }

    [Instruction('Y', 2)]
    public static void IncrementY(FungeIP ip)
    {
        ip.Position += FungeVector.Down;
    }

    [Instruction('Z', 3)]
    public static void IncrementZ(FungeIP ip)
    {
        ip.Position += FungeVector.Forwards;
    }
}