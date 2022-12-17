using System.Reflection;
using MyNunit.Printer;

var assemblies = new List<Assembly>();

foreach (var path in args)
{
    if (IsAssembly(path))
    {
        assemblies.Add(Assembly.LoadFrom(path));
    }
    else
    {
        Console.WriteLine("The specified file was not found");
    }
}

var myNunit = new MyNunit.MyNunit(assemblies);
var printer = new ConsoleTestPrinter();

myNunit.Print(printer);


bool IsAssembly(string path)
{
    bool isAssembly = false;

    try
    {
        AssemblyName a = AssemblyName.GetAssemblyName(path);

        isAssembly = true;
    } catch {}

    return isAssembly;
}