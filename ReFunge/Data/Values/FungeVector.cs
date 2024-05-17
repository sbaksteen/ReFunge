using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace ReFunge.Data.Values
{
    internal struct FungeVector : IFungeValue<FungeVector>, IEquatable<FungeVector>
    {

        private readonly ImmutableList<int> _Values;
        public readonly int Size => _Values.Count;
        public readonly int Dim 
        {
           get
            {
                for (int i = _Values.Count - 1; i >= 0; i--)
                {
                    if (_Values[i] != 0)
                    {
                        return i + 1;
                    }
                }
                return 0;
            }
        }

        public FungeVector(params int[] values)
        {
            _Values = values.ToImmutableList();
        }
        public FungeVector(ImmutableList<int> values)
        {
            _Values = values;
        }
        public FungeVector()
        {
            _Values = ImmutableList<int>.Empty;
        }

        public static FungeVector RIGHT = new FungeVector(1);
        public static FungeVector LEFT = new FungeVector(-1);
        public static FungeVector DOWN = new FungeVector(0, 1);
        public static FungeVector UP = new FungeVector(0, -1);
        public static FungeVector FORWARDS = new FungeVector(0, 0, 1);
        public static FungeVector BACKWARDS = new FungeVector(0, 0, -1);

        public static FungeVector Cardinal(int dim, int sign)
        {
            int[] ints = new int[dim+1];
            ints[dim] = sign;
            return new FungeVector(ints);
        }

        public int this[int index] => index < _Values.Count ? _Values[index] : 0;

        public static FungeVector operator +(FungeVector a, FungeVector b)
        {
            int[] ints = new int[Math.Max(a.Size, b.Size)];
            for (int i = 0; i < ints.Length; i++)
            {
                ints[i] = a[i] + b[i];
            }
            return new FungeVector(ints);
        }

        public static FungeVector operator -(FungeVector a)
        {
            int[] ints = new int[a.Size];
            for (int i = 0; i < ints.Length; i++)
            {
                ints[i] = -a[i];
            }
            return new FungeVector(ints);
        }
        public static FungeVector operator -(FungeVector a, FungeVector b)
        {
            return a + -b;
        }

        public static FungeVector operator *(FungeVector a, int b)
        {
            int[] ints = new int[a.Size];
            for (int i = 0; i < ints.Length; i++)
            {
                ints[i] = a[i] * b;
            }
            return new FungeVector(ints);
        }

        // Equality operator that returns true if the two vectors are equal, even if they have different sizes
        public static bool operator ==(FungeVector a, FungeVector b)
        {
            int size = Math.Max(a.Size, b.Size);
            for (int i = 0; i < size; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool operator !=(FungeVector a, FungeVector b)
        {
            return !(a == b);
        }

        public static FungeVector PopFromStack(FungeIP ip)
        {
            return ip.PopVectorFromStack();
        }

        public void PushToStack(FungeIP ip)
        {
            ip.PushVectorToStack(this);
        }

        public readonly override int GetHashCode()
        {
            const int prime = 31;
            int hash = 1;

            for (int i = 0; i < _Values.Count; i++)
            {
                int value = _Values[i];
                if (value != 0)
                {
                    // Combine the value and its index in the hash code
                    hash = hash * prime + value.GetHashCode();
                    hash = hash * prime + i.GetHashCode();
                }
            }

            return hash;
        }

        public bool Equals(FungeVector other)
        {
            return this == other;
        }

        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is FungeVector vector)
            {
                return this == vector;
            }
            return false;
        }

        public readonly override string ToString()
        {
            return "V[" + string.Join(", ", _Values) + "]";
        }
    }
}
