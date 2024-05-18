using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using ReFunge;
using ReFunge.Data.Values;

namespace ReFungeTests;

[TestFixture]
[ExcludeFromCodeCoverage, Category("Diagnostics")]
public class MycologyTestSuite
{
    
    [OneTimeSetUp]
    public void Setup()
    {
        var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mycology/");
        Assert.That(Directory.Exists(directory));
        Directory.SetCurrentDirectory(directory);
        Assert.That(File.Exists("mycology.b98"));
        Assert.That(File.Exists("mycoterm.b98"));
        Assert.That(File.Exists("mycotrds.b98"));
        Assert.That(File.Exists("mycouser.b98"));
        Assert.That(File.Exists("mycorand.bf"));
        Assert.That(File.Exists("sanity.bf"));

    }

    [Test]
    public void Interpreter_PassesMycologySanity()
    {
        var bfOutput = new StringWriter();
        var bfInput = new StringReader("");
        var bfError = new StringWriter();
        
        Interpreter interpreter = new(2, bfInput, bfOutput, bfError);
        interpreter.Load("sanity.bf");
        interpreter.Run();

        string output = bfOutput.ToString().TrimEnd();
        
        Assert.That(output, Is.EqualTo("0 1 2 3 4 5 6 7 8 9"));

    }
    
    [Test]
    public void Interpreter_PassesMycologyCore()
    {
        var bfOutput = new StringWriter();
        var bfInput = new StringReader("");
        var bfError = new StringWriter();
        
        Interpreter interpreter = new(2, bfInput, bfOutput, bfError);
        interpreter.Load("mycology.b98");
        while (interpreter is { Tick: < 10000000, Quit: false, IPList.Count: > 0 })
        {
            interpreter.DoStep();
        }
        if (interpreter.Tick >= 10000000)
        {
            Console.Out.WriteLine(bfOutput.ToString());
            Assert.Fail("Interpreter timed out");
        }
        var output = bfOutput.ToString().Split("\n");
        var bads = 0;
        foreach (var line in output)
        {
            if (line.Contains("UNDEF:"))
            {
                Console.Out.WriteLine(line);
            }
        }
        foreach (var line in output)
        {
            if (line.Contains("BAD:"))
            {
                bads++;
                Console.Out.WriteLine(line);
            }
        }
        if (bads > 0)
        {
            Assert.Fail($"{bads} BAD lines");
        }

        Assert.Pass();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        for (int i = 0; i < 9; i++)
        {
            File.Delete($"mycotmp{i}.tmp");
        }
    }
}