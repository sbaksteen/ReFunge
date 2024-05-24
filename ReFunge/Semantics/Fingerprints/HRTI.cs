using System.Diagnostics;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("HRTI", FingerprintType.InstancedPerIP)]
public class HRTI : InstancedFingerprint
{
    // HRTI: High Resolution Timer Interface
    // Provides access to high resolution timers for measuring time intervals
    // From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/HRTI.markdown)
    
    private Stopwatch? _timer;
    
    private static int MicrosPerTick => int.Max(1,(int)(1000000/Stopwatch.Frequency));

    private static int MicrosSinceLastSecond => DateTime.Now.Microsecond + 1000 * DateTime.Now.Millisecond;

    public HRTI(FungeIP ip) : base(ip)
    {
        
    }

    [Instruction('G')]
    public FungeInt Granularity(FungeIP ip) => MicrosPerTick;
    
    [Instruction('S')]
    public FungeInt SinceLastSecond(FungeIP ip) => MicrosSinceLastSecond;
    
    [Instruction('M')]
    public void Mark(FungeIP ip)
    {
        if (_timer is null)
        {
            _timer = Stopwatch.StartNew();
        }
        else
        {
            _timer.Restart();
        }
    }

    [Instruction('T')]
    public FungeInt Look(FungeIP ip)
    {
        if (_timer is null)
        {
            throw new FungeReflectException(new InvalidOperationException("Timer not started"));
        }

        return (int)_timer.Elapsed.TotalMicroseconds;
    }

    [Instruction('E')]
    public void Stop(FungeIP ip)
    {
        if (_timer is null)
        {
            throw new FungeReflectException(new InvalidOperationException("Timer not started"));
        }

        _timer = null;
    }
    
    
}