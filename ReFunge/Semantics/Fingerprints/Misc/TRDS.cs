using System.Text;
using ReFunge.Data;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.Misc;

[Fingerprint("TRDS", FingerprintType.InstancedPerInterpreter)]
public class TRDS : InstancedFingerprint
{
    public TRDS(Interpreter interpreter) : base(interpreter)
    {
        Interpreter.Input = _reader = new TardisReader(Interpreter.Input);
        Interpreter.Output = _outputWriter = new TardisWriter(Interpreter.Output);
        Interpreter.Error = _errorWriter = new TardisWriter(Interpreter.Error);

        _initialSpace = Interpreter.PrimarySpace.Clone();
    }

    private TardisReader _reader;
    private TardisWriter _errorWriter;
    private TardisWriter _outputWriter;
    private FungeSpace _initialSpace;

    private Dictionary<long, FungeIP> _travelingIPs = new();

    public override void EachTick(long tickNo)
    {
        if (_travelingIPs.TryGetValue(tickNo, out var ip))
        {
            if (Interpreter.IPList.Contains(ip))
            {
                ip.Unfreeze();
            }
            else
            {
                Interpreter.AddNewIP(ip);
                
            }

            _travelingIPs.Remove(tickNo);
        }
    }

    private class TardisReader : TextReader
    {
        private TextReader _reader;
        private bool _rewind = false;
        private string _read;
        private int _idx;

        public TardisReader(TextReader reader)
        {
            _reader = reader;
        }

        public override int Peek()
        {
            if (_rewind)
            {
                return _read[_idx];
            }
            else
            {
                return _reader.Peek();
            }
        }

        public override int Read()
        {
            if (_rewind)
            {
                return _read[_idx++];
            }
            else
            {
                var c = _reader.Read();
                _read = _read + (char)c;
                return c;
            }
        }

        public void EnterStasis()
        {
            _idx = 0;
            _rewind = true;
        }

        public void ExitStasis()
        {
            _rewind = false;
        }
    }

    private class TardisWriter : TextWriter
    {
        private TextWriter _writer;
        private bool _write = true;

        public void EnterStasis()
        {
            _write = false;
        }

        public void ExitStasis()
        {
            _write = true;
        }

        public TardisWriter(TextWriter writer)
        {
            _writer = writer;
        }


        public override Encoding Encoding => _writer.Encoding;

        public override void Write(char value)
        {
            if (_write)
            {
                _writer.Write(value);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _writer.Dispose();
        }

        public override void Flush()
        {
            if (_write)
                _writer.Flush();
        }
    }
}