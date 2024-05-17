namespace ReFunge.Data.Values
{
    internal struct FungeInt(int value) : IFungeValue<FungeInt>
    {
        private int Value = value;

        public static implicit operator FungeInt(int value)
        {
            return new FungeInt(value);
        }

        public static implicit operator int(FungeInt me)
        {
            return me.Value;
        }

        public static FungeInt PopFromStack(FungeIP ip)
        {
            return ip.PopFromStack();
        }

        public void PushToStack(FungeIP ip)
        {
            ip.PushToStack(this);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
