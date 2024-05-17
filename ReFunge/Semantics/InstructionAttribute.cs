
namespace ReFunge.Semantics
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    internal sealed class InstructionAttribute(char instruction) : Attribute
    {
        public char Instruction { get; } = instruction;
    }
}