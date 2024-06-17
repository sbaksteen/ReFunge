using System.Collections;

namespace ReFunge.Data.Values;

public readonly record struct FungeString(string Value) : IFungeValue<FungeString>, IEnumerable<char>
{
    public int Handprint => this.Aggregate(0, (current, c) => current * 256 + c);

    public IEnumerator<char> GetEnumerator()
    {
        return ((IEnumerable<char>)Value).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Value).GetEnumerator();
    }

    public static FungeString PopFromStack(FungeIP ip)
    {
        return ip.PopStringFromStack();
    }

    public void PushToStack(FungeIP ip)
    {
        ip.PushStringToStack(this);
    }

    public static implicit operator FungeString(string value)
    {
        return new FungeString(value);
    }

    public static implicit operator string(FungeString me)
    {
        return me.Value;
    }
}