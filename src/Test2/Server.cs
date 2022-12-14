namespace Test2;

using System.Net;
using System.Net.Sockets;

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
    /// <returns>
    /// <see cref="Task"/>.
    /// </returns>
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
                    Task.Run(async () => await this.Read(socket).ConfigureAwait(false)));
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

    private async Task Read(Socket socket, )
    {
        var stream = new NetworkStream(socket);
        using var reader = new StreamReader(stream);

        try
        {
            while (true)
            {
                var message = await reader.ConfigureReadLineAsync();

                // to  console...
            }


            await transmissionTask;
        }
        finally
        {
            reader.Close();
            stream.Close();
            socket.Close();
        }
    }
    
    private string receiveMessage()
}