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
            CoreInstructions.GoLeft.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(FungeVector.LEFT));
            CoreInstructions.GoRight.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(FungeVector.RIGHT));
                
            CoreInstructions.GoLeft.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.LEFT));
            CoreInstructions.GoRight.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.RIGHT));

            CoreInstructions.GoLeft.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.LEFT));
            CoreInstructions.GoRight.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.RIGHT));
        }

        [Test]
        public void UpAndDown_SetDeltaCorrectly()
        {
            // Up and down should work in 2D and 3D
            CoreInstructions.GoUp.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.UP));
            CoreInstructions.GoDown.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.DOWN));

            CoreInstructions.GoUp.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.UP));
            CoreInstructions.GoDown.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.DOWN));

        }

        [Test]
        public void UpAndDown_Reflect_In1D()
        {
            FungeVector d = ip1D.Delta;
            CoreInstructions.GoUp.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
            CoreInstructions.GoDown.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(d));
        }

        [Test]
        public void ForwardAndBackwards_SetDeltaCorrectly()
        {
            CoreInstructions.GoForwards.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.FORWARDS));
            CoreInstructions.GoBackwards.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.BACKWARDS));

        }

        [Test]
        public void ForwardAndBackwards_Reflect_In1DAnd2D()
        {
            FungeVector d = ip1D.Delta;
            CoreInstructions.GoForwards.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
            CoreInstructions.GoBackwards.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(d));

            d = ip2D.Delta;
            CoreInstructions.GoForwards.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(-d));
            CoreInstructions.GoBackwards.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(d));
        }

        [Test]
        public void RandomDelta_VisitsAllPossibleValues()
        {
            // Test that all possible values are visited
            HashSet<FungeVector> toVisit1D = [
                FungeVector.RIGHT,
                FungeVector.LEFT
            ];
            HashSet<FungeVector> toVisit2D = [
                FungeVector.RIGHT,
                FungeVector.LEFT,
                FungeVector.UP,
                FungeVector.DOWN
            ];
            HashSet<FungeVector> toVisit3D = [
                FungeVector.RIGHT,
                FungeVector.LEFT,
                FungeVector.UP,
                FungeVector.DOWN,
                FungeVector.FORWARDS,
                FungeVector.BACKWARDS
            ];
            HashSet<FungeVector> visited = new();
            for (int i = 0; i < 100; i++)
            {
                CoreInstructions.GoRandom.Execute(ip1D);
                visited.Add(ip1D.Delta);
            }
            Assert.That(visited.SetEquals(toVisit1D));

            visited.Clear();
            for (int i = 0; i < 100; i++)
            {
                CoreInstructions.GoRandom.Execute(ip2D);
                visited.Add(ip2D.Delta);
            }
            Assert.That(visited.SetEquals(toVisit2D));

            visited.Clear();
            for (int i = 0; i < 100; i++)
            {
                CoreInstructions.GoRandom.Execute(ip3D);
                visited.Add(ip3D.Delta);
            }
            Assert.That(visited.SetEquals(toVisit3D));
        }

        [Test]
        public void DecideHorizontal_GoesRight_ForZeroArgument()
        {
            // Should go right if top of stack is zero
            ip1D.Delta = FungeVector.LEFT;
            ip1D.PushToStack(0);
            CoreInstructions.DecideHorizontal.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(FungeVector.RIGHT));

            // Should also work in 2D and 3D
            ip2D.Delta = FungeVector.UP;
            ip2D.PushToStack(0);
            CoreInstructions.DecideHorizontal.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.RIGHT));

            ip3D.Delta = FungeVector.FORWARDS;
            ip3D.PushToStack(0);
            CoreInstructions.DecideHorizontal.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.RIGHT));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-242)]
        [TestCase(-1)]
        public void DecideHorizontal_GoesLeft_ForNonZeroArgument(int n)
        {
            // Should go left if top of stack is non-zero
            ip1D.Delta = FungeVector.RIGHT;
            ip1D.PushToStack(n);
            CoreInstructions.DecideHorizontal.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(FungeVector.LEFT));

            // Should also work in 2D and 3D
            ip2D.Delta = FungeVector.DOWN;
            ip2D.PushToStack(n);
            CoreInstructions.DecideHorizontal.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.LEFT));

            ip3D.Delta = FungeVector.BACKWARDS;
            ip3D.PushToStack(n);
            CoreInstructions.DecideHorizontal.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.LEFT));
        }

        [Test]
        public void DecideVertical_GoesDown_ForZeroArgument()
        {
            // Should go down if top of stack is zero
            ip2D.Delta = FungeVector.UP;
            ip2D.PushToStack(0);
            CoreInstructions.DecideVertical.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.DOWN));

            // Should also work in 3D
            ip3D.Delta = FungeVector.FORWARDS;
            ip3D.PushToStack(0);
            CoreInstructions.DecideVertical.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.DOWN));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-242)]
        [TestCase(-1)]
        public void DecideVertical_GoesUp_ForNonZeroArgument(int n)
        {
            // Should go up if top of stack is non-zero
            ip2D.Delta = FungeVector.DOWN;
            ip2D.PushToStack(n);
            CoreInstructions.DecideVertical.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.UP));

            // Should also work in 3D
            ip3D.Delta = FungeVector.BACKWARDS;
            ip3D.PushToStack(n);
            CoreInstructions.DecideVertical.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.UP));
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
            CoreInstructions.DecideVertical.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
        }

        [Test]
        public void DecideForwards_GoesBackwards_ForZeroArgument()
        {
            // Should go backwards if top of stack is zero
            ip3D.Delta = FungeVector.FORWARDS;
            ip3D.PushToStack(0);
            CoreInstructions.DecideForwards.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.BACKWARDS));
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-242)]
        [TestCase(-1)]
        public void DecideForwards_GoesForwards_ForNonZeroArgument(int n)
        {
            // Should go forwards if top of stack is non-zero
            ip3D.Delta = FungeVector.BACKWARDS;
            ip3D.PushToStack(n);
            CoreInstructions.DecideForwards.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(FungeVector.FORWARDS));
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
            CoreInstructions.DecideForwards.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(-d));

            d = ip2D.Delta;
            ip2D.PushToStack(n);
            CoreInstructions.DecideForwards.Execute(ip2D);
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
            CoreInstructions.Reflect.Execute(ip3D);
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
            CoreInstructions.Skip.Execute(ip3D);
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
            var p = ip3D.Position;
            CoreInstructions.JumpForward.Execute(ip3D);
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
            CoreInstructions.SetDelta.Execute(ip3D);
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
            CoreInstructions.SetDelta.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(x, y)));
        }
            
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(-242)]
        public void SetDelta_SetsDeltaCorrectly_1D(int x)
        {
            ip1D.PushToStack(x);
            CoreInstructions.SetDelta.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(new FungeVector(x)));
        }
            
        [Test]
        public void Iterate_RepeatsCorrectly_NextCell()
        {
            ip1D.Space.LoadString(new FungeVector(), "k3");
            ip1D.PushToStack(3);
            CoreInstructions.Iterate.Execute(ip1D);
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
            CoreInstructions.Iterate.Execute(ip1D);
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
            CoreInstructions.Iterate.Execute(ip1D);
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
            CoreInstructions.Iterate.Execute(ip1D);
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector(1)));
        }
            
        [Test]
        public void Iterate_SkipsInstruction_IfNegative()
        {
            ip1D.Space.LoadString(new FungeVector(), "k3");
            ip1D.PushToStack(-1);
            CoreInstructions.Iterate.Execute(ip1D);
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector(1)));
        }
            
        [Test]
        public void Iterate_SkipsInstruction_IfZero_Wrapping()
        {
            ip1D.Space.LoadString(new FungeVector(-1), "3k");
            ip1D.PushToStack(0);
            CoreInstructions.Iterate.Execute(ip1D);
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector(-1)));
        }
            
        [Test]
        public void Iterate_SkipsInstruciton_IfZero_WithSpacesAndComments()
        {
            ip1D.Space.LoadString(new FungeVector(), "k    ;;;3");
            ip1D.PushToStack(0);
            CoreInstructions.Iterate.Execute(ip1D);
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector(8)));
        }
            
        [Test]
        public void Iterate_IteratesCorrectly_WithSkipInstruction()
        {
            ip1D.Space.LoadString(new FungeVector(), "k#");
            ip1D.PushToStack(4);
            CoreInstructions.Iterate.Execute(ip1D);
            Assert.That(ip1D.Position, Is.EqualTo(new FungeVector(4)));
        }
            
        [Test]
        public void TurnRight_TurnsRight_CardinalDirection()
        {
            ip2D.Delta = FungeVector.UP;
            CoreInstructions.TurnRight.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.RIGHT));
        }
            
        [Test]
        public void TurnRight_TurnsRight_NonCardinalDirection()
        {
            ip2D.Delta = new FungeVector(3, -2);
            CoreInstructions.TurnRight.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(2, 3)));
        }
            
        [Test]
        public void TurnRight_TurnsRight_In3D()
        {
            ip3D.Delta = new FungeVector(3, -2, 5);
            CoreInstructions.TurnRight.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(new FungeVector(2, 3, 5)));
        }
            
        [Test]
        public void TurnRight_Reflects_In1D()
        {
            FungeVector d = ip1D.Delta;
            CoreInstructions.TurnRight.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
        }
            
        [Test]
        public void TurnLeft_TurnsLeft_CardinalDirection()
        {
            ip2D.Delta = FungeVector.UP;
            CoreInstructions.TurnLeft.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.LEFT));
        }
            
        [Test]
        public void TurnLeft_TurnsLeft_NonCardinalDirection()
        {
            ip2D.Delta = new FungeVector(3, -2);
            CoreInstructions.TurnLeft.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(-2, -3)));
        }
            
        [Test]
        public void TurnLeft_TurnsLeft_In3D()
        {
            ip3D.Delta = new FungeVector(3, -2, 5);
            CoreInstructions.TurnLeft.Execute(ip3D);
            Assert.That(ip3D.Delta, Is.EqualTo(new FungeVector(-2, -3, 5)));
        }
            
        [Test]
        public void TurnLeft_Reflects_In1D()
        {
            FungeVector d = ip1D.Delta;
            CoreInstructions.TurnLeft.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
        }
            
        [Test]
        public void Compare_TurnsRight_IfFirstArgumentGreater()
        {
            ip2D.PushToStack(5);
            ip2D.PushToStack(3);
            CoreInstructions.Compare.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.DOWN));
        }
            
        [Test]
        public void Compare_TurnsLeft_IfFirstArgumentLesser()
        {
            ip2D.PushToStack(3);
            ip2D.PushToStack(5);
            CoreInstructions.Compare.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.UP));
        }
            
        [Test]
        public void Compare_DoesNothing_IfEqual()
        {
            ip2D.PushToStack(5);
            ip2D.PushToStack(5);
            CoreInstructions.Compare.Execute(ip2D);
            Assert.That(ip2D.Delta, Is.EqualTo(FungeVector.RIGHT));
        }
    }
}