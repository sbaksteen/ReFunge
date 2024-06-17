using JetBrains.Annotations;
using ReFunge.Data.Values;

namespace ReFunge.Semantics;

/// <summary>
///     <p>
///         An attribute that marks a method or field as an instruction for a Funge fingerprint or other instruction set,
///         which can be invoked by an IP when it encounters the corresponding instruction character.
///         When multiple <see cref="InstructionAttribute" />s are applied to a method or field, it will be associated with
///         each of the characters in the attributes.
///     </p>
///     <p>
///         A method marked with this attribute must have a signature that matches the following:
///         <list type="bullet">
///             <item>
///                 <description>
///                     The return type must either be void or
///                     implement <see cref="IFungeValue{TSelf}" />;
///                 </description>
///             </item>
///             <item>
///                 <description>The first argument must be a <see cref="FungeIP" />;</description>
///             </item>
///             <item>
///                 <description>All other arguments must implement <see cref="IFungeValue{TSelf}" />.</description>
///             </item>
///         </list>
///         A field marked with this attribute must be a <see cref="FungeFunc" />.
///     </p>
/// </summary>
/// <seealso cref="FungeFunc" />
/// <seealso cref="FungeInstruction" />
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = true)]
[MeansImplicitUse(ImplicitUseKindFlags.Access)]
public sealed class InstructionAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="InstructionAttribute" /> class.
    /// </summary>
    /// <param name="instruction">The character associated with the instruction.</param>
    /// <param name="minDimension">
    ///     The minimum amount of dimensions the IP must be able to interface with to execute this
    ///     instruction.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="minDimension" /> is less than 1.</exception>
    public InstructionAttribute(char instruction, int minDimension = 1)
    {
        if (minDimension < 1)
            throw new ArgumentOutOfRangeException(nameof(minDimension), minDimension,
                "Minimum dimension must be at least 1.");
        Instruction = instruction;
        MinDimension = minDimension;
    }

    /// <summary>
    ///     The character associated with the instruction.
    /// </summary>
    public char Instruction { get; private init; }

    /// <summary>
    ///     The minimum amount of dimensions the IP must be able to interface with to execute this instruction.
    /// </summary>
    public int MinDimension { get; private init; }
}