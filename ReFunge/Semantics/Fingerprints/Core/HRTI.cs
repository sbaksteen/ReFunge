using System.Diagnostics;
using ReFunge.Data.Values;

namespace ReFunge.Semantics.Fingerprints.Core;

/// <summary>
///     HRTI: High Resolution Timer Interface <br />
///     Provides access to high resolution timers for measuring time intervals <br />
///     From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/HRTI.markdown)
/// </summary>
[Fingerprint("HRTI", FingerprintType.InstancedPerIP)]
public class HRTI : InstancedFingerprint
{
    private Stopwatch? _timer;

    /// <summary>
    ///     Create a new instance of HRTI.
    /// </summary>
    /// <param name="ip">The IP this instance is associated with</param>
    public HRTI(FungeIP ip) : base(ip)
    {
    }

    private static int MicrosPerTick => int.Max(1, (int)(1000000 / Stopwatch.Frequency));

    private static int MicrosSinceLastSecond => DateTime.Now.Microsecond + 1000 * DateTime.Now.Millisecond;

    /// <summary>
    ///     Get the granularity of the timer in microseconds.
    /// </summary>
    /// <param name="ip">The IP executing the instruction</param>
    /// <returns>The granularity of the timer in microseconds</returns>
    [Instruction('G')]
    public FungeInt Granularity(FungeIP ip)
    {
        return MicrosPerTick;
    }

    /// <summary>
    ///     Get the number of microseconds since the last second.
    /// </summary>
    /// <param name="ip">The IP executing the instruction</param>
    /// <returns>The number of microseconds since the last second</returns>
    [Instruction('S')]
    public FungeInt SinceLastSecond(FungeIP ip)
    {
        return MicrosSinceLastSecond;
    }

    /// <summary>
    ///     Mark the current time.
    /// </summary>
    /// <param name="ip">The IP executing the instruction</param>
    [Instruction('M')]
    public void Mark(FungeIP ip)
    {
        if (_timer is null)
            _timer = Stopwatch.StartNew();
        else
            _timer.Restart();
    }

    /// <summary>
    ///     Look at the time elapsed since the last mark.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <returns>The time elapsed since the last mark in microseconds.</returns>
    /// <exception cref="FungeReflectException">
    ///     Thrown when the timer has not been started, including when it has been stopped
    ///     by <see cref="Stop" />.
    /// </exception>
    [Instruction('T')]
    public FungeInt Look(FungeIP ip)
    {
        if (_timer is null) throw new FungeReflectException(new InvalidOperationException("Timer not started"));

        return (int)_timer.Elapsed.TotalMicroseconds;
    }

    /// <summary>
    ///     Stop the timer.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <exception cref="FungeReflectException">
    ///     Thrown when the timer has not been started, including when it has already been
    ///     stopped by <see cref="Stop" />.
    /// </exception>
    [Instruction('E')]
    public void Stop(FungeIP ip)
    {
        if (_timer is null) throw new FungeReflectException(new InvalidOperationException("Timer not started"));

        _timer = null;
    }
}