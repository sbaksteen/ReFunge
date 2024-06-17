using ReFunge.Semantics.Fingerprints.Core;

namespace ReFunge.Data.Values;

/// <summary>
///     A value that can be pushed to and popped from a <see cref="FungeStack" />, facilitated by a <see cref="FungeIP" />.
///     Examples include <see cref="FungeInt" /> and <see cref="FungeVector" />.
/// </summary>
/// <remarks>
///     The dependency on <see cref="FungeIP" /> is done so that the IP has the freedom to push and pop in different ways.
///     See for example <see cref="FungeVector" />, which models an arbitrary-dimensional vector, and is pushed or popped
///     as an
///     n-dimensional vector based on the <see cref="FungeIP.Dim" /> property of the IP.
///     Another area where this is relevant is the insert mode and queue mode implemented for the <see cref="MODE" />
///     fingerprint,
///     which let the IP push or pop on the bottom of the stack.
/// </remarks>
/// <typeparam name="TSelf">The type returned by <see cref="PopFromStack" />. Should be the same as the inheriting type.</typeparam>
public interface IFungeValue<out TSelf> where TSelf : IFungeValue<TSelf>
{
    /// <summary>
    ///     Pops a value from an IP's stack. Typically, this is from the top of the TOSS (top of stack-stack), but this can
    ///     be altered by IP modes.
    /// </summary>
    /// <param name="ip">The IP facilitating the stack operation</param>
    /// <returns>The popped value</returns>
    public static abstract TSelf PopFromStack(FungeIP ip);

    /// <summary>
    ///     Pushes integers corresponding to this value to the IP's stack. Typically, this is to the top of the TOSS (top of
    ///     stack-stack), but this can be altered by IP modes.
    /// </summary>
    /// <param name="ip">The IP facilitating the stack operation</param>
    public void PushToStack(FungeIP ip);
}