using System.Collections;

namespace ReFunge.Data.Values
{
    internal readonly struct FungeString(string value) : IFungeValue<FungeString>, IEnumerable<char>
    {
        private readonly string Value = value;

        public int Handprint
        {
            get
            {
                int r = 0;
                foreach (char c in this)
                {
                    r = r * 256 + c;
                }
                return r;
            }
        }

        public static implicit operator FungeString(string value)
        {
            return new FungeString(value);
        }

        public static implicit operator string(FungeString me)
        {
            return me.Value;
        }

        public static FungeString PopFromStack(FungeIP ip)
        {
            return ip.PopStringFromStack();
        }

        public void PushToStack(FungeIP ip)
        {
            ip.PushStringToStack(this);
        }

        public IEnumerator<char> GetEnumerator()
        {
            return ((IEnumerable<char>)Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Value).GetEnumerator();
        }
    }
}
