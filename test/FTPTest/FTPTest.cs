namespace FTPTest;

using FTP;

/// <summary>
/// FTP test class.
/// </summary>
public class Tests
{
    private const int Port = 8888;
    private readonly Server server = new (Port);
    private readonly CancellationTokenSource sourceToken = new ();

    /// <summary>
    /// Set up method.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        Task.Run(async () => await this.server.Start(this.sourceToken.Token));
    }

    /// <summary>
    /// Test equality of responses of multiple queries <see cref="Client.ListAsync"/>.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task NumberListAnswersAreEqualTest()
    {
        const int clientCount = 10;
        var clients = new Client[clientCount];

        for (var i = 0; i < clientCount; i++)
        {
            clients[i] = new Client(Port);
        }

        var tasks = new Task<(int, List<(string, bool)>)>[clientCount];

        for (var i = 0; i < clientCount; i++)
        {
            tasks[i] = clients[i].ListAsync("./");
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
    /// <returns>Task.</returns>
    [Test]
    public async Task ExistingDirectorySizeTest()
    {
        var client = new Client(Port);

        var (size, _) = await client.ListAsync("./");

        Assert.That(size, Is.GreaterThan(0));
    }

    /// <summary>
    /// A test that checks the non-negativity of the size of an not existing directory.
    /// </summary>
    /// <returns>Task.</returns>
    [Test]
    public async Task NotExistingDirectorySizeTest()
    {
        var client = new Client(Port);

        var (size, _) = await client.ListAsync("@<notExistPath>");

        Assert.That(size, Is.EqualTo(-1));
    }
}