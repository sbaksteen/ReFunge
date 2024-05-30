using ReFunge.Data.Values;
using ReFunge.Semantics.Fingerprints;
using System.Reflection;
using ReFunge.Data;


namespace ReFunge.Semantics;

using InstructionMap = Dictionary<FungeInt, FungeFunc>;
using FingerprintMap = Dictionary<FungeInt, FungeInstruction>;

public class InstructionRegistry
{
    private Interpreter _interpreter;

    internal FingerprintMap CoreInstructions { get; }

    private readonly Dictionary<FungeInt, FingerprintMap> _staticFingerprints = [];
    
    private readonly Dictionary<FungeInt, InstancedFingerprint> _interpreterFingerprints = [];
    private readonly Dictionary<FungeInt, Type> _spaceFingerprints = [];
    private readonly Dictionary<FungeInt, Type> _ipFingerprints = [];
    
    private void RegisterFingerprint(FungeString name, Type t)
    {
        if (t.GetCustomAttribute<FingerprintAttribute>() is not { } attribute)
        {
            throw new InvalidOperationException("Fingerprint must have a FingerprintAttribute");
        }
        switch (attribute.Type)
        {
            case FingerprintType.Static:
                _staticFingerprints[name.Handprint] = ReadFuncs(t, name);
                return;
            case FingerprintType.InstancedPerInterpreter:
                _interpreterFingerprints[name.Handprint] = (Activator.CreateInstance(t, [_interpreter]) as InstancedFingerprint)!;
                return;
            case FingerprintType.InstancedPerSpace:
                _spaceFingerprints[name.Handprint] = t;
                return;
            case FingerprintType.InstancedPerIP:
                _ipFingerprints[name.Handprint] = t;
                return;
            default:
                throw new InvalidOperationException("Unknown fingerprint type");
        }
    }
    
    public FingerprintType TypeOf(FungeInt code)
    {
        if (_staticFingerprints.ContainsKey(code))
        {
            return FingerprintType.Static;
        }
        if (_interpreterFingerprints.ContainsKey(code))
        {
            return FingerprintType.InstancedPerInterpreter;
        }
        if (_spaceFingerprints.ContainsKey(code))
        {
            return FingerprintType.InstancedPerSpace;
        }
        if (_ipFingerprints.ContainsKey(code))
        {
            return FingerprintType.InstancedPerIP;
        }

        throw new ArgumentException($"Fingerprint {code} not found");
    }
    
    public InstancedFingerprint NewInstance(FungeInt code, FungeIP ip)
    {
        if (_ipFingerprints[code] is not { } fingerprintType)
            throw new ArgumentException($"Fingerprint {code} not found");
        
        return (Activator.CreateInstance(fingerprintType, [ip]) as InstancedFingerprint)!;
    }
    
    public InstancedFingerprint NewInstance(FungeInt code, FungeSpace space)
    {
        if (_spaceFingerprints[code] is not { } fingerprintType)
            throw new ArgumentException($"Fingerprint {code} not found");
        
        return (Activator.CreateInstance(fingerprintType, [space]) as InstancedFingerprint)!;
    }

    public FingerprintMap GetStaticFingerprint(FungeInt code)
    {
        return _staticFingerprints[code];
    }

    public FingerprintMap GetStaticFingerprint(FungeString name)
    {
        return _staticFingerprints[name.Handprint];
    }
    
    public FingerprintMap GetInterpreterFingerprint(FungeInt code)
    {
        return _interpreterFingerprints[code].Instructions;
    }
    
    public FingerprintMap GetInterpreterFingerprint(FungeString name)
    {
        return _interpreterFingerprints[name.Handprint].Instructions;
    }

    private FingerprintMap ReadFuncs(Type t, string name)
    {
        FingerprintMap r = [];
        foreach (var method in t.GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
            var attributes = (InstructionAttribute[])method.GetCustomAttributes(typeof(InstructionAttribute));
            if (attributes.Length == 0) 
                continue;
            var func = FungeFunc.Create(method, null);
            foreach (var attribute in attributes)
            {
                int? code = name != "Core" ? new FungeString(name).Handprint : null;
                r[attribute.Instruction] = new FungeInstruction(func, $"{name}::{attribute.Instruction}", t,
                    code);
            }
        }
        foreach (var f in t.GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            var attributes = (InstructionAttribute[])f.GetCustomAttributes(typeof(InstructionAttribute));
            foreach (var attribute in attributes)
            {
                if (f.GetValue(null) is not FungeFunc func) 
                    continue;
                int? code = name != "Core" ? new FungeString(name).Handprint : null;
                r[attribute.Instruction] = new FungeInstruction(func, $"{name}::{attribute.Instruction}", t,
                    code);
            }
        }
        return r;
    }

    public InstructionRegistry(Interpreter interpreter) 
    {
        CoreInstructions = ReadFuncs(typeof(CoreInstructions), "");
        foreach (var t in GetType().Assembly.GetTypes())
        {
            if (t.GetCustomAttribute<FingerprintAttribute>() is not { } attribute)
                continue;
            RegisterFingerprint(attribute.Name, t);
        }
        _interpreter = interpreter;
    }
}