using System.Reflection;

if (File.Exists(args[0]))
{
    try
    {
        var assembly = Assembly.LoadFrom(args[0]);
        var result = MyNunit.MyNunit.RunAssemblyTests(assembly);

        Console.Write(result.ToString());
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine("Please, restart the program");
    }
}
else
{
    Console.WriteLine("The specified file was not found");
}
