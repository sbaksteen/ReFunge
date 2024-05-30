namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("MODE")]
public static class MODE
{
    // MODE: Funge-98 standard mode control
    // Allows for changing several IP modes.
    // From the Funge-98 specification (https://github.com/catseye/Funge-98/blob/master/library/MODE.markdown)
    
    [Instruction('H')]
    public static void ToggleHoverMode(FungeIP ip)
    {
        ip.ToggleModes(IPModes.HoverMode);
    }

    [Instruction('I')]
    public static void ToggleInvertMode(FungeIP ip)
    {
        ip.ToggleModes(IPModes.InvertMode);
    }

    [Instruction('Q')]
    public static void ToggleQueueMode(FungeIP ip)
    {
        ip.ToggleModes(IPModes.QueueMode);
    }

    [Instruction('S')]
    public static void ToggleSwitchMode(FungeIP ip)
    {
        ip.ToggleModes(IPModes.SwitchMode);
    }
}