using System.Reflection;
using ReFunge.Data;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

using InstructionMap = Dictionary<FungeInt, FungeFunc>;
public abstract class InstancedFingerprint
{
    private FungeIP? _ip;
    private FungeSpace? _space;
    private Interpreter? _interpreter;
    
    protected FungeIP IP => _ip ?? throw new InvalidOperationException("IP not set");
    protected FungeSpace Space => _space ?? throw new InvalidOperationException("Space not set");
    protected Interpreter Interpreter => _interpreter ?? throw new InvalidOperationException("Interpreter not set");

    public InstructionMap Instructions { get; } = [];

    public FungeFunc this[FungeInt instruction] => Instructions[instruction];
    
    public string Name { get; }
    
    private InstancedFingerprint()
    {
        var fingerprintAttribute = GetType().GetCustomAttribute<FingerprintAttribute>();
        if (fingerprintAttribute is null)
        {
            throw new InvalidOperationException("Fingerprint must have a FingerprintAttribute");
        }
        Name = fingerprintAttribute.Name;
        // Get all instance methods with the Instruction attribute and create delegates from them in order to populate the Instructions dictionary
        foreach (var method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            var attributes = (InstructionAttribute[])method.GetCustomAttributes(typeof(InstructionAttribute));
            if (attributes.Length == 0)
            {
                continue;
            }

            var func = FungeFunc.Create(method, this);
            foreach (var attribute in attributes)
            {
                Instructions[attribute.Instruction] = func;
            }
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
}