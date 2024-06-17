using System.Reflection;
using ReFunge.Data;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

using InstructionMap = Dictionary<FungeInt, FungeInstruction>;

/// <summary>
///     Represents a fingerprint that requires an instance object to be created for each interpreter, space, or IP.
/// </summary>
public abstract class InstancedFingerprint
{
    private readonly Interpreter? _interpreter;
    private readonly FungeIP? _ip;
    private readonly FungeSpace? _space;

    private InstancedFingerprint()
    {
        var fingerprintAttribute = GetType().GetCustomAttribute<FingerprintAttribute>();
        if (fingerprintAttribute is null)
            throw new InvalidOperationException("Fingerprint must have a FingerprintAttribute");
        Name = fingerprintAttribute.Name;
        // Get all instance methods with the Instruction attribute and create delegates from them in order to populate the Instructions dictionary
        foreach (var method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            var attributes = (InstructionAttribute[])method.GetCustomAttributes(typeof(InstructionAttribute));
            if (attributes.Length == 0) continue;

            var func = FungeFunc.Create(method, this);
            foreach (var attribute in attributes)
                Instructions[attribute.Instruction] = new FungeInstruction(func, Name + "::" + attribute.Instruction,
                    new FungeString(fingerprintAttribute.Name).Handprint, attribute.MinDimension);
        }
    }

    protected InstancedFingerprint(FungeIP ip) : this()
    {
        _ip = ip;
    }

    protected InstancedFingerprint(FungeSpace space) : this()
    {
        _space = space;
    }

    protected InstancedFingerprint(Interpreter interpreter) : this()
    {
        _interpreter = interpreter;
    }

    protected FungeIP IP => _ip ?? throw new InvalidOperationException("IP not set");
    protected FungeSpace Space => _space ?? throw new InvalidOperationException("Space not set");
    protected Interpreter Interpreter => _interpreter ?? throw new InvalidOperationException("Interpreter not set");

    /// <summary>
    ///     The instructions that this fingerprint provides.
    /// </summary>
    public InstructionMap Instructions { get; } = [];

    /// <summary>
    ///     Gets the instruction associated with 'A' (65) plus the given offset.
    /// </summary>
    /// <param name="instruction">The offset from 'A'.</param>
    public FungeInstruction this[FungeInt instruction] => Instructions[instruction];

    /// <summary>
    ///     The name of the fingerprint.
    /// </summary>
    public string Name { get; }
}