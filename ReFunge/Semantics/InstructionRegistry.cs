using System.Reflection;
using ReFunge.Data;
using ReFunge.Data.Values;
using ReFunge.Semantics.Fingerprints;

namespace ReFunge.Semantics;

using InstructionMap = Dictionary<FungeInt, FungeInstruction>;

/// <summary>
///     A registry of all instructions available to the interpreter. This includes core instructions and fingerprints.
/// </summary>
public class InstructionRegistry
{
    private readonly Interpreter _interpreter;

    private readonly Dictionary<FungeInt, InstancedFingerprint> _interpreterFingerprints = [];
    private readonly Dictionary<FungeInt, Type> _ipFingerprints = [];
    private readonly Dictionary<FungeInt, Type> _spaceFingerprints = [];

    private readonly Dictionary<FungeInt, InstructionMap> _staticFingerprints = [];

    /// <summary>
    ///     Create a new instruction registry for the given interpreter, populating it with the core instructions
    ///     and all fingerprints found in the ReFunge assembly.
    /// </summary>
    /// <param name="interpreter">The interpreter to create the registry for.</param>
    public InstructionRegistry(Interpreter interpreter)
    {
        CoreInstructions = ReadFuncs(typeof(CoreInstructions), "Core");
        foreach (var t in GetType().Assembly.GetTypes())
        {
            if (t.GetCustomAttribute<FingerprintAttribute>() is null)
                continue;
            RegisterFingerprint(t);
        }

        _interpreter = interpreter;
    }

    internal InstructionMap CoreInstructions { get; }

    /// <summary>
    ///     Register a fingerprint represented by the given type. The type must have a FingerprintAttribute.
    ///     All instructions in the fingerprint marked with <see cref="InstructionAttribute" />s will be added to the registry.
    /// </summary>
    /// <param name="t">The type representing the fingerprint.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the type does not have a <see cref="FingerprintAttribute" />
    ///     or has an invalid <see cref="FingerprintType" />.
    /// </exception>
    public void RegisterFingerprint(Type t)
    {
        if (t.GetCustomAttribute<FingerprintAttribute>() is not { } attribute)
            throw new InvalidOperationException("Fingerprint must have a FingerprintAttribute");
        var code = new FungeString(attribute.Name).Handprint;
        switch (attribute.Type)
        {
            case FingerprintType.Static:
                _staticFingerprints[code] = ReadFuncs(t, attribute.Name);
                return;
            case FingerprintType.InstancedPerInterpreter:
                _interpreterFingerprints[code] =
                    (Activator.CreateInstance(t, [_interpreter]) as InstancedFingerprint)!;
                return;
            case FingerprintType.InstancedPerSpace:
                _spaceFingerprints[code] = t;
                return;
            case FingerprintType.InstancedPerIP:
                _ipFingerprints[code] = t;
                return;
            default:
                throw new InvalidOperationException("Unknown fingerprint type");
        }
    }

    /// <summary>
    ///     Get the type of fingerprint represented by the given code.
    /// </summary>
    /// <param name="code">The code of the fingerprint.</param>
    /// <returns>The type of fingerprint.</returns>
    /// <exception cref="ArgumentException">Thrown when the fingerprint is not found.</exception>
    public FingerprintType TypeOf(FungeInt code)
    {
        if (_staticFingerprints.ContainsKey(code)) return FingerprintType.Static;
        if (_interpreterFingerprints.ContainsKey(code)) return FingerprintType.InstancedPerInterpreter;
        if (_spaceFingerprints.ContainsKey(code)) return FingerprintType.InstancedPerSpace;
        if (_ipFingerprints.ContainsKey(code)) return FingerprintType.InstancedPerIP;

        throw new ArgumentException($"Fingerprint {code} not found");
    }

    /// <summary>
    ///     Create a new instance of the IP-instanced fingerprint represented by the given code.
    /// </summary>
    /// <param name="code">The code of the fingerprint.</param>
    /// <param name="ip">The IP to create the fingerprint for.</param>
    /// <returns>The new fingerprint instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the fingerprint is not found.</exception>
    public InstancedFingerprint NewInstance(FungeInt code, FungeIP ip)
    {
        if (_ipFingerprints[code] is not { } fingerprintType)
            throw new ArgumentException($"Fingerprint {code} not found");

        return (Activator.CreateInstance(fingerprintType, [ip]) as InstancedFingerprint)!;
    }

    /// <summary>
    ///     Create a new instance of the space-instanced fingerprint represented by the given code.
    /// </summary>
    /// <param name="code">The code of the fingerprint.</param>
    /// <param name="space">The space to create the fingerprint for.</param>
    /// <returns>The new fingerprint instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the fingerprint is not found.</exception>
    public InstancedFingerprint NewInstance(FungeInt code, FungeSpace space)
    {
        if (_spaceFingerprints[code] is not { } fingerprintType)
            throw new ArgumentException($"Fingerprint {code} not found");

        return (Activator.CreateInstance(fingerprintType, [space]) as InstancedFingerprint)!;
    }

    /// <summary>
    ///     Get the static fingerprint represented by the given code.
    /// </summary>
    /// <param name="code">The code of the fingerprint.</param>
    /// <returns>The static fingerprint's instruction map.</returns>
    public InstructionMap GetStaticFingerprint(FungeInt code)
    {
        return _staticFingerprints[code];
    }

    /// <summary>
    ///     Get the interpreter-instanced fingerprint represented by the given code.
    /// </summary>
    /// <param name="code">The code of the fingerprint.</param>
    /// <returns>The fingerprint instance's instruction map.</returns>
    public InstructionMap GetInterpreterFingerprint(FungeInt code)
    {
        return _interpreterFingerprints[code].Instructions;
    }

    private static InstructionMap ReadFuncs(Type t, string name)
    {
        InstructionMap r = [];
        foreach (var method in t.GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
            var attributes = (InstructionAttribute[])method.GetCustomAttributes(typeof(InstructionAttribute));
            if (attributes.Length == 0)
                continue;
            var func = FungeFunc.Create(method, null);
            foreach (var attribute in attributes)
            {
                int? code = name != "Core" ? new FungeString(name).Handprint : null;
                r[attribute.Instruction] = new FungeInstruction(func, $"{name}::{attribute.Instruction}",
                    code, attribute.MinDimension);
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
                r[attribute.Instruction] = new FungeInstruction(func, $"{name}::{attribute.Instruction}",
                    code, attribute.MinDimension);
            }
        }

        return r;
    }
}