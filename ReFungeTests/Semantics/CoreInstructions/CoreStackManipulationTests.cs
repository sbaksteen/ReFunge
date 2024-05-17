using System.Diagnostics;
using ReFunge.Data;
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
        
        [Test]
        public void ClearStack_ClearsStack()
        {
            ip2D.PushToStack(5);
            ip2D.PushToStack(3);
            ip2D.PushToStack(7);
            ip2D.PushToStack(2);
            ip2D.PushToStack(1);
            ip2D.PushToStack(9);
            CoreInstructions.ClearStack.Execute(ip2D);
            Assert.That(ip2D.StackStack.TOSS.Size, Is.EqualTo(0));
        }

        [Test]
        public void BeginBlock_PushesNewStack()
        {
            ip2D.PushToStack(0);
            CoreInstructions.BeginBlock.Execute(ip2D);
            Assert.That(ip2D.StackStack.Size, Is.EqualTo(2));
        }

        [Test]
        public void BeginBlock_UpdatesStorageOffset()
        {
            ip2D.PushToStack(0);
            CoreInstructions.BeginBlock.Execute(ip2D);
            Assert.That(ip2D.StorageOffset, Is.EqualTo(new FungeVector(1, 0)));
        }

        [Test]
        public void BeginBlock_PushesStorageOffsetToSOSS()
        {
            ip2D.PushToStack(0);
            CoreInstructions.BeginBlock.Execute(ip2D);
            Debug.Assert(ip2D.StackStack.SOSS != null, "ip2D.StackStack.SOSS != null");
            Assert.That(ip2D.StackStack.SOSS.Size, Is.EqualTo(2));
            Assert.That(ip2D.StackStack.SOSS.Pop(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip2D.StackStack.SOSS.Pop(), Is.EqualTo(new FungeInt(0)));
        }

        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(50u)]
        public void BeginBlock_PushesZeroesToSOSS_IfArgumentNegative(uint n)
        {
            ip2D.PushToStack((int)-n);
            CoreInstructions.BeginBlock.Execute(ip2D);
            Debug.Assert(ip2D.StackStack.SOSS != null, "ip2D.StackStack.SOSS != null");
            Assert.That(ip2D.StackStack.SOSS.Size, Is.EqualTo(n + 2));
        }

        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(50u)]
        public void BeginBlock_TransfersValuesToNewStack(uint n)
        {
            for (var i = 0; i < n; i++)
            {
                ip2D.PushToStack(i);
            }
            ip2D.PushToStack((int)n);
            CoreInstructions.BeginBlock.Execute(ip2D);
            Debug.Assert(ip2D.StackStack.SOSS != null, "ip2D.StackStack.SOSS != null");
            Assert.That(ip2D.StackStack.SOSS.Size, Is.EqualTo(2));
            Assert.That(ip2D.StackStack.TOSS.Size, Is.EqualTo(n));
            for (var i = 0; i < n; i++)
            {
                Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt((int)n-i-1)));
            }
        }
        
        [Test]
        public void EndBlock_PopsStack()
        {
            ip2D.StackStack.PushStack(new FungeStack());
            CoreInstructions.EndBlock.Execute(ip2D);
            Assert.That(ip2D.StackStack.Size, Is.EqualTo(1));
        }
        
        [Test]
        public void EndBlock_UpdatesStorageOffset()
        {
            ip2D.StackStack.PushStack(new FungeStack());
            ip2D.StackStack.SOSS.Push(5);
            ip2D.StackStack.SOSS.Push(3);
            CoreInstructions.EndBlock.Execute(ip2D);
            Assert.That(ip2D.StorageOffset, Is.EqualTo(new FungeVector(5, 3)));
        }
        
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(50u)]
        public void EndBlock_TransfersValuesToPreviousStack(uint n)
        {
            ip2D.StackStack.PushStack(new FungeStack());
            for (int i = 0; i < n; i++)
            {
                ip2D.PushToStack(i);
            }
            ip2D.PushToStack((int)n);
            CoreInstructions.EndBlock.Execute(ip2D);
            Assert.That(ip2D.StackStack.TOSS.Size, Is.EqualTo(n));
            for (int i = 0; i < n; i++)
            {
                Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt((int)n-i-1)));
            }
        }
        
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(50u)]
        public void EndBlock_PopsValuesFromSOSS_IfArgumentNegative(uint n)
        {
            ip2D.StackStack.PushStack(new FungeStack());
            for (int i = 0; i < 102; i++)
            {
                ip2D.StackStack.SOSS.Push(i);
            }
            ip2D.PushToStack((int)-n);
            CoreInstructions.EndBlock.Execute(ip2D);
            Assert.That(ip2D.StackStack.TOSS.Size, Is.EqualTo(100 - n));
        }

        [Test]
        public void EndBlock_Reflects_IfOnlyOneStackExists()
        {
            CoreInstructions.EndBlock.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(-1, 0)));
            Assert.That(ip2D.StackStack.Size, Is.EqualTo(1));
        }

        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(50u)]
        public void StackTransfer_TransfersElements_WithPositiveArgument(uint n)
        {
            ip2D.StackStack.PushStack(new FungeStack());
            for (int i = 0; i < n; i++)
            {
                ip2D.StackStack.SOSS.Push(i);
            }
            ip2D.PushToStack((int)n);
            CoreInstructions.StackTransfer.Execute(ip2D);
            Assert.That(ip2D.StackStack.TOSS.Size, Is.EqualTo(n));
            for (int i = 0; i < n; i++)
            {
                Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(i)));
            }
        }
        
        [TestCase(1u)]
        [TestCase(2u)]
        [TestCase(50u)]
        public void StackTransfer_TransfersElements_WithNegativeArgument(uint n)
        {
            ip2D.StackStack.PushStack(new FungeStack());
            for (int i = 0; i < n; i++)
            {
                ip2D.StackStack.TOSS.Push(i);
            }
            ip2D.PushToStack((int)-n);
            CoreInstructions.StackTransfer.Execute(ip2D);
            Assert.That(ip2D.StackStack.SOSS.Size, Is.EqualTo(n));
            for (int i = 0; i < n; i++)
            {
                Assert.That(ip2D.StackStack.SOSS.Pop(), Is.EqualTo(new FungeInt(i)));
            }
        }
        
        [Test]
        public void StackTransfer_Reflects_IfOnlyOneStackExists()
        {
            CoreInstructions.StackTransfer.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(-1, 0)));
            Assert.That(ip2D.StackStack.Size, Is.EqualTo(1));
        }
        
        [Test]
        public void StackTransfer_DoesNothing_IfArgumentIsZero()
        {
            ip2D.StackStack.PushStack(new FungeStack());
            ip2D.PushToStack(0);
            CoreInstructions.StackTransfer.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(1, 0)));
            Assert.That(ip2D.StackStack.TOSS.Size, Is.EqualTo(0));
            Assert.That(ip2D.StackStack.SOSS.Size, Is.EqualTo(0));
        }
    }
}