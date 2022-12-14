namespace Test2;

public class ConsolePrinter : IPrinter
{
    public void Print(string message)
    {
        Console.WriteLine(message);
    }
}