using System.Collections;
using Nito.Collections;
using ReFunge.Data.Values;

namespace ReFunge.Data;

public class FungeStack
{
    private readonly Deque<int> _stack = new();

    public FungeStack()
    {
    }

    public FungeStack(Deque<int> stack)
    {
        _stack = stack;
    }

    public int Size => _stack.Count;

    public int this[int index] => index >= _stack.Count ? 0 : _stack.ElementAt(index);

    public FungeInt Pop(bool back = false)
    {
        if (_stack.Count == 0) return 0;
        return back ? _stack.RemoveFromBack() : _stack.RemoveFromFront();
    }

    public void Push(FungeInt value, bool back = false)
    {
        if (back)
            _stack.AddToBack(value);
        else
            _stack.AddToFront(value);
    }

    public FungeVector PopVector(int dim, bool back = false)
    {
        var ints = new int[dim];
        for (var i = dim - 1; i >= 0; i--) ints[i] = Pop(back);
        return new FungeVector(ints);
    }

    public void PushVector(FungeVector vector, int dim, bool back = false)
    {
        for (var i = 0; i < dim; i++) Push(vector[i], back);
    }

    public FungeString PopString(bool back = false)
    {
        var str = "";
        var c = (char)Pop(back);
        while (c != 0)
        {
            str += c;
            c = (char)Pop(back);
        }

        return str;
    }

    public void PushString(FungeString str, bool back = false)
    {
        string s = str;
        Push(0, back);
        for (var i = s.Length - 1; i >= 0; i--) Push(s[i], back);
    }

    internal void Clear()
    {
        _stack.Clear();
    }

    internal FungeStack Clone()
    {
        return new FungeStack(new Deque<int>(_stack));
    }
}

public class FungeStackStack : IEnumerable<FungeStack>
{
    private readonly Stack<FungeStack> _stack = new();

    public FungeStackStack()
    {
        _stack.Push(new FungeStack());
    }

    public int Size => _stack.Count;

    public FungeStack TOSS => _stack.Peek();
    public FungeStack? SOSS => _stack.Count > 1 ? _stack.ElementAt(1) : null;

    public FungeStack this[int index] => _stack.ElementAt(index);

    public IEnumerator<FungeStack> GetEnumerator()
    {
        return ((IEnumerable<FungeStack>)_stack).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_stack).GetEnumerator();
    }

    public FungeStackStack Clone()
    {
        FungeStackStack newStack = new();
        newStack._stack.Clear();
        foreach (var s in _stack) newStack._stack.Push(s.Clone());
        return newStack;
    }

    public void PushStack(FungeStack stack)
    {
        _stack.Push(stack);
    }

    public void NewStack()
    {
        _stack.Push(new FungeStack());
    }

    public void RemoveStack()
    {
        if (_stack.Count > 1) _stack.Pop();
    }
}