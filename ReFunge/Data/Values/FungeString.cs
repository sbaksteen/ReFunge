using System.Collections;

namespace ReFunge.Data.Values
{
    internal readonly struct FungeString(string value) : IFungeValue<FungeString>, IEnumerable<char>
    {
        private readonly string _value = value;

        public int Handprint => this.Aggregate(0, (current, c) => current * 256 + c);

        public static implicit operator FungeString(string value) => new(value);

        public static implicit operator string(FungeString me) => me._value;

        public static FungeString PopFromStack(FungeIP ip) => ip.PopStringFromStack();

        public void PushToStack(FungeIP ip) => ip.PushStringToStack(this);

        public IEnumerator<char> GetEnumerator() => ((IEnumerable<char>)_value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_value).GetEnumerator();
    }
}
