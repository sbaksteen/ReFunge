using System.Collections;
using Nito.Collections;
using ReFunge.Data.Values;

namespace ReFunge.Data;

/// <summary>
///     Represents a Funge stack of integers. This is a simple wrapper around <see cref="Deque{T}" /> with some additional
///     functionality for Funge-specific operations. It is held in a <see cref="FungeStackStack" /> for use by a
///     <see cref="FungeIP" />.
/// </summary>
public class FungeStack
{
    private readonly Deque<int> _stack = new();

    /// <summary>
    ///     Create a new, empty Funge stack.
    /// </summary>
    public FungeStack()
    {
    }

    /// <summary>
    ///     Create a new Funge stack with the given contents.
    /// </summary>
    /// <param name="stack">The contents of the stack.</param>
    public FungeStack(Deque<int> stack)
    {
        _stack = stack;
    }

    /// <summary>
    ///     The number of elements on the stack.
    /// </summary>
    public int Size => _stack.Count;

    /// <summary>
    ///     Retrieve an element at the given index on the stack. If the index is out of range, 0 is returned.
    ///     Index 0 is the top of the stack.
    /// </summary>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    public int this[int index] => index >= _stack.Count ? 0 : _stack.ElementAt(index);

    /// <summary>
    ///     Removes and returns the top element from the stack. If the stack is empty, 0 is returned instead.
    ///     If <paramref name="bottom" /> is true, the element is popped from the bottom of the stack instead of the top.
    /// </summary>
    /// <param name="bottom">Whether to pop from the bottom of the stack.</param>
    /// <returns>The popped element.</returns>
    public FungeInt Pop(bool bottom = false)
    {
        if (_stack.Count == 0) return 0;
        return bottom ? _stack.RemoveFromBack() : _stack.RemoveFromFront();
    }

    /// <summary>
    ///     Pushes a value to the top of the stack.
    ///     If <paramref name="bottom" /> is true, the value is pushed to the bottom of the stack.
    /// </summary>
    /// <param name="value">The value to push.</param>
    /// <param name="bottom">Whether to push to the bottom of the stack.</param>
    public void Push(FungeInt value, bool bottom = false)
    {
        if (bottom)
            _stack.AddToBack(value);
        else
            _stack.AddToFront(value);
    }

    /// <summary>
    ///     Pops a <see cref="FungeVector" /> of the given dimension from the stack. Uses <see cref="Pop" /> to retrieve
    ///     each element, so when the stack is empty, 0 is returned for each element. If <paramref name="bottom" /> is true,
    ///     the vector is popped from the bottom of the stack instead of the front. <br />
    ///     The first element popped is the last element in the vector. For example, if <paramref name="dim" /> is 3, the
    ///     z component is popped first, then the y component, then the x component.
    /// </summary>
    /// <param name="dim">The dimension of the vector to pop.</param>
    /// <param name="bottom">Whether to pop from the bottom of the stack.</param>
    /// <returns>The popped vector.</returns>
    public FungeVector PopVector(int dim, bool bottom = false)
    {
        var ints = new int[dim];
        for (var i = dim - 1; i >= 0; i--) ints[i] = Pop(bottom);
        return new FungeVector(ints);
    }

    /// <summary>
    ///     Pushes a <see cref="FungeVector" /> to the stack. If <paramref name="bottom" /> is true, the vector is pushed to
    ///     the bottom of the stack instead of the front. <br />
    ///     The vector is pushed in order, such that the top of the stack after the operation is the last element in the
    ///     vector. Only the first <paramref name="dim" /> elements of the vector are pushed, so if the vector has more
    ///     elements, they are ignored.
    /// </summary>
    /// <param name="vector">The vector to push.</param>
    /// <param name="dim">The dimension of the vector to push.</param>
    /// <param name="bottom">Whether to push to the bottom of the stack.</param>
    public void PushVector(FungeVector vector, int dim, bool bottom = false)
    {
        for (var i = 0; i < dim; i++) Push(vector[i], bottom);
    }

    /// <summary>
    ///     Pops a <see cref="FungeString" /> from the stack. If <paramref name="bottom" /> is true, the string is popped from
    ///     the bottom of the stack instead of the front. <br />
    ///     A string is popped by popping integers until a 0 is encountered, treating each integer as a char value.
    /// </summary>
    /// <param name="bottom">Whether to pop from the bottom of the stack.</param>
    /// <returns>The popped string.</returns>
    public FungeString PopString(bool bottom = false)
    {
        var str = "";
        var c = (char)Pop(bottom);
        while (c != 0)
        {
            str += c;
            c = (char)Pop(bottom);
        }

        return str;
    }

    /// <summary>
    ///     Pushes a <see cref="FungeString" /> to the stack. If <paramref name="bottom" /> is true, the string is pushed to
    ///     the bottom of the stack instead of the front. <br />
    ///     A string is pushed by pushing a 0, then pushing each character in reverse order.
    /// </summary>
    /// <param name="str">The string to push.</param>
    /// <param name="bottom">Whether to push to the bottom of the stack.</param>
    public void PushString(FungeString str, bool bottom = false)
    {
        string s = str;
        Push(0, bottom);
        for (var i = s.Length - 1; i >= 0; i--) Push(s[i], bottom);
    }

    /// <summary>
    ///     Clears the stack, removing all elements.
    /// </summary>
    public void Clear()
    {
        _stack.Clear();
    }

    /// <summary>
    ///     Creates a deep copy of the stack.
    /// </summary>
    /// <returns>The copied stack.</returns>
    internal FungeStack Clone()
    {
        return new FungeStack(new Deque<int>(_stack));
    }
}

/// <summary>
///     Represents a stack of <see cref="FungeStack" />s. This is a simple wrapper around <see cref="Stack{T}" /> with some
///     additional functionality for Funge-specific operations. It is used by a <see cref="FungeIP" /> to manage its
///     stacks.
///     A <see cref="FungeStackStack" /> always contains at least one stack.
/// </summary>
public class FungeStackStack : IEnumerable<FungeStack>
{
    private readonly Stack<FungeStack> _stack = new();

    /// <summary>
    ///     Create a new Funge stack stack with a single empty stack.
    /// </summary>
    public FungeStackStack()
    {
        _stack.Push(new FungeStack());
    }

    /// <summary>
    ///     The number of stacks in the stack stack.
    /// </summary>
    public int Size => _stack.Count;

    /// <summary>
    ///     The top stack on the stack stack.
    /// </summary>
    public FungeStack TOSS => _stack.Peek();

    /// <summary>
    ///     The second stack on the stack stack, or null if there is only one stack.
    /// </summary>
    public FungeStack? SOSS => _stack.Count > 1 ? _stack.ElementAt(1) : null;

    /// <summary>
    ///     Retrieve a stack at the given index. Index 0 is the top stack.
    /// </summary>
    /// <param name="index">The zero-based index of the stack to retrieve.</param>
    /// <returns>The stack at the given index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
    public FungeStack this[int index] => _stack.ElementAt(index);

    /// <summary>
    ///     Returns the stack's enumerator.
    /// </summary>
    /// <returns>The stack's enumerator.</returns>
    public IEnumerator<FungeStack> GetEnumerator()
    {
        return ((IEnumerable<FungeStack>)_stack).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_stack).GetEnumerator();
    }

    /// <summary>
    ///     Creates a deep copy of the stack stack.
    /// </summary>
    /// <returns>The copied stack stack.</returns>
    public FungeStackStack Clone()
    {
        FungeStackStack newStack = new();
        newStack._stack.Clear();
        foreach (var s in _stack) newStack._stack.Push(s.Clone());
        return newStack;
    }

    /// <summary>
    ///     Pushes a stack to the stack stack.
    /// </summary>
    /// <param name="stack">The stack to push.</param>
    public void PushStack(FungeStack stack)
    {
        _stack.Push(stack);
    }

    /// <summary>
    ///     Creates a new stack and pushes it to the stack stack.
    /// </summary>
    public void NewStack()
    {
        _stack.Push(new FungeStack());
    }

    /// <summary>
    ///     Removes the top stack from the stack stack. If there is only one stack, nothing happens.
    /// </summary>
    public void RemoveStack()
    {
        if (_stack.Count > 1) _stack.Pop();
    }
}