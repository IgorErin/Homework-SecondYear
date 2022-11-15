namespace FTP;

using System.Net;
using System.Net.Sockets;
using System.Text;
using Extensions;

/// <summary>
/// Server class for <see cref="Client"/>.
/// </summary>
public class Server
{
    private const int GetAnswerBufferSize = 8;
    private const int ListAnswerBufferSize = 4;
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
        var currentDirectoryPath = Directory.GetCurrentDirectory(); // TODO() to server dir path !!!

        var currentDirectory = new DirectoryInfo(path); //TODO()

        if (!currentDirectory.Exists)
        {
            await stream.ConfigureWriteAsyncFromZero(BitConverter.GetBytes(IncorrectRequestLengthAnswer),  ListAnswerBufferSize);
            await stream.ConfigureFlushAsync();

            return;
        }

        var size = currentDirectory.GetFiles().Length + currentDirectory.GetDirectories().Length;
        await stream.ConfigureWriteAsyncFromZero(BitConverter.GetBytes(size), ListAnswerBufferSize);

        foreach (var file in currentDirectory.EnumerateFiles())
        {
            await stream.ConfigureWriteAsync(Encoding.Unicode.GetBytes(file.Name));
            await stream.ConfigureWriteWhiteSpaceAsync();

            await stream.ConfigureWriteFalseAsync();
            await stream.ConfigureWriteWhiteSpaceAsync();
        }

        foreach (var directory in currentDirectory.EnumerateDirectories())
        {
            await stream.ConfigureWriteAsync(Encoding.Unicode.GetBytes(directory.Name));
            await stream.ConfigureWriteWhiteSpaceAsync();

            await stream.ConfigureWriteTrueAsync();
            await stream.ConfigureWriteWhiteSpaceAsync();
        }

        await stream.ConfigureFlushAsync();
    }

    private async Task GetAsync(string path, NetworkStream writer)
    {
        if (!File.Exists(path))
        {
            await writer.ConfigureWriteAsyncFromZero(BitConverter.GetBytes(IncorrectRequestLengthAnswer), GetAnswerBufferSize);

            await writer.ConfigureFlushAsync();

            return;
        }

        var file = new FileInfo(path);

        await writer.ConfigureWriteAsyncFromZero(BitConverter.GetBytes(file.Length), GetAnswerBufferSize);

        var fileStream = file.Open(FileMode.Open);
        await fileStream.CopyToAsync(writer).ConfigureAwait(false);
        fileStream.Close();

        await writer.ConfigureFlushAsync();
    }

    private async Task ReadAndExecuteRequests(Socket socket)
    {
        var stream = new NetworkStream(socket);
        using var reader = new StreamReader(stream);

        try
        {
            while (true)
            {
                var command = await reader.ConfigureReadLineAsync();
                var dividedCommand = command?.Split() ?? Array.Empty<string>();

                if (dividedCommand.Length != 2)
                {
                    continue;
                }

                var (commandType, path) = (dividedCommand[0], dividedCommand[1]);

                var _ = commandType switch
                {
                    "1" => Task.Run(
                        async () => await this.ListAsync(path, stream).ConfigureAwait(false)),
                    "2" => Task.Run(
                        async () => await this.GetAsync(path, stream).ConfigureAwait(false)),
                    _ => Task.Run(async () =>
                        await stream.ConfigureWriteAsyncFromZero(BitConverter.GetBytes(-1), GetAnswerBufferSize))
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
