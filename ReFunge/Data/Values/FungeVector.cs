using System.Diagnostics.CodeAnalysis;

namespace ReFunge.Data.Values;

/// <summary>
///     Represents a vector in Funge-space.
///     This models an infinite-dimensional vector with a finite number of non-zero coordinates.
///     As such, any trailing zeros are ignored when comparing vectors, and are not included in the hash code.
/// </summary>
public readonly struct FungeVector : IFungeValue<FungeVector>, IEquatable<FungeVector>
{
    /// <summary>
    ///     The actual contents of the vector. May contain trailing zeros.
    /// </summary>
    private readonly int[] _values;

    /// <summary>
    ///     The size of the backing array, including trailing zeros.
    /// </summary>
    private int Size => _values.Length;

    /// <summary>
    ///     The number of non-zero coordinates in the vector.
    /// </summary>
    public int Dim
    {
        get
        {
            for (var i = _values.Length - 1; i >= 0; i--)
                if (_values[i] != 0)
                    return i + 1;
            return 0;
        }
    }

    /// <summary>
    ///     Converts the vector to an array of a given size.
    /// </summary>
    /// <param name="size">The desired size of the array.</param>
    /// <returns>
    ///     An array containing the first <paramref name="size" /> coordinates of the vector.
    /// </returns>
    public int[] ToArray(int size)
    {
        var result = new int[size];
        for (var i = 0; i < size; i++) result[i] = this[i];
        return result;
    }

    /// <summary>
    ///     Creates a new vector from an array of values. The array is copied, but trailing zeros are not removed.
    /// </summary>
    /// <param name="values">The values to initialize the vector with.</param>
    public FungeVector(params int[] values)
    {
        _values = [..values];
    }

    /// <summary>
    ///     Default constructor that initializes an empty vector. Equivalent to <c>FungeVector(0)</c>.
    /// </summary>
    public FungeVector()
    {
        _values = [];
    }

    /// <summary>
    ///     A vector pointing to the right.
    /// </summary>
    public static FungeVector Right = new(1);

    /// <summary>
    ///     A vector pointing to the left.
    /// </summary>
    public static FungeVector Left = new(-1);

    /// <summary>
    ///     A vector pointing down.
    /// </summary>
    public static FungeVector Down = new(0, 1);

    /// <summary>
    ///     A vector pointing up.
    /// </summary>
    public static FungeVector Up = new(0, -1);

    /// <summary>
    ///     A vector pointing forwards. (In the direction of the z-axis)
    /// </summary>
    public static FungeVector Forwards = new(0, 0, 1);

    /// <summary>
    ///     A vector pointing backwards. (In the direction of the negative z-axis)
    /// </summary>
    public static FungeVector Backwards = new(0, 0, -1);

    /// <summary>
    ///     Creates a new vector with a single non-zero coordinate.
    ///     The set coordinate is at the given dimension, always has a magnitude of 1,
    ///     and is positive if <paramref name="sign" /> is positive.
    /// </summary>
    /// <param name="dim">The dimension of the cardinal vector (0 is one-dimensional, 1 is two-dimensional, etc.)</param>
    /// <param name="sign">The value at the given coordinate is set to the sign of this value.</param>
    /// <returns></returns>
    public static FungeVector Cardinal(int dim, int sign)
    {
        var result = new int[dim + 1];
        result[dim] = int.Sign(sign);
        return new FungeVector(result);
    }

    /// <summary>
    ///     Gets the value at the given index in the vector.
    /// </summary>
    /// <param name="index">The index.</param>
    public int this[int index] => index < _values.Length ? _values[index] : 0;

    /// <summary>
    ///     Adds two vectors together element-wise.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The result of adding <c>a</c> and <c>b</c> together.</returns>
    public static FungeVector operator +(FungeVector a, FungeVector b)
    {
        var result = new int[Math.Max(a.Size, b.Size)];
        for (var i = 0; i < result.Length; i++) result[i] = a[i] + b[i];
        return new FungeVector(result);
    }

    /// <summary>
    ///     Negates a vector element-wise.
    /// </summary>
    /// <param name="a">The vector.</param>
    /// <returns><c>a</c> with each element negated.</returns>
    public static FungeVector operator -(FungeVector a)
    {
        var result = new int[a.Size];
        for (var i = 0; i < result.Length; i++) result[i] = -a[i];
        return new FungeVector(result);
    }

    /// <summary>
    ///     Subtracts two vectors element-wise.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>The result of subtracting <c>b</c> from <c>a</c></returns>
    public static FungeVector operator -(FungeVector a, FungeVector b)
    {
        return a + -b;
    }

    /// <summary>
    ///     Multiplies a vector by a scalar.
    /// </summary>
    /// <param name="a">The vector.</param>
    /// <param name="b">The scalar.</param>
    /// <returns>The result of multiplying each element of <c>a</c> by <c>b</c>.</returns>
    public static FungeVector operator *(FungeVector a, int b)
    {
        var result = new int[a.Size];
        for (var i = 0; i < result.Length; i++) result[i] = a[i] * b;
        return new FungeVector(result);
    }

    /// <summary>
    ///     Checks if two vectors are equal, ignoring trailing zeros.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(FungeVector a, FungeVector b)
    {
        var size = Math.Max(a.Size, b.Size);
        for (var i = 0; i < size; i++)
            if (a[i] != b[i])
                return false;
        return true;
    }

    /// <summary>
    ///     Checks if two vectors are not equal, ignoring trailing zeros.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(FungeVector a, FungeVector b)
    {
        return !(a == b);
    }

    /// <summary>
    ///     Pops a vector from an IP's stack.
    /// </summary>
    /// <seealso cref="FungeIP.PopVectorFromStack" />
    /// <param name="ip">The IP facilitating the stack operation</param>
    /// <returns>
    ///     A FungeVector created by popping the top <c>n</c> elements of <c>ip</c>'s stack, where <c>n</c> is the
    ///     dimensionality of the IP.
    /// </returns>
    public static FungeVector PopFromStack(FungeIP ip)
    {
        return ip.PopVectorFromStack();
    }

    /// <summary>
    ///     Pushes the vector to an IP's stack. Only pushes the first <c>n</c> elements, where <c>n</c> is the
    ///     dimensionality of the IP.
    /// </summary>
    /// <seealso cref="FungeIP.PushVectorToStack" />
    /// <param name="ip">The IP facilitating the stack operation.</param>
    public void PushToStack(FungeIP ip)
    {
        ip.PushVectorToStack(this);
    }

    /// <summary>
    ///     Gets the hash code of the vector. Zeros are ignored, while the index of the non-zero values are included.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        const int prime = 31;
        var hash = 1;

        for (var i = 0; i < Size; i++)
        {
            var value = _values[i];
            if (value == 0) continue;
            // Combine the value and its index in the hash code
            hash = hash * prime + value.GetHashCode();
            hash = hash * prime + i.GetHashCode();
        }

        return hash;
    }

    /// <summary>
    ///     Checks if two vectors are equal, ignoring trailing zeros. Equivalent to <see cref="operator ==" />.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(FungeVector other)
    {
        return this == other;
    }

    /// <summary>
    ///     Checks if an object is a <see cref="FungeVector" /> and is equal to this vector.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is FungeVector vector && this == vector;
    }

    /// <summary>
    ///     Converts the vector to a string representation.
    ///     The format is "F[x, y, z, ...]", where x, y, z, ... are the contents of the vector's backing array.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "F[" + string.Join(", ", _values) + "]";
    }

    /// <summary>
    ///     Returns a vector identical to this one, but with the given coordinate set to the given value.
    /// </summary>
    /// <param name="i">The index at which to change the value.</param>
    /// <param name="value">The value to set the given coordinate to.</param>
    /// <returns></returns>
    public FungeVector SetCoordinate(int i, FungeInt value)
    {
        var newSize = Math.Max(i + 1, Size);
        var result = new int[newSize];
        for (var j = 0; j < newSize; j++)
        {
            if (j == i)
            {
                result[j] = value;
                continue;
            }

            result[j] = this[j];
        }

        return new FungeVector(result);
    }

    /// <summary>
    ///     Returns a vector whose contents are the contents of this vector's backing array, reversed.
    /// </summary>
    /// <returns></returns>
    public FungeVector Reverse()
    {
        var result = new int[Size];
        for (var i = 0; i < Size; i++) result[i] = _values[Size - i - 1];
        return new FungeVector(result);
    }
}