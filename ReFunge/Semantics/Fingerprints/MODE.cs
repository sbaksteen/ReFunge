namespace ReFunge.Semantics.Fingerprints;

[Fingerprint("MODE")]
public static class MODE
{
    // MODE: Funge-98 standard mode control
    // Allows for changing several IP modes.
    [Instruction('H')]
    public static void ToggleHoverMode(FungeIP ip)
    {
        ip.HoverMode = !ip.HoverMode;
    }

    [Instruction('I')]
    public static void ToggleInvertMode(FungeIP ip)
    {
        ip.InvertMode = !ip.InvertMode;
    }

    [Instruction('Q')]
    public static void ToggleQueueMode(FungeIP ip)
    {
        ip.QueueMode = !ip.QueueMode;
    }

    [Instruction('S')]
    public static void ToggleSwitchMode(FungeIP ip)
    {
        ip.SwitchMode = !ip.SwitchMode;
    }
}