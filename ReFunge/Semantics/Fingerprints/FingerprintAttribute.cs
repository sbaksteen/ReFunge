using JetBrains.Annotations;

namespace ReFunge.Semantics.Fingerprints;

public enum FingerprintType
{
    Static,
    InstancedPerInterpreter,
    InstancedPerSpace,
    InstancedPerIP
}

[AttributeUsage(AttributeTargets.Class)]
[MeansImplicitUse(ImplicitUseKindFlags.Access)]
public sealed class FingerprintAttribute(string name, FingerprintType type = FingerprintType.Static) : Attribute
{
    public string Name { get; } = name;
    public FingerprintType Type { get; } = type;
}