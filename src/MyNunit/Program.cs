if (File.Exists(args[0]))
{
    try
    {
        var nunit = new MyNunit.MyNunit();

        var result = nunit.RunTestsFrom(args[0]);

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
