namespace FTP;

using System.Net.Sockets;
using System.Text;
using Exceptions;
using Extensions;

/// <summary>
/// Client class for <see cref="Server"/>.
/// </summary>
public sealed class Client : IDisposable
{
    private const int GetSizeBufferSize = 8;
    private const int ListSizeBufferSize = 4;
    private const int StringCharSizeInBytes = 2;
    private const int BoolSizeInBytes = 1;
    private const int ChunkSize = 1024;

    private readonly int port;

    private readonly byte[] charBuffer = new byte[StringCharSizeInBytes];
    private readonly byte[] boolBuffer = new byte[BoolSizeInBytes];
    private readonly byte[] fileChunkBuffer = new byte[ChunkSize];

    private readonly LinkedList<string> charList = new ();

    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class.
    /// </summary>
    /// <param name="port">Port for communicating with the server.</param>
    public Client(int port)
    {
        this.port = port;
    }

    /// <summary>
    /// List method allows you to get the number of files, folders and their names in the directory you specified.
    /// </summary>
    /// <param name="path">Path to the directory.</param>
    /// <returns>
    /// Pair is the number of elements in the directory,
    /// their enumeration with a flag equal to the True when the name belongs to the directory and a False
    /// when the name belongs to the file.
    /// </returns>
    /// <exception cref="ClientException">
    /// An exception will be thrown if there is a zero response from the server or
    /// if the transmission protocol is violated.
    /// </exception>
    public async Task<(int, List<(string, bool)>)> ListAsync(string path)
    {
        var client = new TcpClient("localHost", this.port);

        await using var networkStream = client.GetStream();
        await using var writer = new StreamWriter(networkStream);

        try
        {
            await writer.WriteLineAsync($"1 {path}\n");
            await writer.FlushAsync();

            var size = await this.GetSizeFromListAsync(networkStream);

            if (size == -1)
            {
                return (-1, new List<(string, bool)>());
            }

            if (size < 0)
            {
                throw new ClientException();
            }

            var directoryDataList = new List<(string, bool)>(size);

            for (var i = 0; i < size; i++)
            {
                var name = await this.GetNameAsync(networkStream);
                var isDirectory = await this.GetDirectoryFlagAsync(networkStream);

                directoryDataList.Add((name, isDirectory));

                await this.ReadWhiteSpaceAsync(networkStream);
            }

            return (size, directoryDataList);
        }
        finally
        {
            await networkStream.DisposeAsync();
            await writer.DisposeAsync();

            client.Dispose();
        }
    }

    /// <summary>
    /// A method that allows you to get the number of bytes and the bytes themselves
    /// in the form of an <see cref="Array"/> of the file you specified.
    /// </summary>
    /// <param name="pathToGet">Path relative to the server.</param>
    /// <param name="streamToWrite">Stream for writing data from the server.</param>
    /// <returns>Pair is the number of bytes and the bytes themselves.</returns>
    /// <exception cref="ClientException">
    /// An exception will be thrown if there is a zero response from the server or
    /// if the transmission protocol is violated.
    /// </exception>
    public async Task<long> GetAsync(string pathToGet, Stream streamToWrite)
    {
        var client = new TcpClient("localHost", this.port);

        await using var networkStream = client.GetStream();
        await using var writer = new StreamWriter(networkStream);

        try
        {
            await writer.WriteLineAsync($"2 {pathToGet}");
            await writer.FlushAsync();

            var fileSize = await this.GetSizeFromGetAsync(networkStream);

            var bytesLeft = fileSize;
            var currentChunkSize = Math.Min(ChunkSize, bytesLeft);
            var chunkBuffer = new byte[ChunkSize];

            while (bytesLeft > 0)
            {
                var writtenByteCount = await networkStream.ReadAsync(chunkBuffer, 0, (int)currentChunkSize);

                if (writtenByteCount != currentChunkSize)
                {
                    throw new ClientException("data loss during transmission, incomplete specifier");
                }

                await streamToWrite.WriteAsync(chunkBuffer, 0, (int)currentChunkSize);

                bytesLeft -= currentChunkSize;
                currentChunkSize = Math.Min(ChunkSize, bytesLeft);
            }

            await streamToWrite.FlushAsync();
            streamToWrite.Close();

            return fileSize;
        }
        finally
        {
            await networkStream.DisposeAsync();
            await writer.DisposeAsync();

            client.Dispose();
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

    private async Task ReadWhiteSpaceAsync(NetworkStream stream)
    {
        var charWhiteSpaceBuffer = new byte[StringCharSizeInBytes];
        await stream.ConfigureReadAsyncWithCheck(charWhiteSpaceBuffer);

        if (Encoding.Unicode.GetString(charWhiteSpaceBuffer) != " ")
        {
            throw new Exception();
        }
    }

    private async Task<string> GetNameAsync(NetworkStream stream)
    {
        await stream.ConfigureReadAsyncWithCheck(this.charBuffer);

        this.charList.Clear();
        var symbol = Encoding.Unicode.GetString(this.charBuffer);

        while (symbol != " ")
        {
            this.charList.AddLast(symbol);

            await stream.ConfigureReadAsyncWithCheck(this.charBuffer);
            symbol = Encoding.Unicode.GetString(this.charBuffer);
        }

        return string.Join(string.Empty, this.charList);
    }

    private async Task<bool> GetDirectoryFlagAsync(NetworkStream stream)
    {
        await stream.ConfigureReadAsyncWithCheck(this.boolBuffer);

        return BitConverter.ToBoolean(this.boolBuffer);
    }

    private async Task<int> GetSizeFromListAsync(NetworkStream stream)
    {
        var sizeInBytes = new byte[ListSizeBufferSize];

        await stream.ConfigureReadAsyncWithCheck(sizeInBytes);

        return BitConverter.ToInt32(sizeInBytes);
    }

    private async Task<long> GetSizeFromGetAsync(NetworkStream stream)
    {
        var sizeInBytes = new byte[GetSizeBufferSize];

        await stream.ConfigureReadAsyncWithCheck(sizeInBytes);

        return BitConverter.ToInt64(sizeInBytes);
    }
}
