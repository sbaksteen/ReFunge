namespace ReFunge.Semantics.Fingerprints.Core;

/// <summary>
///     MODE: Funge-98 standard mode control <br />
///     Allows for changing several IP modes. <br />
///     From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/MODE.markdown)
/// </summary>
[Fingerprint("MODE")]
public static class MODE
{
    /// <summary>
    ///     Toggles hover mode.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <seealso cref="IPModes.HoverMode" />
    [Instruction('H')]
    public static void ToggleHoverMode(FungeIP ip)
    {
        ip.ToggleModes(IPModes.HoverMode);
    }

    /// <summary>
    ///     Toggles invert mode.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <seealso cref="IPModes.InvertMode" />
    [Instruction('I')]
    public static void ToggleInvertMode(FungeIP ip)
    {
        ip.ToggleModes(IPModes.InvertMode);
    }

    /// <summary>
    ///     Toggles queue mode.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <seealso cref="IPModes.QueueMode" />
    [Instruction('Q')]
    public static void ToggleQueueMode(FungeIP ip)
    {
        ip.ToggleModes(IPModes.QueueMode);
    }

    /// <summary>
    ///     Toggles switch mode.
    /// </summary>
    /// <param name="ip">The IP executing the instruction.</param>
    /// <seealso cref="IPModes.SwitchMode" />
    [Instruction('S')]
    public static void ToggleSwitchMode(FungeIP ip)
    {
        ip.ToggleModes(IPModes.SwitchMode);
    }
}