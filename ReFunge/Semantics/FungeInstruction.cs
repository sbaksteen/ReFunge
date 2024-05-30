namespace ReFunge.Semantics.Fingerprints;

public class FungeInstruction
{
    private FungeFunc _func;
    private readonly string _name;
    private readonly Type _source;
    private readonly int? _sourceFingerprintCode;
    
    public FungeInstruction(FungeFunc func, string name, Type source, int? sourceFingerprintCode = null)
    {
        _func = func;
        _name = name;
        _source = source;
        _sourceFingerprintCode = sourceFingerprintCode;
    }

    public string Name => _name;

    public Type Source => _source;

    public int? SourceFingerprintCode => _sourceFingerprintCode;

    public void Execute(FungeIP ip)
    {
        _func.Execute(ip);
    }
}