namespace FTPTest;

using FTP;

public class Tests
{
    private const int port = 8888;
    private readonly Server server = new (port);
    private readonly CancellationTokenSource sourceToken = new ();

    /// <summary>
    /// Set up method.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        Task.Run(async () => await this.server.Start(sourceToken.Token));
    }

    /// <summary>
    /// Test equality of responses of multiple queries <see cref="Client.List(string)"/>.
    /// </summary>
    [Test]
    public async Task NumberListAnswersAreEqualTest()
    {
        const int clientCount = 10;
        var clients = new Client[clientCount];

        for (var i = 0; i < clientCount; i++)
        {
            clients[i] = new Client(port);
        }

        var tasks = new Task<(int, List<(string, bool)>)>[clientCount];

        for (var i = 0; i < clientCount; i++)
        {
            tasks[i] = clients[i].List("./");
        }

        var sizeArray = new int[clientCount];
        var listArray = new List<(string, bool)>[clientCount];

        for (var i = 0; i < clientCount; i++)
        {
           (sizeArray[i], listArray[i]) = ((await tasks[i].ConfigureAwait(false)).Item1, (await tasks[i].ConfigureAwait(false)).Item2);
        }

        foreach (var item in sizeArray)
        {
            Assert.That(item, Is.EqualTo(sizeArray[0]));
        }

        foreach (var item in listArray)
        {
            Assert.That(item, Is.Not.Null);
            Assert.That(item, Is.EqualTo(listArray[0]));
        }
    }

    /// <summary>
    /// A test that checks the non-negativity of the size of an existing directory.
    /// </summary>
    [Test]
    public async Task ExistingDirectorySizeTest()
    {
        var client = new Client(port);

        var (size, _) = await client.List("./");

        Assert.That(size, Is.GreaterThan(0));
    }

    /// <summary>
    /// A test that checks the non-negativity of the size of an not existing directory.
    /// </summary>
    [Test]
    public async Task NotExistingDirectorySizeTest()
    {
        var client = new Client(port);

        var (size, _) = await client.List("@<notExistPath>");

        Assert.That(size, Is.EqualTo(-1));
    }
}