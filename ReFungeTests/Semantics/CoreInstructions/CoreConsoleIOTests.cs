using ReFunge.Data.Values;
using ReFunge.Semantics;

namespace ReFungeTests.Semantics;

internal partial class CoreInstructionsTests
{
    [TestFixture]
    internal class CoreConsoleIOTests : CoreInstructionsTests
    {
        [Test]
        public void Input_PushesCorrectValue()
        {
            StreamWriter writer = new StreamWriter(InputStream);
            writer.Write("42");
            writer.Flush();
            InputStream.Seek(0, SeekOrigin.Begin);
            ip1D.DoOp('~');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt('4')));
            ip1D.DoOp('~');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt('2')));
        }
            
        [Test]
        public void Input_Reflects_OnEndOfInput()
        {
            var d = ip1D.Delta;
            ip1D.DoOp('~');
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
        }
            
        [Test]
        public void InputInteger_PushesCorrectValue()
        {
            StreamWriter writer = new StreamWriter(InputStream);
            writer.Write("42");
            writer.Flush();
            InputStream.Seek(0, SeekOrigin.Begin);
            ip1D.DoOp('&');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(42)));
        }
            
        [Test]
        public void InputInteger_PushesZero_ForInvalidInput()
        {
            StreamWriter writer = new StreamWriter(InputStream);
            writer.Write("not a number");
            writer.Flush();
            InputStream.Seek(0, SeekOrigin.Begin);
            ip1D.DoOp('&');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
        }
            
        [Test]
        public void InputInteger_DoesNotConsumeNonIntegerInput()
        {
            StreamWriter writer = new StreamWriter(InputStream);
            writer.Write("not a number");
            writer.Flush();
            InputStream.Seek(0, SeekOrigin.Begin);
            ip1D.DoOp('&');
            ip1D.DoOp('~');
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt('n')));
        }
            
        [Test]
        public void InputInteger_Reflects_OnEndOfInput()
        {
            var d = ip1D.Delta;
            ip1D.DoOp('&');
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
        }
            
        [Test]
        public void Output_WritesCorrectValue()
        {
            ip1D.PushToStack('4');
            ip1D.DoOp(',');
            Assert.That(_output.ToString(), Is.EqualTo("4"));
            ip1D.PushToStack('2');
            ip1D.DoOp(',');
            Assert.That(_output.ToString(), Is.EqualTo("42"));
        }
            
        [Test]
        public void OutputInteger_WritesCorrectValue()
        {
            ip1D.PushToStack(42);
            ip1D.DoOp('.');
            Assert.That(_output.ToString(), Is.EqualTo("42 "));
        }
    }
}