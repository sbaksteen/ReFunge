using System.Runtime.CompilerServices;
using CommandLine;
using CommandLine.Text;

[assembly: InternalsVisibleTo("ReFungeTests")]

namespace ReFunge;

internal class Program
{
    public static void Main(string[] args)
    {
        var parser = new Parser(settings =>
        {
            settings.HelpWriter = null;
            settings.AutoVersion = false;
        });
        var parserResult = parser.ParseArguments<RunOptions>(args);
        parserResult
            .WithParsed(Run)
            .WithNotParsed(errs => DisplayHelp(parserResult, errs));
    }

    private static void DisplayHelp<T>(ParserResult<T> parserResult, IEnumerable<Error> errs)
    {
        var helpText = HelpText.AutoBuild(parserResult, h =>
        {
            h.Heading = $"ReFunge v{typeof(Program).Assembly.GetName().Version!.ToString(3)}";
            h.Copyright = "";
            return h;
        });
        Console.Error.WriteLine(helpText);
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
        var returnValue = interpreter.Run();
        if (opts.ShowTime)
        {
            var end = DateTime.Now;
            Console.Out.WriteLine($"Time taken: {interpreter.Tick} ticks; {end - now}.");
        }

        Environment.Exit(returnValue);
    }

    [Verb("run", true, HelpText = "Run a Funge-98 program.")]
    public class RunOptions
    {
        [Value(0, Required = true, HelpText = "The file to run.", MetaName = "input")]
        public string InputFile { get; set; }

        [Option('d', "dim", HelpText = "Number of dimensions to run the program in.", Default = 2)]
        public int Dimensions { get; set; }

        [Option('t', "time", HelpText = "Show the time taken to run the program.", Default = false)]
        public bool ShowTime { get; set; }
    }
}