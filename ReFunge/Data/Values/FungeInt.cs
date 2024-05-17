namespace ReFunge.Data.Values
{
    internal readonly record struct FungeInt(int Value) : IFungeValue<FungeInt>
    {

        public static implicit operator FungeInt(int value) => new(value);

        public static implicit operator int(FungeInt me) => me.Value;

        public static FungeInt PopFromStack(FungeIP ip) => ip.PopFromStack();

        public void PushToStack(FungeIP ip) => ip.PushToStack(this);

        public override string ToString() => Value.ToString();
    }
}
