namespace Test2;

public class ConsoleReader
{
    private readonly IWriter writer;

    public ConsoleReader(IWriter writer)
    {
        this.writer = writer;
    }

    public void Start()
    {
        var messagge = Console.ReadLine();
        this.writer.Write(messagge);

        while (messagge != "exit")
        {
            messagge = Console.ReadLine();
            this.writer.Write(messagge);
        }
    }
}