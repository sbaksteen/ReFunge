using ReFunge.Data.Values;
using ReFunge.Semantics;

namespace ReFungeTests.Semantics;

internal partial class CoreInstructionsTests
{
    public class CoreSpaceManipulationTests : CoreInstructionsTests
    {
        [Test]
        public void FetchCharacter_GetsCorrectCharacter()
        {
            ip2D.Space.LoadString(new FungeVector(), "'f");
            CoreInstructions.FetchCharacter.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt('f')));
        }
        
        [Test]
        public void FetchCharacter_GetsSpaceCharacter()
        {
            ip2D.Space.LoadString(new FungeVector(), "' ");
            CoreInstructions.FetchCharacter.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(' ')));
        }

        [Test]
        public void FetchCharacter_JumpsForward()
        {
            ip2D.Space.LoadString(new FungeVector(), "'f");
            CoreInstructions.FetchCharacter.Execute(ip2D);
            Assert.That(ip2D.Position, Is.EqualTo(new FungeVector(1, 0)));
        }
        
        [Test]
        public void StoreCharacter_StoresCharacterCorrectly()
        {
            ip2D.PushToStack(new FungeInt('f'));
            CoreInstructions.StoreCharacter.Execute(ip2D);
            Assert.That(ip2D.Space[new FungeVector(1, 0)], Is.EqualTo(new FungeInt('f')));
        }
        
        [Test]
        public void Put_PutsCharacterCorrectly()
        {
            ip2D.PushToStack(new FungeInt('f'));
            ip2D.PushToStack(5);
            ip2D.PushToStack(3);
            CoreInstructions.Put.Execute(ip2D);
            Assert.That(ip2D.Space[new FungeVector(5, 3)], Is.EqualTo(new FungeInt('f')));
        }
        
        [Test]
        public void Get_GetsCorrectCharacter()
        {
            ip2D.Space.LoadString(new FungeVector(), "testing");
            ip2D.PushToStack(5);
            ip2D.PushToStack(0);
            CoreInstructions.Get.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt('n')));
        }
    }
}