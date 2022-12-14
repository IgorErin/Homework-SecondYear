namespace Test2;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Client class for <see cref="Server"/>.
/// </summary>
public sealed class Client : IDisposable, IWriter
{
    private readonly NetworkStream networkStream;
    private readonly TcpListener tcpListener;

    private readonly StreamReader streamReader;
    private readonly StreamWriter streamWriter;

    private readonly int port;
    private readonly IPAddress ipAddress;

    private readonly IPrinter printer;
    
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="port">Port for communicating with the server.</param>
    public Client(int port, IPAddress ipAddress, IPrinter printer)
    {
        this.port = port;
        this.ipAddress = ipAddress;
        this.printer = printer;
        
        var client = new TcpClient("localHost", port);
        this.tcpListener = new TcpListener(ipAddress, this.port);

        this.networkStream = client.GetStream();
        
        this.streamWriter = new StreamWriter(networkStream);
        this.streamReader = new StreamReader(networkStream);
    }

    public void Start()
    {
        this.tcpListener.Start();

        Task.Run(async () => this.ReadMessages());
    }

    private async Task ReadMessages()
    {
        while (true)
        {
            var message = await this.streamReader.ConfigureReadLineAsync();
            
            this.printer.Print(message);

            if (message == "exit")
            {
                this.Exit();
                break;
            }
        }
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

    /// <summary>
    /// <see cref="Dispose"/> method for release resources.
    /// </summary>
    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
    }

    private async Task Exit()
    {
        this.tcpListener.Stop();
        await this.networkStream.DisposeAsync();
    }
}