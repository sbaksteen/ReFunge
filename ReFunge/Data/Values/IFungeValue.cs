namespace ReFunge.Data.Values;

/// <summary>
/// A value that can be pushed to and popped from a <c>FungeStack</c>, facilitated by a <c>FungeIP</c>.
/// Examples include <c>FungeInt</c> and <c>FungeVector</c>.
/// </summary>
/// <remarks>
/// The dependency on <c>FungeIP</c> is done so that the IP has the freedom to push and pop in different ways.
/// See for example <c>FungeVector</c>, which models an arbitrary-dimensional vector, and is pushed or popped as an
/// n-dimensional vector based on the <c>Dim</c> property of the IP.
/// Another area where this is relevant is the insert mode and queue mode implemented for the <c>MODE</c> fingerprint,
/// which let the IP push or pop on the bottom of the stack.
/// </remarks>
/// <typeparam name="T">The type returned by <c>PopFromStack</c>. Should be the same as the inheriting type.</typeparam>
public interface IFungeValue<out T> where T : IFungeValue<T>
{
    /// <summary>
    /// Pops a value from an IP's stack. Typically, this is from the top of the TOSS (top of stack stack), but this can
    /// be altered by IP modes.
    /// </summary>
    /// <param name="ip">The IP facilitating the stack operation</param>
    /// <returns>The popped value</returns>
    public static abstract T PopFromStack(FungeIP ip);
    
    /// <summary>
    /// Pushes integers corresponding to this value to the IP's stack. Typically, this is to the top of the TOSS (top of
    /// stack stack), but this can be altered by IP modes.
    /// </summary>
    /// <param name="ip">The IP facilitating the stack operation</param>
    public void PushToStack(FungeIP ip);
}