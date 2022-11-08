namespace FTP;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Server class for <see cref="Client"/>.
/// </summary>
public class Server
{
    private const int GetAnswerSizeBuffer = 8;
    private const int ListAnswerSizeBuffer = 4;
    private const int IncorrectRequestLengthAnswer = -1;

    private readonly int port;

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    /// <param name="port">Port for communicating with the server.</param>
    public Server(int port)
        => this.port = port;

    /// <summary>
    /// The method whose call starts listening on the specified port.
    /// </summary>
    /// <param name="token">Token to stop listening.</param>
    public async Task Start(CancellationToken token)
    {
        var listener = new TcpListener(IPAddress.Any, this.port);
        listener.Start();

        var taskList = new LinkedList<Task>();

        try
        {
            while (true)
            {
                var socket = await listener.AcceptSocketAsync(token).ConfigureAwait(false);

                if (token.IsCancellationRequested)
                {
                    break;
                }

                taskList.AddLast(
                    Task.Run(async () => await this.ReadAndExecuteRequests(socket).ConfigureAwait(false)));
            }
        }
        finally
        {
            foreach (var task in taskList)
            {
                await task.ConfigureAwait(false);
            }

            listener.Stop();
        }
    }

    private async Task ListAsync(string path, NetworkStream stream)
    {
        Console.WriteLine("in List");

        var currentDirectory = new DirectoryInfo(path);

        if (!currentDirectory.Exists)
        {
            await stream.WriteAsync(
                BitConverter.GetBytes(IncorrectRequestLengthAnswer), 0, ListAnswerSizeBuffer).ConfigureAwait(false);

            await stream.FlushAsync().ConfigureAwait(false);

            Console.WriteLine("Flush -1 in List"); // TODO() remove

            return;
        }

        var size = currentDirectory.GetFiles().Length + currentDirectory.GetDirectories().Length;

        await stream.WriteAsync(BitConverter.GetBytes(IncorrectRequestLengthAnswer), 0, size).ConfigureAwait(false);

        // foreach (var file in currentDirectory.EnumerateFiles())
        // {
        //     await stream.WriteAsync($"{file.Name}, {false} ").ConfigureAwait(false);
        // }
        //
        // foreach (var directory in currentDirectory.EnumerateDirectories())
        // {
        //     await stream.WriteAsync($"{directory.Name} {true} ").ConfigureAwait(false); //TODO()
        // }

        await stream.FlushAsync().ConfigureAwait(false);

        Console.WriteLine("Flush data in List");
    }

    private async Task GetAsync(string path, NetworkStream writer)
    {
        if (!File.Exists(path))
        {
            await writer.WriteAsync(
                BitConverter.GetBytes(IncorrectRequestLengthAnswer), 0, GetAnswerSizeBuffer).ConfigureAwait(false);

            await writer.FlushAsync().ConfigureAwait(false);

            return;
        }

        var file = new FileInfo(path);

        await writer.WriteAsync(
            BitConverter.GetBytes(file.Length), 0, GetAnswerSizeBuffer).ConfigureAwait(false);

        var fileStream = file.Open(FileMode.Open);

        await fileStream.CopyToAsync(writer).ConfigureAwait(false);

        fileStream.Close();

        await writer.FlushAsync().ConfigureAwait(false);
    }

    private async Task ReadAndExecuteRequests(Socket socket)
    {
        var stream = new NetworkStream(socket);

        using var reader = new StreamReader(stream);

        try
        {
            while (true)
            {
                var command = await reader.ReadLineAsync().ConfigureAwait(false);

                var dividedCommand = command?.Split() ?? Array.Empty<string>();

                if (dividedCommand.Length != 2)
                {
                    continue;
                }

                var (commandType, path) = (dividedCommand[0], dividedCommand[1]);

                var _ = commandType switch
                {
                    "1" => Task.Run(
                        async () => await this.ListAsync(path, stream).ConfigureAwait(false), CancellationToken.None),
                    "2" => Task.Run(
                        async () => await this.GetAsync(path, stream).ConfigureAwait(false), CancellationToken.None),
                    _ => Task.Run(async () => await stream.WriteAsync(BitConverter.GetBytes(-1), 0, ListAnswerSizeBuffer))
                };
            }
        }
        finally
        {
            reader.Close();
            stream.Close();
            socket.Close();
        }
    }
}
