namespace ReFunge.Semantics;

/// <summary>
///     Represents an instruction executable by a FungeIP.
/// </summary>
public class FungeInstruction
{
    private readonly FungeFunc _func;

    /// <summary>
    ///     Creates a new FungeInstruction.
    /// </summary>
    /// <param name="func">The <see cref="FungeFunc" /> callback associated with the instruction.</param>
    /// <param name="name">
    ///     The name of the instruction. For example, the core instruction &gt; (go right)
    ///     has the name <c>Core::&gt;</c>.
    /// </param>
    /// <param name="sourceFingerprintCode">The code (handprint) of the fingerprint this instruction is from, if applicable.</param>
    /// <param name="minDimension">
    ///     The minimum amount of dimensions the IP must be able to interface with to execute this
    ///     instruction.
    /// </param>
    public FungeInstruction(FungeFunc func, string name, int? sourceFingerprintCode = null, int minDimension = 1)
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
        SourceFingerprintCode = sourceFingerprintCode;
    }

    /// <summary>
    ///     The name of the instruction.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     The code (handprint) of the fingerprint this instruction is from, if applicable.
    /// </summary>
    public int? SourceFingerprintCode { get; }

    /// <summary>
    ///     Executes the instruction.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <exception cref="FungeReflectException">Thrown when the IP does not see enough dimensions to execute the instruction.</exception>
    public void Execute(FungeIP ip)
    {
        _func.Execute(ip);
    }
}