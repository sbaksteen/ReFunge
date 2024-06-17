using System.Globalization;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.DataTypes;

/// <summary>
///     FPSP: Floating-point (single precision) extension
///     Implements single-precision floating-point arithmetic.
///     Allows floats to be stored on the stack as integers representing the bits of the float.
///     From RC/Funge-98.
/// </summary>
[Fingerprint("FPSP")]
public static class FPSP
{
    /// <summary>
    ///     Adds two floating-point numbers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the addition.</returns>
    [Instruction('A')]
    public static FungeFloat Add(FungeIP _, FungeFloat a, FungeFloat b)
    {
        return a + b;
    }

    /// <summary>
    ///     Sine of a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The sine of the operand.</returns>
    [Instruction('B')]
    public static FungeFloat Sine(FungeIP _, FungeFloat a)
    {
        return float.Sin(a);
    }

    /// <summary>
    ///     Cosine of a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The cosine of the operand.</returns>
    [Instruction('C')]
    public static FungeFloat Cosine(FungeIP _, FungeFloat a)
    {
        return float.Cos(a);
    }

    /// <summary>
    ///     Divides two floating-point numbers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The result of the division.</returns>
    [Instruction('D')]
    public static FungeFloat Divide(FungeIP _, FungeFloat a, FungeFloat b)
    {
        return a / b;
    }

    /// <summary>
    ///     Arcsine of a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The arcsine of the operand.</returns>
    [Instruction('E')]
    public static FungeFloat ArcSine(FungeIP _, FungeFloat a)
    {
        return float.Asin(a);
    }

    /// <summary>
    ///     Converts an integer to a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The integer to convert.</param>
    /// <returns>The converted floating-point number.</returns>
    [Instruction('F')]
    public static FungeFloat IntToFloat(FungeIP _, FungeInt a)
    {
        return (float)a;
    }

    /// <summary>
    ///     Arctangent of a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The arctangent of the operand.</returns>
    [Instruction('G')]
    public static FungeFloat ArcTangent(FungeIP _, FungeFloat a)
    {
        return float.Atan(a);
    }

    /// <summary>
    ///     Arc-cosine of a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The arccosine of the operand.</returns>
    [Instruction('H')]
    public static FungeFloat ArcCosine(FungeIP _, FungeFloat a)
    {
        return float.Acos(a);
    }

    /// <summary>
    ///     Converts a floating-point number to an integer.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The floating-point number to convert.</param>
    /// <returns>The converted integer.</returns>
    [Instruction('I')]
    public static FungeInt FloatToInt(FungeIP _, FungeFloat a)
    {
        return (int)a;
    }

    /// <summary>
    ///     Natural logarithm of a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The natural logarithm of the operand.</returns>
    [Instruction('K')]
    public static FungeFloat NaturalLog(FungeIP _, FungeFloat a)
    {
        return float.Log(a);
    }

    /// <summary>
    ///     Base 10 logarithm of a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The base 10 logarithm of the operand.</returns>
    [Instruction('L')]
    public static FungeFloat LogBase10(FungeIP _, FungeFloat a)
    {
        return float.Log10(a);
    }

    /// <summary>
    ///     Multiplies two floating-point numbers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the multiplication.</returns>
    [Instruction('M')]
    public static FungeFloat Multiply(FungeIP _, FungeFloat a, FungeFloat b)
    {
        return a * b;
    }

    /// <summary>
    ///     Negates a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The negation of the operand.</returns>
    [Instruction('N')]
    public static FungeFloat Negate(FungeIP _, FungeFloat a)
    {
        return -a;
    }

    /// <summary>
    ///     Prints a floating-point number to the output stream, followed by a space.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="a">The floating-point number to print.</param>
    [Instruction('P')]
    public static void Print(FungeIP ip, FungeFloat a)
    {
        ip.Interpreter.WriteString(a + " ");
    }

    /// <summary>
    ///     Square root of a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The square root of the operand.</returns>
    [Instruction('Q')]
    public static FungeFloat SquareRoot(FungeIP _, FungeFloat a)
    {
        return float.Sqrt(a);
    }

    /// <summary>
    ///     Parses a floating-point number from a string.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="str">The string to parse.</param>
    /// <returns>The parsed floating-point number.</returns>
    /// <exception cref="FungeReflectException">Thrown if the string is not a valid floating-point number.</exception>
    [Instruction('R')]
    public static FungeFloat TryParse(FungeIP ip, FungeString str)
    {
        if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)) return result;
        throw new FungeReflectException(new ArgumentException("Invalid double value"));
    }

    /// <summary>
    ///     Subtracts two floating-point numbers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The minuend.</param>
    /// <param name="b">The subtrahend.</param>
    /// <returns>The result of the subtraction.</returns>
    [Instruction('S')]
    public static FungeFloat Subtract(FungeIP _, FungeFloat a, FungeFloat b)
    {
        return a - b;
    }

    /// <summary>
    ///     Tangent of a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The tangent of the operand.</returns>
    [Instruction('T')]
    public static FungeFloat Tangent(FungeIP _, FungeFloat a)
    {
        return float.Tan(a);
    }

    /// <summary>
    ///     Absolute value of a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The operand.</param>
    /// <returns>The absolute value of the operand.</returns>
    [Instruction('V')]
    public static FungeFloat AbsoluteValue(FungeIP _, FungeFloat a)
    {
        return float.Abs(a);
    }

    /// <summary>
    ///     Raises e to the power of a floating-point number.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The exponent.</param>
    /// <returns>e raised to the power of the exponent.</returns>
    [Instruction('X')]
    public static FungeFloat NaturalExponent(FungeIP _, FungeFloat a)
    {
        return float.Exp(a);
    }

    /// <summary>
    ///     Raises a floating-point number to a power.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The base.</param>
    /// <param name="b">The exponent.</param>
    /// <returns>The base raised to the power of the exponent.</returns>
    [Instruction('Y')]
    public static FungeFloat Power(FungeIP _, FungeFloat a, FungeFloat b)
    {
        return float.Pow(a, b);
    }


    /// <summary>
    ///     Represents a Funge float, stored as an integer representing the bits of the float.
    ///     This is a simple wrapper around C#'s <see cref="float" /> type, and has implicit conversions to and from
    ///     <see cref="float" />.
    ///     It implements <see cref="IFungeValue{T}" /> for itself, facilitating its use in Funge instructions.
    /// </summary>
    /// <param name="Value">The float value.</param>
    public readonly record struct FungeFloat(float Value) : IFungeValue<FungeFloat>
    {
        /// <summary>
        ///     Pop a <see cref="FungeFloat" /> from the stack of the given IP. An integer is popped from the stack and
        ///     reinterpreted as a float.
        /// </summary>
        /// <param name="ip">The IP facilitating the stack operation.</param>
        /// <returns>The popped <see cref="FungeFloat" />.</returns>
        public static FungeFloat PopFromStack(FungeIP ip)
        {
            var value = ip.PopFromStack();
            return new FungeFloat(BitConverter.Int32BitsToSingle(value));
        }

        /// <summary>
        ///     Push this <see cref="FungeFloat" /> to the stack of the given IP. The float is reinterpreted as an integer
        ///     and pushed to the stack.
        /// </summary>
        /// <param name="ip">The IP facilitating the stack operation.</param>
        public void PushToStack(FungeIP ip)
        {
            ip.PushToStack(BitConverter.SingleToInt32Bits(Value));
        }

        /// <summary>
        ///     Implicitly convert a <see cref="float" /> to a <see cref="FungeFloat" />.
        /// </summary>
        /// <param name="value">The float value to convert.</param>
        /// <returns>The resulting <see cref="FungeFloat" />.</returns>
        public static implicit operator FungeFloat(float value)
        {
            return new FungeFloat(value);
        }

        /// <summary>
        ///     Implicitly convert a <see cref="FungeFloat" /> to a <see cref="float" />.
        /// </summary>
        /// <param name="me">The <see cref="FungeFloat" /> to convert.</param>
        /// <returns>The resulting <see cref="float" />.</returns>
        public static implicit operator float(FungeFloat me)
        {
            return me.Value;
        }

        /// <summary>
        ///     Returns a string representation of the float. This is the float's value as a string, using the invariant culture.
        /// </summary>
        /// <returns>The float as a string.</returns>
        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}