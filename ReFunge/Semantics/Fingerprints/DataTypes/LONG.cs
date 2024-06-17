using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.DataTypes;

/// <summary>
///     LONG: Long integers <br />
///     Implements long integers, taking up two stack cells. <br />
///     From RC/Funge-98.
/// </summary>
[Fingerprint("LONG")]
public static class LONG
{
    /// <summary>
    ///     Add two long integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the addition.</returns>
    [Instruction('A')]
    public static FungeLong Add(FungeIP _, FungeLong a, FungeLong b)
    {
        return a + b;
    }

    /// <summary>
    ///     Absolute value of a long integer.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The absolute value of the operand.</returns>
    [Instruction('B')]
    public static FungeLong AbsoluteValue(FungeIP _, FungeLong a)
    {
        return long.Abs(a);
    }

    /// <summary>
    ///     Divide two long integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The result of the division, or 0 if the divisor is 0.</returns>
    [Instruction('D')]
    public static FungeLong Divide(FungeIP _, FungeLong a, FungeLong b)
    {
        if (b == 0) return 0;
        return a / b;
    }

    /// <summary>
    ///     Convert an integer to a long integer.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The integer to convert.</param>
    /// <returns>The long integer.</returns>
    [Instruction('E')]
    public static FungeLong IntToLong(FungeIP _, FungeInt a)
    {
        return (long)a;
    }

    /// <summary>
    ///     Shift a long integer left by a number of bits.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The long integer to shift.</param>
    /// <param name="b">The number of bits to shift by.</param>
    /// <returns>The result of the shift.</returns>
    /// <exception cref="FungeReflectException">Thrown if the shift amount is negative.</exception>
    [Instruction('L')]
    public static FungeLong LeftShift(FungeIP _, FungeLong a, FungeInt b)
    {
        if (b < 0)
            throw new FungeReflectException(new ArgumentOutOfRangeException(nameof(b),
                "Shift amount must be non-negative"));
        return a << b;
    }

    /// <summary>
    ///     Multiply two long integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the multiplication.</returns>
    [Instruction('M')]
    public static FungeLong Multiply(FungeIP _, FungeLong a, FungeLong b)
    {
        return a * b;
    }

    /// <summary>
    ///     Negate a long integer.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The negation of the operand.</returns>
    [Instruction('N')]
    public static FungeLong Negate(FungeIP _, FungeLong a)
    {
        return -a;
    }

    /// <summary>
    ///     Modulo of two long integers. This behaves like the remainder operation in C#, with the sign of the result
    ///     matching the dividend. If the divisor is 0, the result is 0.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The result of the modulo operation, or 0 if the divisor is 0.</returns>
    [Instruction('O')]
    public static FungeLong Modulo(FungeIP _, FungeLong a, FungeLong b)
    {
        if (b == 0) return 0;
        return a % b;
    }

    /// <summary>
    ///     Print a long integer to the output stream.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="a">The long integer to print.</param>
    [Instruction('P')]
    public static void Print(FungeIP ip, FungeLong a)
    {
        ip.Interpreter.WriteString(a + " ");
    }

    /// <summary>
    ///     Right shift a long integer by a number of bits.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The long integer to shift.</param>
    /// <param name="b">The number of bits to shift by.</param>
    /// <returns>The result of the shift.</returns>
    /// <exception cref="FungeReflectException">Thrown if the shift amount is negative.</exception>
    [Instruction('R')]
    public static FungeLong RightShift(FungeIP _, FungeLong a, FungeInt b)
    {
        if (b < 0)
            throw new FungeReflectException(new ArgumentOutOfRangeException(nameof(b),
                "Shift amount must be non-negative"));
        return a >> b;
    }

    /// <summary>
    ///     Subtract two long integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The minuend.</param>
    /// <param name="b">The subtrahend.</param>
    /// <returns>The result of the subtraction.</returns>
    [Instruction('S')]
    public static FungeLong Subtract(FungeIP _, FungeLong a, FungeLong b)
    {
        return a - b;
    }

    /// <summary>
    ///     Attempt to parse a long integer from a string.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="str">The string to parse.</param>
    /// <returns>The parsed long integer.</returns>
    /// <exception cref="FungeReflectException">Thrown if the string is not a valid long integer.</exception>
    [Instruction('Z')]
    public static FungeLong TryParse(FungeIP _, FungeString str)
    {
        if (long.TryParse(str, out var result)) return result;
        throw new FungeReflectException(new ArgumentException("Invalid long integer format"));
    }

    /// <summary>
    ///     Represents a long integer, taking up two stack cells. This is a simple wrapper around <see cref="long" />, and
    ///     has implicit conversions to and from <see cref="long" />. It implements <see cref="IFungeValue{TSelf}" /> for
    ///     itself, facilitating its use in Funge instructions.
    /// </summary>
    /// <param name="Value">The value of the long integer.</param>
    public readonly record struct FungeLong(long Value) : IFungeValue<FungeLong>
    {
        /// <summary>
        ///     Pop a long integer from the stack. Two integers are popped, then combined into a long integer.
        ///     The top of the stack is the low 32 bits, and the next element is the high 32 bits.
        /// </summary>
        /// <param name="ip">The IP executing the instruction.</param>
        /// <returns>The popped long integer.</returns>
        public static FungeLong PopFromStack(FungeIP ip)
        {
            var intLow = (uint)(int)ip.PopFromStack();
            var intHigh = (uint)(int)ip.PopFromStack();
            return new FungeLong(((long)intHigh << 32) | intLow);
        }

        /// <summary>
        ///     Push the long integer to the stack. The high 32 bits are pushed first, followed by the low 32 bits.
        /// </summary>
        /// <param name="ip">The IP executing the instruction.</param>
        public void PushToStack(FungeIP ip)
        {
            ip.PushToStack((int)(Value >> 32));
            ip.PushToStack((int)(Value & 0xFFFFFFFF));
        }

        /// <summary>
        ///     Implicitly convert a <see cref="long" /> to a <see cref="FungeLong" />.
        /// </summary>
        /// <param name="value">The long integer to convert.</param>
        /// <returns>The <see cref="FungeLong" />.</returns>
        public static implicit operator FungeLong(long value)
        {
            return new FungeLong(value);
        }

        /// <summary>
        ///     Implicitly convert a <see cref="FungeLong" /> to a <see cref="long" />.
        /// </summary>
        /// <param name="me">The <see cref="FungeLong" /> to convert.</param>
        /// <returns>The long integer.</returns>
        public static implicit operator long(FungeLong me)
        {
            return me.Value;
        }

        /// <summary>
        ///     Return the string representation of the long integer.
        /// </summary>
        /// <returns>The string representation of the long integer.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}