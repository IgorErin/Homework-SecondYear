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

        var serverTask = Task.Run(async () => await server.Start(tokenSource.Token));

        var client1 = new Client(8888);

        // var result = await client1.List("D:\\Projects\\Homework-SecondYear\\src\\TestDir");
        //
        // Console.WriteLine($"result: size = {result.Item1}");
        // foreach (var item in result.Item2)
        // {
        //     Console.Write(item);
        // }

        var newFileInfo = new FileInfo("D:\\Projects\\Homework-SecondYear\\src\\TestDir\\testSubDir\\result.txt");

        var fileStream = newFileInfo.Open(FileMode.Open);

        await client1.GetAsync("D:\\Projects\\Homework-SecondYear\\src\\TestDir\\testFile.txt", fileStream);
    }
}
