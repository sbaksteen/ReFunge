using ReFunge.Data.Values;
using ReFunge.Semantics.Fingerprints;
using System.Reflection;


namespace ReFunge.Semantics
{
    using InstructionMap = Dictionary<FungeInt, FungeFunc>;
    internal class InstructionRegistry
    {
        private static readonly Lazy<InstructionRegistry> lazy = new Lazy<InstructionRegistry>(() => new InstructionRegistry());

        public static InstructionRegistry Instance { get { return lazy.Value; } }

        private readonly InstructionMap coreInstructions = [];

        internal static InstructionMap CoreInstructions { get { return Instance.coreInstructions; } }

        private readonly Dictionary<FungeInt, InstructionMap> fingerprints = [];

        public static void RegisterFingerprint(FungeString name, Type t)
        {
            Instance.registerFingerprint(name, t);
        }

        private void registerFingerprint(FungeString name, Type t)
        {
            fingerprints[name.Handprint] = ReadFuncs(t);
        }

        public static InstructionMap GetFingerprint(FungeInt code)
        {
            return Instance.fingerprints[code];
        }

        public static InstructionMap GetFingerprint(FungeString name)
        {
            return Instance.fingerprints[name.Handprint];
        }

        private InstructionMap ReadFuncs(Type t)
        {
            InstructionMap r = [];
            foreach (var f in t.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var attributes = (InstructionAttribute[])f.GetCustomAttributes(typeof(InstructionAttribute));
                foreach (InstructionAttribute attribute in attributes)
                {
                    if (f.GetValue(null) is FungeFunc func)
                    {
                        r[attribute.Instruction] = func;
                    }
                }
            }
            return r;
        }

        private InstructionRegistry() 
        {
            coreInstructions = ReadFuncs(typeof(CoreInstructions));
            registerFingerprint("NULL", typeof(NULL));
            registerFingerprint("ROMA", typeof(ROMA));
        }
    }
}
