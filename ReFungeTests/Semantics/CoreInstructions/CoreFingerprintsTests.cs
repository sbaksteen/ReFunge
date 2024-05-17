using ReFunge.Data.Values;
using ReFunge.Semantics;
using ReFunge.Semantics.Fingerprints;

namespace ReFungeTests.Semantics;

internal partial class CoreInstructionsTests
{
    internal class CoreFingerprintsTests : CoreInstructionsTests
    {
        [Test]
        public void LoadFingerprint_LoadsNULL()
        {
            ip2D.PushToStack(new FungeString("NULL").Handprint);
            ip2D.PushToStack(1);
            CoreInstructions.LoadFingerprint.Execute(ip2D);
            foreach (var stack in ip2D.FingerprintStacks)
            {
                Assert.That(stack, Has.Count.EqualTo(1));
                Assert.That(stack.Peek(), Is.EqualTo(NULL.Reflect));
            }
        }

        [Test]
        public void LoadFingerprint_Reflects_IfFingerprintNotRecognized()
        {
            ip2D.PushToStack(0);
            ip2D.PushToStack(1);
            CoreInstructions.LoadFingerprint.Execute(ip2D);
            // Fingerprint 0 doesn't exist...
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(-1, 0)));
            foreach (var stack in ip2D.FingerprintStacks)
            {
                Assert.That(stack, Is.Empty);
            }
        }

        [Test]
        public void LoadFingerprint_OnlyLoadsExistingFunctions()
        {
            ip2D.PushToStack(new FungeString("ROMA").Handprint);
            ip2D.PushToStack(1);
            CoreInstructions.LoadFingerprint.Execute(ip2D);
            Assert.That(ip2D.FingerprintStacks['I' - 'A'], Has.Count.EqualTo(1));
            Assert.That(ip2D.FingerprintStacks['V' - 'A'], Has.Count.EqualTo(1));
            Assert.That(ip2D.FingerprintStacks['X' - 'A'], Has.Count.EqualTo(1));
            Assert.That(ip2D.FingerprintStacks['L' - 'A'], Has.Count.EqualTo(1));
            Assert.That(ip2D.FingerprintStacks['C' - 'A'], Has.Count.EqualTo(1));
            Assert.That(ip2D.FingerprintStacks['D' - 'A'], Has.Count.EqualTo(1));
            Assert.That(ip2D.FingerprintStacks['M' - 'A'], Has.Count.EqualTo(1));
            Assert.That(ip2D.FingerprintStacks['Z' - 'A'], Is.Empty);
        }

        [Test]
        public void LoadFingerprint_PushesHandprint()
        {
            ip2D.PushToStack(new FungeString("ROMA").Handprint);
            ip2D.PushToStack(1);
            CoreInstructions.LoadFingerprint.Execute(ip2D);
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(1)));
            Assert.That(ip2D.PopFromStack(), Is.EqualTo(new FungeInt(new FungeString("ROMA").Handprint)));
        }

        [Test]
        public void UnloadFingerprint_UnloadsNULL()
        {
            ip2D.LoadFingerprint(new FungeString("NULL").Handprint);
            ip2D.PushToStack(new FungeString("NULL").Handprint);
            ip2D.PushToStack(1);
            CoreInstructions.UnloadFingerprint.Execute(ip2D);
            foreach (var stack in ip2D.FingerprintStacks)
            {
                Assert.That(stack, Is.Empty);
            }
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(1, 0)));
        }
        
        [Test]
        public void UnloadFingerprint_Reflects_IfFingerprintNotRecognized()
        {
            ip2D.LoadFingerprint(new FungeString("NULL").Handprint);
            ip2D.PushToStack(0);
            ip2D.PushToStack(1);
            CoreInstructions.UnloadFingerprint.Execute(ip2D);
            // Fingerprint 0 doesn't exist...
            Assert.That(ip2D.Delta, Is.EqualTo(new FungeVector(-1, 0)));
            foreach (var stack in ip2D.FingerprintStacks)
            {
                Assert.That(stack, Has.Count.EqualTo(1));
            }
        }
        
        [Test]
        public void UnloadFingerprint_OnlyUnloadsExistingFunctions()
        {
            ip2D.LoadFingerprint(new FungeString("NULL").Handprint);
            ip2D.PushToStack(new FungeString("ROMA").Handprint);
            ip2D.PushToStack(1);
            CoreInstructions.UnloadFingerprint.Execute(ip2D);
            Assert.That(ip2D.FingerprintStacks['I' - 'A'], Is.Empty);
            Assert.That(ip2D.FingerprintStacks['V' - 'A'], Is.Empty);
            Assert.That(ip2D.FingerprintStacks['X' - 'A'], Is.Empty);
            Assert.That(ip2D.FingerprintStacks['L' - 'A'], Is.Empty);
            Assert.That(ip2D.FingerprintStacks['C' - 'A'], Is.Empty);
            Assert.That(ip2D.FingerprintStacks['D' - 'A'], Is.Empty);
            Assert.That(ip2D.FingerprintStacks['M' - 'A'], Is.Empty);
            Assert.That(ip2D.FingerprintStacks['Z' - 'A'], Has.Count.EqualTo(1));
        }
    }
}