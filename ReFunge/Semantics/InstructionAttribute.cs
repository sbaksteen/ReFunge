
using JetBrains.Annotations;

namespace ReFunge.Semantics;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = true)]
[MeansImplicitUse(ImplicitUseKindFlags.Access)]

public sealed class InstructionAttribute(char instruction) : Attribute
{
    public char Instruction { get; } = instruction;
}