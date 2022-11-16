namespace FTPTest;

using FTP;

public class Tests
{
    private Server server = new Server(8888);
    private CancellationTokenSource sourceToken = new CancellationTokenSource();

    [SetUp]
    public void Setup()
    {
        Task.Run(async () => await this.server.Start(sourceToken.Token));
    }

    [Test]
    public async Task Test1()
    {
        var client = new Client(8888);

        var result = await client.List("./");

        foreach (var item in result.Item2)
        {
            Console.WriteLine(item);
        }
    }
}