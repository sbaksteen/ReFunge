using ReFunge.Data.Values;
using ReFunge.Semantics;

namespace ReFungeTests.Semantics;

internal partial class CoreInstructionsTests
{
    [TestFixture]
    internal class CoreArithmeticTests : CoreInstructionsTests
    {
        [Test]
        public void Add_PushesCorrectSum()
        {
            ip2D.PushToStack(5);
            ip2D.PushToStack(3);
            CoreInstructions.Add.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(8)));
        }

        [Test]
        public void Sub_PushesCorrectDifference()
        {
            ip2D.PushToStack(5);
            ip2D.PushToStack(3);
            CoreInstructions.Subtract.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(2)));
        }

        [Test]
        public void Mul_PushesCorrectProduct()
        {
            ip2D.PushToStack(5);
            ip2D.PushToStack(3);
            CoreInstructions.Multiply.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(15)));
        }

        [Test]
        public void Div_PushesCorrectQuotient()
        {
            ip2D.PushToStack(43);
            ip2D.PushToStack(5);
            CoreInstructions.Divide.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(8)));
        }

        [Test]
        public void Div_PushesZero_ForZeroDivisor()
        { 
            ip2D.PushToStack(43);
            ip2D.PushToStack(0);
            CoreInstructions.Divide.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
        }

        [Test]
        public void Mod_PushesCorrectModulus()
        {
            ip2D.PushToStack(43);
            ip2D.PushToStack(5);
            CoreInstructions.Modulo.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(3)));
        }

        [Test]
        public void Mod_PushesZero_ForZeroDivisor() { 
            ip2D.PushToStack(43);
            ip2D.PushToStack(0);
            CoreInstructions.Modulo.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
        }

        [Test]
        public void Not_PushesOne_ForZeroArgument()
        {
            ip2D.PushToStack(0);
            CoreInstructions.LogicalNot.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(1)));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-324)]
        [TestCase(-1)]
        public void Not_PushesZero_ForNonZeroArgument(int n)
        {
            ip2D.PushToStack(n);
            CoreInstructions.LogicalNot.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
        }

        [Test]
        public void GreaterThan_PushesOne_IfFirstArgumentGreater()
        {
            ip2D.PushToStack(5);
            ip2D.PushToStack(3);
            CoreInstructions.GreaterThan.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(1)));

        }

        [Test]
        public void GreaterThan_PushesZero_IfFirstArgumentLesserOrEqual() 
        { 
            ip2D.PushToStack(3);
            ip2D.PushToStack(5);
            CoreInstructions.GreaterThan.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(0)));

            ip2D.PushToStack(5);
            ip2D.PushToStack(5);
            CoreInstructions.GreaterThan.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
        }
    }
}