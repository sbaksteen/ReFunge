using CommandLine;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ReFungeTests")]

namespace ReFunge;

internal class Program
{
    [Verb("run", isDefault: true, HelpText = "Run a Funge-98 program.")]
    public class RunOptions
    {
        [Option('i', "input", HelpText = "Input file to read from.", Required = true)]
        public string InputFile { get; set; }
        
        [Option('d', "dim", HelpText = "Number of dimensions to run the program in.", Default = 2)]
        public int Dimensions { get; set; }
        
        [Option('t', "time", HelpText = "Show the time taken to run the program.", Default = false)]
        public bool ShowTime { get; set; }
    }
    public class InfoOptions
    {
        
    }
    
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<RunOptions, InfoOptions>(args)
            .WithParsed<RunOptions>(Run);
    }

    private static void Run(RunOptions opts)
    {
        var file = new FileInfo(opts.InputFile);
        if (!file.Exists)
        {
            Console.Error.WriteLine($"File {opts.InputFile} does not exist.");
            return;
        }
        
        var now = DateTime.Now;

        var interpreter = new Interpreter(opts.Dimensions);
        interpreter.Load(file.FullName);
        interpreter.Run();
        if (opts.ShowTime)
        {
            var end = DateTime.Now;
            Console.Out.WriteLine($"Time taken: {interpreter.Tick} ticks; {end - now}.");
        }
    }
}