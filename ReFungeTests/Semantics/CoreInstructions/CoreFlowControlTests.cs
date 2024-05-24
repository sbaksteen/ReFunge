using ReFunge.Data.Values;
using ReFunge.Semantics;

namespace ReFungeTests.Semantics;

internal partial class CoreInstructionsTests
{
    [TestFixture]
    internal class CoreFlowControlTests : CoreInstructionsTests
    {
        [Test]
        public void LeftAndRight_SetDeltaCorrectly()
        {
            // Left and right should work in all dimensions
            ip1D.DoOp('<');
            Assert.That(ip1D.Delta, Is.EqualTo(FungeVector.Left));
            ip1D.DoOp('>');
            Assert.That(ip1D.Delta, Is.EqualTo(FungeVector.Right));
                
            ip2D.DoOp('<');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Left));
            ip2D.DoOp('>');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Right));

            ip3D.DoOp('<');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Left));
            ip3D.DoOp('>');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Right));
        }

        [Test]
        public void UpAndDown_SetDeltaCorrectly()
        {
            // Up and down should work in 2D and 3D
            ip2D.DoOp('^');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Up));
            ip2D.DoOp('v');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Down));

            ip3D.DoOp('^');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Up));
            ip3D.DoOp('v');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Down));

        }

        [Test]
        public void UpAndDown_Reflect_In1D()
        {
            FungeVector d = ip1D.Delta;
            ip1D.DoOp('^');
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
            ip1D.DoOp('v');
            Assert.That(ip1D.Delta, Is.EqualTo(d));
        }

        [Test]
        public void ForwardAndBackwards_SetDeltaCorrectly()
        {
            ip3D.DoOp('h');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Forwards));
            ip3D.DoOp('l');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Backwards));

        }

        [Test]
        public void ForwardAndBackwards_Reflect_In1DAnd2D()
        {
            FungeVector d = ip1D.Delta;
            ip1D.DoOp('h');
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
            ip1D.DoOp('l');
            Assert.That(ip1D.Delta, Is.EqualTo(d));

            d = ip2D.Delta;
            ip2D.DoOp('h');
            Assert.That(ip2D.Delta, Is.EqualTo(-d));
            ip2D.DoOp('l');
            Assert.That(ip2D.Delta, Is.EqualTo(d));
        }

        [Test]
        public void RandomDelta_VisitsAllPossibleValues()
        {
            // Test that all possible values are visited
            HashSet<FungeVector> toVisit1D = [
                FungeVector.Right,
                FungeVector.Left
            ];
            HashSet<FungeVector> toVisit2D = [
                FungeVector.Right,
                FungeVector.Left,
                FungeVector.Up,
                FungeVector.Down
            ];
            HashSet<FungeVector> toVisit3D = [
                FungeVector.Right,
                FungeVector.Left,
                FungeVector.Up,
                FungeVector.Down,
                FungeVector.Forwards,
                FungeVector.Backwards
            ];
            HashSet<FungeVector> visited = new();
            for (int i = 0; i < 100; i++)
            {
                ip1D.DoOp('?');
                visited.Add(ip1D.Delta);
            }
            Assert.That(visited.SetEquals(toVisit1D));

            visited.Clear();
            for (int i = 0; i < 100; i++)
            {
                ip2D.DoOp('?');
                visited.Add(ip2D.Delta);
            }
            Assert.That(visited.SetEquals(toVisit2D));

            visited.Clear();
            for (int i = 0; i < 100; i++)
            {
                ip3D.DoOp('?');
                visited.Add(ip3D.Delta);
            }
            Assert.That(visited.SetEquals(toVisit3D));
        }

        [Test]
        public void DecideHorizontal_GoesRight_ForZeroArgument()
        {
            // Should go right if top of stack is zero
            ip1D.Delta = FungeVector.Left;
            ip1D.PushToStack(0);
            ip1D.DoOp('_');
            Assert.That(ip1D.Delta, Is.EqualTo(FungeVector.Right));

            // Should also work in 2D and 3D
            ip2D.Delta = FungeVector.Up;
            ip2D.PushToStack(0);
            ip2D.DoOp('_');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Right));

            ip3D.Delta = FungeVector.Forwards;
            ip3D.PushToStack(0);
            ip3D.DoOp('_');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Right));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-242)]
        [TestCase(-1)]
        public void DecideHorizontal_GoesLeft_ForNonZeroArgument(int n)
        {
            // Should go left if top of stack is non-zero
            ip1D.Delta = FungeVector.Right;
            ip1D.PushToStack(n);
            ip1D.DoOp('_');
            Assert.That(ip1D.Delta, Is.EqualTo(FungeVector.Left));

            // Should also work in 2D and 3D
            ip2D.Delta = FungeVector.Down;
            ip2D.PushToStack(n);
            ip2D.DoOp('_');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Left));

            ip3D.Delta = FungeVector.Backwards;
            ip3D.PushToStack(n);
            ip3D.DoOp('_');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Left));
        }

        [Test]
        public void DecideVertical_GoesDown_ForZeroArgument()
        {
            // Should go down if top of stack is zero
            ip2D.Delta = FungeVector.Up;
            ip2D.PushToStack(0);
            ip2D.DoOp('|');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Down));

            // Should also work in 3D
            ip3D.Delta = FungeVector.Forwards;
            ip3D.PushToStack(0);
            ip3D.DoOp('|');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Down));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-242)]
        [TestCase(-1)]
        public void DecideVertical_GoesUp_ForNonZeroArgument(int n)
        {
            // Should go up if top of stack is non-zero
            ip2D.Delta = FungeVector.Down;
            ip2D.PushToStack(n);
            ip2D.DoOp('|');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Up));

            // Should also work in 3D
            ip3D.Delta = FungeVector.Backwards;
            ip3D.PushToStack(n);
            ip3D.DoOp('|');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Up));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-242)]
        [TestCase(-1)]
        [TestCase(0)]
        public void DecideHorizontal_Reflects_In1D(int n)
        {
            FungeVector d = ip1D.Delta;
            ip1D.PushToStack(n);
            ip1D.DoOp('|');
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
        }

        [Test]
        public void DecideForwards_GoesBackwards_ForZeroArgument()
        {
            // Should go backwards if top of stack is zero
            ip3D.Delta = FungeVector.Forwards;
            ip3D.PushToStack(0);
            ip3D.DoOp('m');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Backwards));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-242)]
        [TestCase(-1)]
        public void DecideForwards_GoesForwards_ForNonZeroArgument(int n)
        {
            // Should go forwards if top of stack is non-zero
            ip3D.Delta = FungeVector.Backwards;
            ip3D.PushToStack(n);
            ip3D.DoOp('m');
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.Forwards));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-242)]
        [TestCase(-1)]
        [TestCase(0)]
        public void DecideForwards_Reflects_In1DAnd2D(int n)
        {
            FungeVector d = ip1D.Delta;
            ip1D.PushToStack(n);
            ip1D.DoOp('m');
            Assert.That(ip1D.Delta, Is.EqualTo(-d));

            d = ip2D.Delta;
            ip2D.PushToStack(n);
            ip2D.DoOp('m');
            Assert.That(ip2D.Delta, Is.EqualTo(-d));
        }

        [TestCase(1, 0, 0)]
        [TestCase(5, 0, 0)]
        [TestCase(3, -3, 2)]
        [TestCase(0, -1, -4)]
        public void Reflect_Reflects(int x, int y, int z)
        {
            var d = new FungeVector(x, y, z);
            ip3D.Delta = d;
            ip3D.DoOp('r');
            Assert.That(ip3D.Delta, Is.EqualTo(-d));
        }
            
        [TestCase(1, 0, 0)]
        [TestCase(5, 0, 0)]
        [TestCase(3, -3, 2)]
        [TestCase(0, -1, -4)]
        public void Skip_SetsPositionCorrectly(int x, int y, int z)
        {
            var d = new FungeVector(x, y, z);
            ip3D.Delta = d;
            var p = ip3D.Position;
            ip3D.DoOp('#');
            Assert.That(ip3D.Position, Is.EqualTo(p + d));
        }
            
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-242)]
        [TestCase(-1)]
        [TestCase(0)]
        public void JumpForward_SetsPositionCorrectly(int n)
        {
            var d = new FungeVector(1, 0, 0);
            ip3D.Delta = d;
            ip3D.PushToStack(n);
            ip3D.DoOp('j');
            Assert.That(ip3D.Position, Is.EqualTo(new FungeVector(n, 0, 0)));
        }
            
        [TestCase(1, 0, 0)]
        [TestCase(5, 0, 0)]
        [TestCase(3, -3, 2)]
        [TestCase(0, -1, -4)]
        public void SetDelta_SetsDeltaCorrectly(int x, int y, int z)
        {
            ip3D.PushToStack(x);
            ip3D.PushToStack(y);
            ip3D.PushToStack(z);
            ip3D.DoOp('x');
            Assert.That(ip3D.Delta, Is.EqualTo(new FungeVector(x, y, z)));
        }
            
        [TestCase(1, 0)]
        [TestCase(5, 0)]
        [TestCase(3, -3)]
        [TestCase(0, -6)]
        public void SetDelta_SetsDeltaCorrectly_2D(int x, int y)
        {
            ip2D.PushToStack(x);
            ip2D.PushToStack(y);
            ip2D.DoOp('x');
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(x, y)));
        }
            
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-242)]
        public void SetDelta_SetsDeltaCorrectly_1D(int x)
        {
            ip1D.PushToStack(x);
            ip1D.DoOp('x');
            Assert.That(ip1D.Delta, Is.EqualTo(new FungeVector(x)));
        }
            
        [Test]
        public void Iterate_RepeatsCorrectly_NextCell()
        {
            ip1D.Space.LoadString(new FungeVector(), "k3");
            ip1D.PushToStack(3);
            ip1D.DoOp('k');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(3)));
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(3)));
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(3)));
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector()));
        }

        [Test]
        public void Iterate_RepeatsCorrectly_Wrapping()
        {
            ip1D.Space.LoadString(new FungeVector(-1), "3k");
            ip1D.PushToStack(3);
            ip1D.DoOp('k');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(3)));
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(3)));
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(3)));
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector()));
        }
            
        [Test]
        public void Iterate_RepeatsCorrectly_WithSpacesAndComments()
        {
            ip1D.Space.LoadString(new FungeVector(), "k    ;;;3");
            ip1D.PushToStack(3);
            ip1D.DoOp('k');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(3)));
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(3)));
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(3)));
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector()));
        }
            
        [Test]
        public void Iterate_SkipsInstruction_IfZero()
        {
            ip1D.Space.LoadString(new FungeVector(), "k3");
            ip1D.PushToStack(0);
            ip1D.DoOp('k');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector(1)));
        }
            
        [Test]
        public void Iterate_SkipsInstruction_IfNegative()
        {
            ip1D.Space.LoadString(new FungeVector(), "k3");
            ip1D.PushToStack(-1);
            ip1D.DoOp('k');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector(1)));
        }
            
        [Test]
        public void Iterate_SkipsInstruction_IfZero_Wrapping()
        {
            ip1D.Space.LoadString(new FungeVector(-1), "3k");
            ip1D.PushToStack(0);
            ip1D.DoOp('k');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector(-1)));
        }
            
        [Test]
        public void Iterate_SkipsInstruciton_IfZero_WithSpacesAndComments()
        {
            ip1D.Space.LoadString(new FungeVector(), "k    ;;;3");
            ip1D.PushToStack(0);
            ip1D.DoOp('k');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector(8)));
        }
            
        [Test]
        public void Iterate_IteratesCorrectly_WithSkipInstruction()
        {
            ip1D.Space.LoadString(new FungeVector(), "k#");
            ip1D.PushToStack(4);
            ip1D.DoOp('k');
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector(4)));
        }
            
        [Test]
        public void TurnRight_TurnsRight_CardinalDirection()
        {
            ip2D.Delta = FungeVector.Up;
            ip2D.DoOp(']');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Right));
        }
            
        [Test]
        public void TurnRight_TurnsRight_NonCardinalDirection()
        {
            ip2D.Delta = new FungeVector(3, -2);
            ip2D.DoOp(']');
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(2, 3)));
        }
            
        [Test]
        public void TurnRight_TurnsRight_In3D()
        {
            ip3D.Delta = new FungeVector(3, -2, 5);
            ip3D.DoOp(']');
            Assert.That(ip3D.Delta, Is.EqualTo(new FungeVector(2, 3, 5)));
        }
            
        [Test]
        public void TurnRight_Reflects_In1D()
        {
            FungeVector d = ip1D.Delta;
            ip1D.DoOp(']');
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
        }
            
        [Test]
        public void TurnLeft_TurnsLeft_CardinalDirection()
        {
            ip2D.Delta = FungeVector.Up;
            ip2D.DoOp('[');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Left));
        }
            
        [Test]
        public void TurnLeft_TurnsLeft_NonCardinalDirection()
        {
            ip2D.Delta = new FungeVector(3, -2);
            ip2D.DoOp('[');
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(-2, -3)));
        }
            
        [Test]
        public void TurnLeft_TurnsLeft_In3D()
        {
            ip3D.Delta = new FungeVector(3, -2, 5);
            ip3D.DoOp('[');
            Assert.That(ip3D.Delta, Is.EqualTo(new FungeVector(-2, -3, 5)));
        }
            
        [Test]
        public void TurnLeft_Reflects_In1D()
        {
            FungeVector d = ip1D.Delta;
            ip1D.DoOp('[');
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
        }
            
        [Test]
        public void Compare_TurnsRight_IfFirstArgumentGreater()
        {
            ip2D.PushToStack(5);
            ip2D.PushToStack(3);
            ip2D.DoOp('w');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Down));
        }
            
        [Test]
        public void Compare_TurnsLeft_IfFirstArgumentLesser()
        {
            ip2D.PushToStack(3);
            ip2D.PushToStack(5);
            ip2D.DoOp('w');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Up));
        }
            
        [Test]
        public void Compare_DoesNothing_IfEqual()
        {
            ip2D.PushToStack(5);
            ip2D.PushToStack(5);
            ip2D.DoOp('w');
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.Right));
        }
    }
}