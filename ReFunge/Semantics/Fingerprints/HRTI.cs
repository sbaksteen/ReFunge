using System.Diagnostics;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints;

public class HRTI
{
    // HRTI: High Resolution Timer Interface
    // Provides access to high resolution timers for measuring time intervals
    private static class Timers
    {
        private static readonly Dictionary<FungeIP, Stopwatch> _timers = new();
        
        public static void Start(FungeIP ip)
        {
            if (_timers.ContainsKey(ip))
            {
                Reset(ip);
            } else {
                _timers[ip] = Stopwatch.StartNew();
            }
        }
        
        public static FungeInt Look(FungeIP ip)
        {
            if (!_timers.TryGetValue(ip, out var timer))
                throw new FungeReflectException();
            return timer.Elapsed.Microseconds;
        }
        
        public static void Stop(FungeIP ip)
        {
            if (!_timers.TryGetValue(ip, out var timer)) 
                throw new FungeReflectException();
            timer.Stop();
            _timers.Remove(ip);
        }

        private static void Reset(FungeIP ip)
        {
            if (!_timers.TryGetValue(ip, out var timer)) 
                throw new FungeReflectException();
            timer.Reset();
        }

        public static int MicrosSinceLastSecond
        {
            get
            {
                var now = Stopwatch.GetTimestamp();
                var ticks = now % Stopwatch.Frequency;
                return (int)(ticks * 1000000 / Stopwatch.Frequency);
            }
        }
        
        public static int Frequency => int.Max(1,(int)(1000000/Stopwatch.Frequency));
    }
    
    [Instruction('G')]
    public static FungeFunc Granularity = new FungeFunc<FungeInt>(_ => Timers.Frequency);
    
    [Instruction('S')]
    public static FungeFunc Second = new FungeFunc<FungeInt>(_ => Timers.MicrosSinceLastSecond);
    
    [Instruction('M')]
    public static FungeFunc Mark = new FungeAction(Timers.Start);

    [Instruction('T')] 
    public static FungeFunc Time = new FungeFunc<FungeInt>(Timers.Look);
    
    [Instruction('E')]
    public static FungeFunc End = new FungeAction(Timers.Stop);
    
    
}