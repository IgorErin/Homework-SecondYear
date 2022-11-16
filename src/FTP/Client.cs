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

    private readonly TcpClient client;

    private readonly StreamWriter writer;
    private readonly StreamReader reader;

    private readonly NetworkStream stream;

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
        this.client = new TcpClient("localHost", port);

        this.stream = this.client.GetStream();
        this.writer = new StreamWriter(this.stream);
        this.reader = new StreamReader(this.stream);
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
    public async Task<(int, List<(string, bool)>)> List(string path)
    {
        await this.writer.WriteLineAsync($"1 {path}\n");
        await this.writer.FlushAsync();

        var size = await this.GetSizeFromListAsync();

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
            var name = await this.GetNameAsync();
            var isDirectory = await this.GetDirectoryFlagAsync();

            directoryDataList.Add((name, isDirectory));

            await this.ReadWhiteSpaceAsync();
        }

        return (size, directoryDataList);
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
        await this.writer.WriteLineAsync($"2 {pathToGet}");
        await this.writer.FlushAsync();

        var fileSize = await this.GetSizeFromGetAsync();

        var bytesLeft = fileSize;
        var currentChunkSize = Math.Min(ChunkSize, bytesLeft);
        var chunkBuffer = new byte[ChunkSize];

        while (bytesLeft > 0)
        {
            var writtenByteCount = await this.stream.ReadAsync(chunkBuffer, 0, (int)currentChunkSize);

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

    /// <summary>
    /// <see cref="Dispose"/> method for release resources.
    /// </summary>
    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.reader.Dispose();
        this.writer.Dispose();
        this.stream.Dispose();
        this.client.Dispose();

        this.disposed = true;
    }

    private async Task ReadWhiteSpaceAsync()
    {
        var charWhiteSpaceBuffer = new byte[StringCharSizeInBytes];
        await this.stream.ConfigureReadAsyncWithCheck(charWhiteSpaceBuffer);

        if (Encoding.Unicode.GetString(charWhiteSpaceBuffer) != " ")
        {
            throw new Exception();
        }
    }

    private async Task<string> GetNameAsync()
    {
        await this.stream.ConfigureReadAsyncWithCheck(this.charBuffer);

        this.charList.Clear();
        var symbol = Encoding.Unicode.GetString(this.charBuffer);

        while (symbol != " ")
        {
            this.charList.AddLast(symbol);

            await this.stream.ConfigureReadAsyncWithCheck(this.charBuffer);
            symbol = Encoding.Unicode.GetString(this.charBuffer);
        }

        return string.Join(string.Empty, this.charList);
    }

    private async Task<bool> GetDirectoryFlagAsync()
    {
        await this.stream.ConfigureReadAsyncWithCheck(this.boolBuffer);

        return BitConverter.ToBoolean(this.boolBuffer);
    }

    private async Task<int> GetSizeFromListAsync()
    {
        var sizeInBytes = new byte[ListSizeBufferSize];

        await this.stream.ConfigureReadAsyncWithCheck(sizeInBytes);

        return BitConverter.ToInt32(sizeInBytes);
    }

    private async Task<long> GetSizeFromGetAsync()
    {
        var sizeInBytes = new byte[GetSizeBufferSize];

        await this.stream.ConfigureReadAsyncWithCheck(sizeInBytes);

        return BitConverter.ToInt64(sizeInBytes);
    }
}
