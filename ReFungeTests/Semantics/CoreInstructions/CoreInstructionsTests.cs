using ReFunge;
using ReFunge.Data;

namespace ReFungeTests.Semantics
{
    [TestFixture]
    internal partial class CoreInstructionsTests
    {
        FungeIP ip1D;
        FungeIP ip2D;
        FungeIP ip3D;

        private StringWriter output;
        protected MemoryStream inputStream;
        private StreamReader input;

        [SetUp]
        public void Setup()
        {
            output = new StringWriter();
            inputStream = new MemoryStream();
            input = new StreamReader(inputStream);
            Interpreter i = new Interpreter(input, output, dim: 2);
            ip2D = i.IPList[0];

            i = new Interpreter(input, output, dim: 1);
            ip1D = i.IPList[0];

            i = new Interpreter(input, output, dim: 3);
            ip3D = i.IPList[0];
        }
        
        [TearDown]
        public void TearDown()
        {
            output.Dispose();
            inputStream.Dispose();
            input.Dispose();
        }
    }
}
