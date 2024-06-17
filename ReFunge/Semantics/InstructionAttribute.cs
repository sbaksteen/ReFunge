using JetBrains.Annotations;

namespace ReFunge.Semantics;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = true)]
[MeansImplicitUse(ImplicitUseKindFlags.Access)]
public sealed class InstructionAttribute : Attribute
{
    
    public InstructionAttribute(char instruction, int minDimension = 1)
    {
        if (minDimension < 1)
            throw new ArgumentOutOfRangeException(nameof(minDimension), minDimension, "Minimum dimension must be at least 1.");
        Instruction = instruction;
        MinDimension = minDimension;
    }

    public char Instruction { get; private init; }
    public int MinDimension { get; private init; }
}