using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.Core;

/// <summary>
///     REFC: Store references to vectors. <br />
///     From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/REFC.markdown)
/// </summary>
[Fingerprint("REFC", FingerprintType.InstancedPerInterpreter)]
public class REFC : InstancedFingerprint
{
    private readonly List<FungeVector> _vectors = new();

    /// <summary>
    ///     Create a new instance of REFC.
    /// </summary>
    /// <param name="interpreter">The interpreter this instance is associated with.</param>
    public REFC(Interpreter interpreter) : base(interpreter)
    {
    }

    /// <summary>
    ///     Store a reference to a vector.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="vector">The vector to store a reference to.</param>
    /// <returns>The index of the stored reference.</returns>
    [Instruction('R')]
    public FungeInt StoreReference(FungeIP ip, FungeVector vector)
    {
        _vectors.Add(vector);
        return _vectors.Count - 1;
    }

    /// <summary>
    ///     Retrieve a vector from a stored reference.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <param name="index">The index of the reference to retrieve.</param>
    /// <returns>The vector stored at the given index.</returns>
    /// <exception cref="FungeReflectException">Thrown if no vector is associated with the index, or the index is below zero.</exception>
    [Instruction('D')]
    public FungeVector RetrieveReference(FungeIP ip, FungeInt index)
    {
        if (index < 0 || index >= _vectors.Count)
            throw new FungeReflectException(new IndexOutOfRangeException("Reference index out of range"));

        return _vectors[index];
    }
}