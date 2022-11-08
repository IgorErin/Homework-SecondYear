namespace FTP;

using System.Net;
using System.Net.Sockets;

public class Server
{
    private readonly int port;

    public Server(int port)
        => this.port = port;

    public async Task Start(CancellationToken token)
    {
        var listener = new TcpListener(IPAddress.Any, this.port);
        listener.Start();

        Console.WriteLine($"Listening on port {this.port}");

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

                taskList.AddLast(Task.Run(async () => await this.ReadAndExecuteRequests(socket, token), CancellationToken.None));
            }
        }
        finally
        {
            foreach (var task in taskList)
            {
                await task;
            }

            listener.Stop();
        }
    }

    private async Task List(string path, StreamWriter writer)
    {
        Console.WriteLine("in List");

        var currentDirectory = new DirectoryInfo(path);

        if (!currentDirectory.Exists)
        {
            await writer.WriteLineAsync("-1 ");
            await writer.FlushAsync();

            Console.WriteLine("Flush -1 in List");

            return;
        }

        var size = currentDirectory.GetFiles().Length + currentDirectory.GetDirectories().Length;

        await writer.WriteAsync($"{size} ");

        foreach (var file in currentDirectory.EnumerateFiles())
        {
            await writer.WriteAsync($"{file.Name}, {false} ");
        }

        foreach (var directory in currentDirectory.EnumerateDirectories())
        {
            await writer.WriteAsync($"{directory.Name} {true} ");
        }

        await writer.WriteLineAsync();
        await writer.FlushAsync();

        Console.WriteLine("Flush data in List");
    }

    private async Task Get(string path, StreamWriter writer)
    {
        var file = new FileInfo(path);

        if (!file.Exists)
        {
            await writer.WriteLineAsync("-1");
            await writer.FlushAsync();

            return;
        }

        await writer.WriteAsync($"{file.Length} ");

        await file.Create().CopyToAsync(writer.BaseStream);

        await writer.FlushAsync();
    }

    private async Task ReadAndExecuteRequests(Socket socket, CancellationToken token)
    {
        var stream = new NetworkStream(socket);

        var reader = new StreamReader(stream);
        var writer = new StreamWriter(stream);

        try
        {
            while (true)
            {
                var command = await reader.ReadLineAsync();
                Console.WriteLine($"read Line in server");

                var dividedCommand = command?.Split() ?? Array.Empty<string>(); // TODO() null

                if (dividedCommand.Length != 2)
                {
                    continue; // TODO()
                }

                var (commandType, path) = (dividedCommand[0], dividedCommand[1]);

                Console.WriteLine($"Command type = {commandType}, path = {path}");

                var _ = commandType switch
                {
                    "1" => Task.Run(async () => await this.List(path, writer), CancellationToken.None),
                    "2" => Task.Run(async () => await this.Get(path, writer), CancellationToken.None),
                    _ => throw new Exception() //TODO() message
                };
            }
        }
        finally
        {
            Console.WriteLine("socket closed");

            reader.Close();
            writer.Close();

            socket.Close();
        }
    }
}