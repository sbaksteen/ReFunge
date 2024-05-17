namespace ReFunge.Data.Values
{
    internal readonly struct FungeInt(int value) : IFungeValue<FungeInt>
    {
        private readonly int _value = value;

        public static implicit operator FungeInt(int value) => new(value);

        public static implicit operator int(FungeInt me) => me._value;

        public static FungeInt PopFromStack(FungeIP ip) => ip.PopFromStack();

        public void PushToStack(FungeIP ip) => ip.PushToStack(this);

        public override string ToString() => _value.ToString();
    }
}
