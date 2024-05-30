using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace ReFunge.Data.Values;

public readonly struct FungeVector : IFungeValue<FungeVector>, IEquatable<FungeVector>
{

    private readonly int[] _values;
    public int Size => _values.Length;
    public int Dim 
    {
        get
        {
            for (var i = _values.Length - 1; i >= 0; i--)
            {
                if (_values[i] != 0)
                {
                    return i + 1;
                }
            }
            return 0;
        }
    }

    public int[] ToArray(int size)
    {
        var result = new int[size];
        for (var i = 0; i < size; i++)
        {
            result[i] = this[i];
        }
        return result;
    }

    public FungeVector(params int[] values) => _values = [..values];

    public FungeVector() => _values = [];

    public static FungeVector Right = new(1);
    public static FungeVector Left = new(-1);
    public static FungeVector Down = new(0, 1);
    public static FungeVector Up = new(0, -1);
    public static FungeVector Forwards = new(0, 0, 1);
    public static FungeVector Backwards = new(0, 0, -1);

    public static FungeVector Cardinal(int dim, int sign)
    {
        var result = new int[dim+1];
        result[dim] = sign;
        return new FungeVector(result);
    }

    public int this[int index] => index < _values.Length ? _values[index] : 0;

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

        for (var i = 0; i < _values.Length; i++)
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

    public FungeVector SetCoordinate(int i, FungeInt fungeInt)
    {
        var result = new int[Math.Max(i + 1, _values.Length)];
        for (var j = 0; j < _values.Length; j++)
        {
            result[j] = _values[j];
        }
        result[i] = fungeInt;
        return new FungeVector(result);
    }

    public FungeVector Reverse()
    {
        var result = new int[_values.Length];
        for (var i = 0; i < _values.Length; i++)
        {
            result[i] = _values[_values.Length - i - 1];
        }
        return new FungeVector(result);
    }
}