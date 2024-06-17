using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("REFC", FingerprintType.InstancedPerInterpreter)]
public class REFC : InstancedFingerprint
{
    // REFC: Store references to vectors.
    // From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/REFC.markdown)

    private readonly List<FungeVector> _vectors = new();

    public REFC(Interpreter interpreter) : base(interpreter)
    {
    }

    [Instruction('R')]
    public FungeInt StoreReference(FungeIP ip, FungeVector vector)
    {
        _vectors.Add(vector);
        return _vectors.Count - 1;
    }

    [Instruction('D')]
    public FungeVector RetrieveReference(FungeIP ip, FungeInt index)
    {
        if (index < 0 || index >= _vectors.Count)
            throw new FungeReflectException(new IndexOutOfRangeException("Reference index out of range"));

        return _vectors[index];
    }
}