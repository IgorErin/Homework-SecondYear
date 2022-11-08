namespace FTP;

/// <summary>
/// Main class.
/// </summary>
public static class FTPMain
{
    /// <summary>
    /// Main method.
    /// </summary>
    public static async Task Main()
    {
        var server = new Server(8888);

        var tokenSource = new CancellationTokenSource();

        Task.Run(async () => await server.Start(tokenSource.Token));

        var client1 = new Client(8888);

        var result = await client1.List("D:\\Projects");

        Console.WriteLine("result: ");
        foreach (var item in result.Item2)
        {
            Console.Write(item);
        }
    }
}
