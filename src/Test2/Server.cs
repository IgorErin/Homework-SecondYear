namespace Test2;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Server class for <see cref="Client"/>.
/// </summary>
public class Server : IWriter
{
    private readonly NetworkStream networkStream;
    private readonly TcpListener tcpListener;

    private readonly StreamReader streamReader;
    private readonly StreamWriter streamWriter;

    private readonly int port;
    private readonly IPrinter printer;
    private readonly IPAddress ipAddress;

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class.
    /// </summary>
    public Server(int port, IPrinter printer)
    {
        this.port = port;
        this.printer = printer;
        
        var client = new TcpClient("localHost", port);
        this.tcpListener = new TcpListener(IPAddress.Any, this.port);

        this.networkStream = client.GetStream();
        
        this.streamWriter = new StreamWriter(networkStream);
        this.streamReader = new StreamReader(networkStream);
    }
    
    public async Task Start()
    {
        this.tcpListener.Start();

        Task.Run(async () => await this.Read(printer));
    }

    public async Task Write(string message)
    {
        await this.streamWriter.WriteLineAsync(message);
        await this.streamWriter.FlushAsync();
        
        if (message == "exit")
        {
            await this.Exit();
        }
    }

    private async Task Read(IPrinter printer)
    {
        try
        {
            while (true)
            {
                var message = await this.streamReader.ConfigureReadLineAsync();

                printer.Print(message);
            }
        }
        finally
        {
            this.streamReader.Close();
        }
    }

    private async Task Exit()
    {
        this.tcpListener.Stop();
        await this.networkStream.DisposeAsync();
    }
}