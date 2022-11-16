namespace FTP;

/// <summary>
/// Main class.
/// </summary>
public static class FTPMain
{
    /// <summary>
    /// Main method.
    /// </summary>
    /// <returns>Main task.</returns>
    public static async Task Main()
    {
        var server = new Server(8888);

        var tokenSource = new CancellationTokenSource();

        var _ = Task.Run(async () => await server.Start(tokenSource.Token));

        var client1 = new Client(8888);

        var result = await client1.List("./");

        Console.WriteLine($"result size = {result.Item1}");
        foreach (var item in result.Item2)
        {
            Console.Write(item);
        }
    }
}
