using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.DataTypes;

/// <summary>
/// CPLI: Complex integer extension <br />
/// Implements complex integers, taking up two stack cells. <br />
/// From RC/Funge-98.
/// </summary>
[Fingerprint("CPLI")]
public class CPLI
{

    /// <summary>
    /// Add two complex integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the addition.</returns>
    [Instruction('A')]
    public static FungeComplex Add(FungeIP _, FungeComplex a, FungeComplex b) => a + b;
    
    /// <summary>
    /// Divide two complex integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The result of the division, with the non-integer parts discarded, or 0 if the divisor is 0.</returns>
    [Instruction('D')]
    public static FungeComplex Divide(FungeIP _, FungeComplex a, FungeComplex b) => a / b;
    
    /// <summary>
    /// Multiply two complex integers.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The result of the multiplication.</returns>
    [Instruction('M')]
    public static FungeComplex Multiply(FungeIP _, FungeComplex a, FungeComplex b) => a * b;
    
    /// <summary>
    /// Subtract one complex integer from another.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="a">The minuend.</param>
    /// <param name="b">The subtrahend.</param>
    /// <returns>The result of the subtraction.</returns>
    [Instruction('S')]
    public static FungeComplex Subtract(FungeIP _, FungeComplex a, FungeComplex b) => a - b;

    /// <summary>
    /// Write a complex integer to the output, followed by a space. The format is "Re + Imi".
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="c">The complex integer to output.</param>
    [Instruction('O')]
    public static void Output(FungeIP ip, FungeComplex c) => ip.Interpreter.WriteString($"{c} ");
    
    /// <summary>
    /// Return the magnitude of a complex integer.
    /// </summary>
    /// <param name="_">The IP executing the instruction.</param>
    /// <param name="c">The complex integer.</param>
    /// <returns>The magnitude of the complex integer.</returns>
    [Instruction('V')]
    public static FungeInt Magnitude(FungeIP _, FungeComplex c) => c.Magnitude;

    /// <summary>
    /// Represents a complex integer, taking up two stack cells. Supports basic arithmetic operations.
    /// Implements <see cref="IFungeValue{TSelf}"/>, facilitating its use in Funge instructions.
    /// </summary>
    /// <param name="Re">The real part.</param>
    /// <param name="Im">The imaginary part.</param>
    public readonly record struct FungeComplex(int Re, int Im) : IFungeValue<FungeComplex>
    {

        /// <summary>
        /// The complex integer 0 + 0i.
        /// </summary>
        public static readonly FungeComplex Zero = new FungeComplex(0, 0);
        
        /// <summary>
        /// Pops a complex integer from the stack. The imaginary part is popped first.
        /// </summary>
        /// <param name="ip">The IP facilitating the stack operation.</param>
        /// <returns>The complex integer popped from the stack.</returns>
        public static FungeComplex PopFromStack(FungeIP ip)
        {
            var im = ip.PopFromStack();
            var re = ip.PopFromStack();
            return new FungeComplex(re, im);
        }

        /// <summary>
        /// Pushes the complex integer to the stack. The real part is pushed first.
        /// </summary>
        /// <param name="ip">The IP facilitating the stack operation.</param>
        public void PushToStack(FungeIP ip)
        {
            ip.PushToStack(Re);
            ip.PushToStack(Im);
        }
        
        /// <summary>
        /// Adds two complex integers.
        /// </summary>
        /// <param name="a">The first complex integer.</param>
        /// <param name="b">The second complex integer.</param>
        /// <returns>The result of the addition.</returns>
        public static FungeComplex operator +(FungeComplex a, FungeComplex b) => new FungeComplex(a.Re + b.Re, a.Im + b.Im);
        
        /// <summary>
        /// Subtracts one complex integer from another.
        /// </summary>
        /// <param name="a">The minuend.</param>
        /// <param name="b">The subtrahend.</param>
        /// <returns>The result of the subtraction.</returns>
        public static FungeComplex operator -(FungeComplex a, FungeComplex b) => new FungeComplex(a.Re - b.Re, a.Im - b.Im);
        
        /// <summary>
        /// Multiplies two complex integers.
        /// </summary>
        /// <param name="a">The first complex integer.</param>
        /// <param name="b">The second complex integer.</param>
        /// <returns>The result of the multiplication.</returns>
        public static FungeComplex operator *(FungeComplex a, FungeComplex b) => new FungeComplex(a.Re * b.Re - a.Im * b.Im, a.Re * b.Im + a.Im * b.Re);
        
        /// <summary>
        /// Divides two complex integers, discarding the non-integer parts of the result.
        /// </summary>
        /// <param name="a">The dividend.</param>
        /// <param name="b">The divisor.</param>
        /// <returns>The result of the division, with the non-integer parts discarded, or 0 if the divisor is 0.</returns>
        public static FungeComplex operator /(FungeComplex a, FungeComplex b)
        {
            if (b == Zero)
                return Zero;
            var denominator = b.Re * b.Re + b.Im * b.Im;
            return new FungeComplex((a.Re * b.Re + a.Im * b.Im) / denominator, (a.Im * b.Re - a.Re * b.Im) / denominator);
        }

        /// <summary>
        /// Returns the magnitude of the complex integer, as an integer.
        /// </summary>
        public FungeInt Magnitude => (int)double.Sqrt(Re * Re + Im * Im);

        /// <summary>
        /// Returns a string representation of the complex integer.
        /// </summary>
        /// <returns>A string representation of the complex integer.</returns>
        public override string ToString()
        {
            return $"{Re} + {Im}i";
        }
    }
}