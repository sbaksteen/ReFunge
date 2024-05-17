using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace ReFunge.Data.Values
{
    internal readonly struct FungeVector : IFungeValue<FungeVector>, IEquatable<FungeVector>
    {

        private readonly ImmutableList<int> _values;
        public int Size => _values.Count;
        public int Dim 
        {
           get
            {
                for (var i = _values.Count - 1; i >= 0; i--)
                {
                    if (_values[i] != 0)
                    {
                        return i + 1;
                    }
                }
                return 0;
            }
        }

        public FungeVector(params int[] values) => _values = values.ToImmutableList();

        public FungeVector(ImmutableList<int> values) => _values = values;

        public FungeVector() => _values = ImmutableList<int>.Empty;

        public static FungeVector Right = new FungeVector(1);
        public static FungeVector Left = new FungeVector(-1);
        public static FungeVector Down = new FungeVector(0, 1);
        public static FungeVector Up = new FungeVector(0, -1);
        public static FungeVector Forwards = new FungeVector(0, 0, 1);
        public static FungeVector Backwards = new FungeVector(0, 0, -1);

        public static FungeVector Cardinal(int dim, int sign)
        {
            int[] ints = new int[dim+1];
            ints[dim] = sign;
            return new FungeVector(ints);
        }

        public int this[int index] => index < _values.Count ? _values[index] : 0;

        public static FungeVector operator +(FungeVector a, FungeVector b)
        {
            var result = new int[Math.Max(a.Size, b.Size)];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = a[i] + b[i];
            }
            return new FungeVector(result);
        }

        public static FungeVector operator -(FungeVector a)
        {
            var result = new int[a.Size];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = -a[i];
            }
            return new FungeVector(result);
        }
        public static FungeVector operator -(FungeVector a, FungeVector b) => a + -b;

        public static FungeVector operator *(FungeVector a, int b)
        {
            var result = new int[a.Size];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = a[i] * b;
            }
            return new FungeVector(result);
        }

        // Equality operator that returns true if the two vectors are equal, even if they have different sizes
        public static bool operator ==(FungeVector a, FungeVector b)
        {
            var size = Math.Max(a.Size, b.Size);
            for (var i = 0; i < size; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator !=(FungeVector a, FungeVector b) => !(a == b);

        public static FungeVector PopFromStack(FungeIP ip) => ip.PopVectorFromStack();

        public void PushToStack(FungeIP ip) => ip.PushVectorToStack(this);

        public readonly override int GetHashCode()
        {
            const int prime = 31;
            var hash = 1;

            for (var i = 0; i < _values.Count; i++)
            {
                var value = _values[i];
                if (value == 0) continue;
                // Combine the value and its index in the hash code
                hash = hash * prime + value.GetHashCode();
                hash = hash * prime + i.GetHashCode();
            }

            return hash;
        }

        public bool Equals(FungeVector other) => this == other;

        public override bool Equals([NotNullWhen(true)] object? obj) => 
            obj is FungeVector vector && this == vector;

        public override string ToString() => 
            "V[" + string.Join(", ", _values) + "]";
    }
}
