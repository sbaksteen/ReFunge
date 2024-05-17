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

        private StringWriter _output;
        protected MemoryStream InputStream;
        private StreamReader _input;
        private StringWriter _error;

        [SetUp]
        public void Setup()
        {
            _output = new StringWriter();
            InputStream = new MemoryStream();
            _input = new StreamReader(InputStream);
            _error = new StringWriter();
            var i = new Interpreter(2, _input, _output, _error);
            ip2D = i.IPList[0];

            i = new Interpreter(1, _input, _output, _error);
            ip1D = i.IPList[0];

            i = new Interpreter(3, _input, _output, _error);
            ip3D = i.IPList[0];
        }
        
        [TearDown]
        public void TearDown()
        {
            _output.Dispose();
            InputStream.Dispose();
            _input.Dispose();
            _error.Dispose();
        }
    }
}
