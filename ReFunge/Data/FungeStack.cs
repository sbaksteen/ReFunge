using System.Collections;
using ReFunge.Data.Values;

namespace ReFunge.Data;

public class FungeStack
{
    private readonly Stack<int> _stack = new();

    public int Size => _stack.Count;

    public FungeStack() { }

    public FungeStack(Stack<int> stack)
    {
        this._stack = stack;
    }

    public int this[int index]  
    { 
        get
        {
            if (index >= _stack.Count)
            {
                return 0;
            }
            return _stack.ElementAt(index);
        }
    }

    public FungeInt Pop() { 
        if (_stack.Count == 0)
        {
            return 0;
        }
        return _stack.Pop(); 
    }

    public void Push(FungeInt value) 
    {
        _stack.Push(value);
    }

    public FungeVector PopVector(int dim)
    {
        var ints = new int[dim];
        for (var i = dim - 1; i >= 0; i--)
        {
            ints[i] = Pop();
        }
        return new FungeVector(ints);
    }

    public void PushVector(FungeVector vector, int dim)
    {
        for (var i = 0; i < dim; i++)
        {
            Push(vector[i]);
        }
    }

    public FungeString PopString()
    {
        var str = "";
        var c = (char)Pop();
        while (c != 0)
        {
            str += c;
            c = (char)Pop();
        }
        return str;
    }

    public void PushString(FungeString str)
    {
        string s = str;
        Push(0);
        for (var i = s.Length - 1; i >= 0; i--)
        {
            Push(s[i]);
        }
    }

    internal void Clear()
    {
        _stack.Clear();
    }

    internal FungeStack Clone()
    {
        return new FungeStack(new(_stack));
    }
}

public class FungeStackStack : IEnumerable<FungeStack>
{
    private readonly Stack<FungeStack> _stack = new();

    public int Size => _stack.Count;

    public FungeStack TOSS => _stack.Peek();
    public FungeStack? SOSS => _stack.Count > 1 ? _stack.ElementAt(1) : null;

    public FungeStack this[int index] => _stack.ElementAt(index);

    public FungeStackStack() {
        _stack.Push(new FungeStack());
    }

    public FungeStackStack Clone()
    {
        FungeStackStack newStack = new();
        foreach (var s in _stack)
        {
            newStack._stack.Push(s.Clone());
        }
        return newStack;
    }
        
    public void PushStack(FungeStack stack)
    {
        _stack.Push(stack);
    }

    public void NewStack(int n)
    {
        List<int> newStack = [];
        for (var i = 0; i < n; i++)
        {
            newStack.Insert(0, TOSS.Pop());
        }
        if (n < 0)
        {
            for (var i = 0; i > n; i--)
            {
                TOSS.Push(0);
            }
        }
        _stack.Push(new FungeStack(new Stack<int>(newStack)));
    }

    public void RemoveStack(int n)
    {
        List<int> transfer = [];
        for (var i = 0; i < n; i++)
        {
            transfer.Add(TOSS.Pop());
        }
        _stack.Pop();
        transfer.Reverse();
        foreach (var i in transfer)
        {
            TOSS.Push(i);
        }
        for (var i = 0; i > n; i--)
        {
            TOSS.Pop();
        }
    }

    public IEnumerator<FungeStack> GetEnumerator()
    {
        return ((IEnumerable<FungeStack>)_stack).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_stack).GetEnumerator();
    }
}