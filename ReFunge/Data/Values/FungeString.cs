using System.Collections;

namespace ReFunge.Data.Values;

/// <summary>
///     Represents a string on the Funge stack. This is a simple wrapper around C#'s <see cref="string" /> type, and has
///     implicit conversions to and from <see cref="string" />. It implements <see cref="IFungeValue{T}" /> for itself,
///     facilitating its use in Funge instructions.
/// </summary>
/// <param name="Value">The string value.</param>
public readonly record struct FungeString(string Value) : IFungeValue<FungeString>, IEnumerable<char>
{
    /// <summary>
    ///     The handprint of the string. This is the string's value as an integer, with each character's ASCII value
    ///     concatenated together. As such, it can only be used sensibly for strings that are 4 characters or less.
    /// </summary>
    public int Handprint => this.Aggregate(0, (current, c) => current * 256 + c);

    /// <summary>
    ///     The string's enumerator.
    /// </summary>
    /// <returns> The string's enumerator. </returns>
    public IEnumerator<char> GetEnumerator()
    {
        return ((IEnumerable<char>)Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Value).GetEnumerator();
    }

    /// <summary>
    ///     Pop a <see cref="FungeString" /> from the stack of the given IP.
    /// </summary>
    /// <param name="ip">The IP facilitating the stack operation.</param>
    /// <returns>The popped <see cref="FungeString" />.</returns>
    /// <seealso cref="FungeIP.PopStringFromStack" />
    /// <seealso cref="FungeStack.PopString" />
    public static FungeString PopFromStack(FungeIP ip)
    {
        return ip.PopStringFromStack();
    }

    /// <summary>
    ///     Push this <see cref="FungeString" /> to the stack of the given IP.
    /// </summary>
    /// <param name="ip">The IP facilitating the stack operation.</param>
    /// <seealso cref="FungeIP.PushStringToStack" />
    /// <seealso cref="FungeStack.PushString" />
    public void PushToStack(FungeIP ip)
    {
        ip.PushStringToStack(this);
    }

    /// <summary>
    ///     Implicitly convert a <see cref="string" /> to a <see cref="FungeString" />.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <returns>The resulting <see cref="FungeString" />.</returns>
    public static implicit operator FungeString(string value)
    {
        return new FungeString(value);
    }

    /// <summary>
    ///     Implicitly convert a <see cref="FungeString" /> to a <see cref="string" />.
    /// </summary>
    /// <param name="me">The <see cref="FungeString" /> to convert.</param>
    /// <returns>The resulting string value.</returns>
    public static implicit operator string(FungeString me)
    {
        return me.Value;
    }
}