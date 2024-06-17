namespace ReFunge.Semantics.Fingerprints;

public class FungeInstruction
{
    private readonly FungeFunc _func;

    public FungeInstruction(FungeFunc func, string name, Type source, int? sourceFingerprintCode = null,
        int minDimension = 1)
    {
        if (minDimension > 1)
            _func = new FungeAction(ip =>
            {
                if (ip.Dim < minDimension)
                    throw new FungeReflectException(new InvalidOperationException(
                        $"{ip}: Operation {name} requires at least {minDimension} dimensions, but only {ip.Dim} are available."));

                func.Execute(ip);
            });
        else
            _func = func;
        Name = name;
        Source = source;
        SourceFingerprintCode = sourceFingerprintCode;
    }

    public string Name { get; }

    public Type Source { get; }

    public int? SourceFingerprintCode { get; }

    public void Execute(FungeIP ip)
    {
        _func.Execute(ip);
    }
}