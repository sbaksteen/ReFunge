using JetBrains.Annotations;

namespace ReFunge.Semantics.Fingerprints;

/// <summary>
///     The type of fingerprint.
/// </summary>
public enum FingerprintType
{
    /// <summary>
    ///     A fingerprint with no internal state, which therefore does not need to be instantiated.
    /// </summary>
    Static,

    /// <summary>
    ///     A fingerprint that is instantiated once per interpreter.
    /// </summary>
    InstancedPerInterpreter,

    /// <summary>
    ///     A fingerprint that is instantiated once per space.
    /// </summary>
    InstancedPerSpace,

    /// <summary>
    ///     A fingerprint that is instantiated once per IP.
    /// </summary>
    InstancedPerIP
}

/// <summary>
///     An attribute that marks a class as a fingerprint for a Funge interpreter.
/// </summary>
/// <param name="name">The name of the fingerprint. This also determines the code (handprint) used to access it.</param>
/// <param name="type">The type of fingerprint.</param>
[AttributeUsage(AttributeTargets.Class)]
[MeansImplicitUse(ImplicitUseKindFlags.Access)]
public sealed class FingerprintAttribute(string name, FingerprintType type = FingerprintType.Static) : Attribute
{
    /// <summary>
    ///     The name of the fingerprint.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    ///     The type of fingerprint.
    /// </summary>
    public FingerprintType Type { get; } = type;
}