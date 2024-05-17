using ReFunge.Data.Values;
using ReFunge.Semantics;

namespace ReFungeTests.Semantics;

internal partial class CoreInstructionsTests
{
    [TestFixture]
    internal class CoreStackManipulationTests : CoreInstructionsTests
    {
        [Test]
        public void Push0_Pushes0()
        {
            CoreInstructions.Push0.Execute(ip2D);
            Assert.That(ip2D.StackStack.TOSS.Size, Is.EqualTo(1));
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
        }
        
        [Test]
        public void Push1_Pushes1()
        {
            CoreInstructions.Push1.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(1)));
        }
        
        [Test]
        public void Push15_Pushes15()
        {
            CoreInstructions.Push15.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(15)));
        }
        
        // Consider PushNumber to be tested now...
        
        [Test]
        public void Discard_DiscardsTopValue()
        {
            ip2D.PushToStack(5);
            CoreInstructions.Discard.Execute(ip2D);
            var poppedValue = ip2D.PopFromStack();
            Assert.That(ip2D.StackStack.TOSS.Size, Is.EqualTo(0));
        }
        
        [Test]
        public void Swap_SwapsTopTwoValues()
        {
            ip2D.PushToStack(5);
            ip2D.PushToStack(3);
            CoreInstructions.Swap.Execute(ip2D);
            var poppedValue1 = ip2D.PopFromStack();
            var poppedValue2 = ip2D.PopFromStack();
            Assert.That(ip2D.StackStack.TOSS.Size, Is.EqualTo(0));
            Assert.That(poppedValue1, Is.EqualTo(new FungeInt(5)));
            Assert.That(poppedValue2, Is.EqualTo(new FungeInt(3)));
        }
        
        [Test]
        public void Duplicate_DuplicatesTopValue()
        {
            ip2D.PushToStack(5);
            CoreInstructions.Duplicate.Execute(ip2D);
            var poppedValue1 = ip2D.PopFromStack();
            var poppedValue2 = ip2D.PopFromStack();
            Assert.That(ip2D.StackStack.TOSS.Size, Is.EqualTo(0));
            Assert.That(poppedValue1, Is.EqualTo(new FungeInt(5)));
            Assert.That(poppedValue2, Is.EqualTo(new FungeInt(5)));
        }
    }
}