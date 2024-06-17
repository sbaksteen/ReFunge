using System.Globalization;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.DataTypes;

/// <summary>
///     FPDP: Floating-point (double precision) extension <br />
///     Implements double-precision floating-point arithmetic. 
///     Allows doubles to be stored on the stack as two stack cells representing the bits of the double. <br />
///     From RC/Funge-98.
/// </summary>
[Fingerprint("FPDP")]
public static class FPDP
{
    /// <summary>
    ///     Add two doubles.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the addition.</returns>
    [Instruction('A')]
    public static FungeDouble Add(FungeIP _, FungeDouble a, FungeDouble b)
    {
        return a + b;
    }

    /// <summary>
    ///     Sine of a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The sine of the operand.</returns>
    [Instruction('B')]
    public static FungeDouble Sine(FungeIP _, FungeDouble a)
    {
        return double.Sin(a);
    }

    /// <summary>
    ///     Cosine of a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The cosine of the operand.</returns>
    [Instruction('C')]
    public static FungeDouble Cosine(FungeIP _, FungeDouble a)
    {
        return double.Cos(a);
    }

    /// <summary>
    ///     Divide two doubles.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The result of the division.</returns>
    [Instruction('D')]
    public static FungeDouble Divide(FungeIP _, FungeDouble a, FungeDouble b)
    {
        return a / b;
    }

    /// <summary>
    ///     Arcsine of a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The arcsine of the operand.</returns>
    [Instruction('E')]
    public static FungeDouble ArcSine(FungeIP _, FungeDouble a)
    {
        return double.Asin(a);
    }

    /// <summary>
    ///     Convert an integer to a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The integer to convert.</param>
    /// <returns>The resulting double.</returns>
    [Instruction('F')]
    public static FungeDouble IntToDouble(FungeIP _, FungeInt a)
    {
        return (double)a;
    }

    /// <summary>
    ///     Arctangent of a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The arctangent of the operand.</returns>
    [Instruction('G')]
    public static FungeDouble ArcTangent(FungeIP _, FungeDouble a)
    {
        return double.Atan(a);
    }

    /// <summary>
    ///     Arc-cosine of a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The arc-cosine of the operand.</returns>
    [Instruction('H')]
    public static FungeDouble ArcCosine(FungeIP _, FungeDouble a)
    {
        return double.Acos(a);
    }

    /// <summary>
    ///     Convert a double to an integer.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The double to convert.</param>
    /// <returns>The resulting integer.</returns>
    [Instruction('I')]
    public static FungeInt DoubleToInt(FungeIP _, FungeDouble a)
    {
        return (int)a;
    }

    /// <summary>
    ///     Natural logarithm of a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The natural logarithm of the operand.</returns>
    [Instruction('K')]
    public static FungeDouble NaturalLog(FungeIP _, FungeDouble a)
    {
        return double.Log(a);
    }

    /// <summary>
    ///     Logarithm base 10 of a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The logarithm base 10 of the operand.</returns>
    [Instruction('L')]
    public static FungeDouble LogBase10(FungeIP _, FungeDouble a)
    {
        return double.Log10(a);
    }

    /// <summary>
    ///     Multiply two doubles.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the multiplication.</returns>
    [Instruction('M')]
    public static FungeDouble Multiply(FungeIP _, FungeDouble a, FungeDouble b)
    {
        return a * b;
    }

    /// <summary>
    ///     Negate a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The negation of the operand.</returns>
    [Instruction('N')]
    public static FungeDouble Negate(FungeIP _, FungeDouble a)
    {
        return -a;
    }

    /// <summary>
    ///     Print a double to the output stream, followed by a space.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="a">The double to print.</param>
    [Instruction('P')]
    public static void Print(FungeIP ip, FungeDouble a)
    {
        ip.Interpreter.WriteString(a + " ");
    }

    /// <summary>
    ///     Square root of a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The square root of the operand.</returns>
    [Instruction('Q')]
    public static FungeDouble SquareRoot(FungeIP _, FungeDouble a)
    {
        return double.Sqrt(a);
    }

    /// <summary>
    ///     Parse a double from a string.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="str">The string to parse.</param>
    /// <returns>The parsed double.</returns>
    /// <exception cref="FungeReflectException">Thrown if the string is not a valid double.</exception>
    [Instruction('R')]
    public static FungeDouble TryParse(FungeIP ip, FungeString str)
    {
        if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)) return result;
        throw new FungeReflectException(new ArgumentException("Invalid double value"));
    }

    /// <summary>
    ///     Subtract two doubles.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The minuend.</param>
    /// <param name="b">The subtrahend.</param>
    /// <returns>The result of the subtraction.</returns>
    [Instruction('S')]
    public static FungeDouble Subtract(FungeIP _, FungeDouble a, FungeDouble b)
    {
        return a - b;
    }

    /// <summary>
    ///     Tangent of a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The tangent of the operand.</returns>
    [Instruction('T')]
    public static FungeDouble Tangent(FungeIP _, FungeDouble a)
    {
        return double.Tan(a);
    }

    /// <summary>
    ///     Absolute value of a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The absolute value of the operand.</returns>
    [Instruction('V')]
    public static FungeDouble AbsoluteValue(FungeIP _, FungeDouble a)
    {
        return double.Abs(a);
    }

    /// <summary>
    ///     Raise e to the power of a double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The exponent.</param>
    /// <returns>e raised to the power of the exponent.</returns>
    [Instruction('X')]
    public static FungeDouble NaturalExponent(FungeIP _, FungeDouble a)
    {
        return double.Exp(a);
    }

    /// <summary>
    ///     Raise a double to the power of another double.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The base.</param>
    /// <param name="b">The exponent.</param>
    /// <returns>The base raised to the power of the exponent.</returns>
    [Instruction('Y')]
    public static FungeDouble Power(FungeIP _, FungeDouble a, FungeDouble b)
    {
        return double.Pow(a, b);
    }

    /// <summary>
    ///     Represents a Funge double-precision floating-point value. This is a simple wrapper around C#'s
    ///     <see cref="double" /> type,
    ///     and has implicit conversions to and from <see cref="double" />. It implements <see cref="IFungeValue{T}" /> for
    ///     itself,
    ///     facilitating its use in Funge instructions.
    /// </summary>
    /// <param name="Value">The double value.</param>
    public readonly record struct FungeDouble(double Value) : IFungeValue<FungeDouble>
    {
        /// <summary>
        ///     Pop a <see cref="FungeDouble" /> from the stack of the given IP. The double is stored as two stack cells.
        ///     The top cell is the least significant bits, and the bottom cell is the most significant bits.
        /// </summary>
        /// <param name="ip">The IP facilitating the stack operation.</param>
        /// <returns>The popped <see cref="FungeDouble" />.</returns>
        public static FungeDouble PopFromStack(FungeIP ip)
        {
            var longVal = LONG.FungeLong.PopFromStack(ip);
            return new FungeDouble(BitConverter.Int64BitsToDouble(longVal));
        }

        /// <summary>
        ///     Push this <see cref="FungeDouble" /> to the stack of the given IP. The double is stored as two stack cells.
        ///     The top cell is the least significant bits, and the bottom cell is the most significant bits.
        /// </summary>
        /// <param name="ip">The IP facilitating the stack operation.</param>
        public void PushToStack(FungeIP ip)
        {
            var bits = BitConverter.DoubleToInt64Bits(Value);
            new LONG.FungeLong(bits).PushToStack(ip);
        }

        /// <summary>
        ///     Implicitly convert a <see cref="double" /> to a <see cref="FungeDouble" />.
        /// </summary>
        /// <param name="value">The double value to convert.</param>
        /// <returns>The resulting <see cref="FungeDouble" />.</returns>
        public static implicit operator FungeDouble(double value)
        {
            return new FungeDouble(value);
        }

        /// <summary>
        ///     Implicitly convert a <see cref="FungeDouble" /> to a <see cref="double" />.
        /// </summary>
        /// <param name="me">The <see cref="FungeDouble" /> to convert.</param>
        /// <returns>The resulting <see cref="double" />.</returns>
        public static implicit operator double(FungeDouble me)
        {
            return me.Value;
        }

        /// <summary>
        ///     Returns a string representation of the double. This is the double's value as a string, using the invariant culture.
        /// </summary>
        /// <returns>The string representation of the double.</returns>
        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}