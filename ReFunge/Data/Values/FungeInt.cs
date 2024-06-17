namespace ReFunge.Data.Values;

/// <summary>
///     Represents a Funge integer value. This is a simple wrapper around C#'s <see cref="int" /> type, and has
///     implicit conversions to and from <see cref="int" />. It implements <see cref="IFungeValue{T}" /> for itself,
///     facilitating its use in Funge instructions.
/// </summary>
/// <param name="Value">The integer value.</param>
public readonly record struct FungeInt(int Value) : IFungeValue<FungeInt>
{
    /// <summary>
    ///     Pop a <see cref="FungeInt" /> from the stack of the given IP.
    /// </summary>
    /// <param name="ip">The IP facilitating the stack operation.</param>
    /// <returns>The popped <see cref="FungeInt" />.</returns>
    public static FungeInt PopFromStack(FungeIP ip)
    {
        return ip.PopFromStack();
    }

    /// <summary>
    ///     Push this <see cref="FungeInt" /> to the stack of the given IP.
    /// </summary>
    /// <param name="ip">The IP facilitating the stack operation.</param>
    public void PushToStack(FungeIP ip)
    {
        ip.PushToStack(this);
    }

    /// <summary>
    ///     Implicitly convert an <see cref="int" /> to a <see cref="FungeInt" />.
    /// </summary>
    /// <param name="value">The integer value to convert.</param>
    /// <returns>The resulting <see cref="FungeInt" />.</returns>
    public static implicit operator FungeInt(int value)
    {
        return new FungeInt(value);
    }

    /// <summary>
    ///     Implicitly convert a <see cref="FungeInt" /> to an <see cref="int" />.
    /// </summary>
    /// <param name="me">The <see cref="FungeInt" /> to convert.</param>
    /// <returns>The resulting integer value.</returns>
    public static implicit operator int(FungeInt me)
    {
        return me.Value;
    }

    /// <summary>
    ///     Get a string representation of this <see cref="FungeInt" />. This is simply the string representation of the
    ///     integer value.
    /// </summary>
    /// <returns>The string representation of the integer value.</returns>
    public override string ToString()
    {
        return Value.ToString();
    }
}