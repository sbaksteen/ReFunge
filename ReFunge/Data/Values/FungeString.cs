using System.Collections;

namespace ReFunge.Data.Values;

public readonly record struct FungeString(string Value) : IFungeValue<FungeString>, IEnumerable<char>
{

    public int Handprint => this.Aggregate(0, (current, c) => current * 256 + c);

    public static implicit operator FungeString(string value) => new(value);

    public static implicit operator string(FungeString me) => me.Value;

    public static FungeString PopFromStack(FungeIP ip) => ip.PopStringFromStack();

    public void PushToStack(FungeIP ip) => ip.PushStringToStack(this);

    public IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)Value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Value).GetEnumerator();
}