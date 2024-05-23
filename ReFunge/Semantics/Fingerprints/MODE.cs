namespace ReFunge.Semantics.Fingerprints;

public class MODE
{
    // MODE: Funge-98 standard mode control
    // Allows for changing several IP modes.
    [Instruction('H')]
    public static FungeFunc HoverMode = new FungeAction(ip =>
    {
        ip.HoverMode = !ip.HoverMode;
    });
    
    [Instruction('I')]
    public static FungeFunc InvertMode = new FungeAction(ip =>
    {
        ip.InvertMode = !ip.InvertMode;
    });
    
    [Instruction('Q')]
    public static FungeFunc QueueMode = new FungeAction(ip =>
    {
        ip.QueueMode = !ip.QueueMode;
    });
    
    [Instruction('S')]
    public static FungeFunc SwitchMode = new FungeAction(ip =>
    {
        ip.SwitchMode = !ip.SwitchMode;
    });
}