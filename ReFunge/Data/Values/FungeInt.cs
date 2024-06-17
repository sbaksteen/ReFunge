namespace ReFunge.Data.Values;

public readonly record struct FungeInt(int Value) : IFungeValue<FungeInt>
{
    public static FungeInt PopFromStack(FungeIP ip)
    {
        return ip.PopFromStack();
    }

    public void PushToStack(FungeIP ip)
    {
        ip.PushToStack(this);
    }

    public static implicit operator FungeInt(int value)
    {
        return new FungeInt(value);
    }

    public static implicit operator int(FungeInt me)
    {
        return me.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}