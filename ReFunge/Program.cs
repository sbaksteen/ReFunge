
using ReFunge;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ReFungeTests")]

var funge = new Interpreter();
funge.Load(args[0]);
funge.Run();