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
            StreamWriter writer = new StreamWriter(inputStream);
            writer.Write("42");
            writer.Flush();
            inputStream.Seek(0, SeekOrigin.Begin);
            CoreInstructions.Input.Execute(ip1D);
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt('4')));
            CoreInstructions.Input.Execute(ip1D);
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt('2')));
        }
            
        [Test]
        public void Input_Reflects_OnEndOfInput()
        {
            var d = ip1D.Delta;
            CoreInstructions.Input.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
        }
            
        [Test]
        public void InputInteger_PushesCorrectValue()
        {
            StreamWriter writer = new StreamWriter(inputStream);
            writer.Write("42");
            writer.Flush();
            inputStream.Seek(0, SeekOrigin.Begin);
            CoreInstructions.InputInteger.Execute(ip1D);
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(42)));
        }
            
        [Test]
        public void InputInteger_PushesZero_ForInvalidInput()
        {
            StreamWriter writer = new StreamWriter(inputStream);
            writer.Write("not a number");
            writer.Flush();
            inputStream.Seek(0, SeekOrigin.Begin);
            CoreInstructions.InputInteger.Execute(ip1D);
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt(0)));
        }
            
        [Test]
        public void InputInteger_DoesNotConsumeNonIntegerInput()
        {
            StreamWriter writer = new StreamWriter(inputStream);
            writer.Write("not a number");
            writer.Flush();
            inputStream.Seek(0, SeekOrigin.Begin);
            CoreInstructions.InputInteger.Execute(ip1D);
            CoreInstructions.Input.Execute(ip1D);
            Assert.That(ip1D.PopFromStack(), Is.EqualTo(new FungeInt('n')));
        }
            
        [Test]
        public void InputInteger_Reflects_OnEndOfInput()
        {
            var d = ip1D.Delta;
            CoreInstructions.InputInteger.Execute(ip1D);
            Assert.That(ip1D.Delta, Is.EqualTo(-d));
        }
            
        [Test]
        public void Output_WritesCorrectValue()
        {
            ip1D.PushToStack('4');
            CoreInstructions.Output.Execute(ip1D);
            Assert.That(output.ToString(), Is.EqualTo("4"));
            ip1D.PushToStack('2');
            CoreInstructions.Output.Execute(ip1D);
            Assert.That(output.ToString(), Is.EqualTo("42"));
        }
            
        [Test]
        public void OutputInteger_WritesCorrectValue()
        {
            ip1D.PushToStack(42);
            CoreInstructions.OutputInteger.Execute(ip1D);
            Assert.That(output.ToString(), Is.EqualTo("42 "));
        }
    }
}