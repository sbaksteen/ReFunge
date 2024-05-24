using ReFunge.Data.Values;
using ReFunge.Semantics.Fingerprints;
using System.Reflection;


namespace ReFunge.Semantics;

using InstructionMap = Dictionary<FungeInt, FungeFunc>;

public class InstructionRegistry
{
    private static readonly Lazy<InstructionRegistry> Lazy = new(() => new InstructionRegistry());

    public static InstructionRegistry Instance => Lazy.Value;

    private readonly InstructionMap _coreInstructions = [];
    
    private readonly Dictionary<FungeFunc, string> _instructionNames = [];
    
    public static string NameOf(FungeFunc func)
    {
        return Instance._instructionNames[func];
    }

    internal static InstructionMap CoreInstructions { get { return Instance._coreInstructions; } }

    private readonly Dictionary<FungeInt, InstructionMap> _fingerprints = [];

    public static void RegisterNewFingerprint(FungeString name, Type t)
    {
        Instance.RegisterFingerprint(name, t);
    }

    private void RegisterFingerprint(FungeString name, Type t)
    {
        _fingerprints[name.Handprint] = ReadFuncs(t, name);
    }

    public static InstructionMap GetFingerprint(FungeInt code)
    {
        return Instance._fingerprints[code];
    }

    public static InstructionMap GetFingerprint(FungeString name)
    {
        return Instance._fingerprints[name.Handprint];
    }

    private InstructionMap ReadFuncs(Type t, string name)
    {
        InstructionMap r = [];
        foreach (var f in t.GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            var attributes = (InstructionAttribute[])f.GetCustomAttributes(typeof(InstructionAttribute));
            foreach (var attribute in attributes)
            {
                if (f.GetValue(null) is FungeFunc func)
                {
                    r[attribute.Instruction] = func;
                    _instructionNames[func] = $"{name}::{attribute.Instruction}";
                }
            }
        }
        return r;
    }

    private InstructionRegistry() 
    {
        _coreInstructions = ReadFuncs(typeof(CoreInstructions), "");
        RegisterFingerprint("NULL", typeof(NULL));
        RegisterFingerprint("ROMA", typeof(ROMA));
        RegisterFingerprint("MODU", typeof(MODU));
        RegisterFingerprint("MODE", typeof(MODE));
        RegisterFingerprint("HRTI", typeof(HRTI));
    }
}