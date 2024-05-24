using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("REFC", FingerprintType.InstancedPerInterpreter)]
public class REFC : InstancedFingerprint
{
    // REFC: Store references to vectors.
    
    private List<FungeVector> _vectors = new();

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
        {
            throw new FungeReflectException(new IndexOutOfRangeException("Reference index out of range"));
        }

        return _vectors[index];
    }
}