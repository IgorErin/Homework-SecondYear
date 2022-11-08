namespace FTP;

using System.Net.Sockets;
using Exceptions;

/// <summary>
/// Client class for <see cref="Server"/>
/// </summary>
public class Client : IDisposable
{
    private const int SizeLengthInBytes = 8;

    private readonly TcpClient client;

    private readonly StreamWriter writer;
    private readonly StreamReader reader;

    private readonly NetworkStream stream;

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

        var size = await this.GetSizeAsync();

        var data = await this.reader.ReadLineAsync();
        var splitData = data?.Split(" ") ?? throw new ClientException("null response from the server");

        var resultList = new List<(string, bool)>();

        var index = 1;

        while (index + 1 < splitData.Length)
        {
            if (!bool.TryParse(splitData[index + 1], out var flag))
            {
                throw new ClientException("data transmission protocol violation, unreadable value");
            }

            resultList.Add((splitData[index], flag));

            index += 2;
        }

        return ((int)size, resultList);
    }

    /// <summary>
    /// A method that allows you to get the number of bytes and the bytes themselves
    /// in the form of an <see cref="Array"/> of the file you specified.
    /// </summary>
    /// <param name="path">Path to file.</param>
    /// <returns>Pair is the number of bytes and the bytes themselves.</returns>
    /// <exception cref="ClientException">
    /// An exception will be thrown if there is a zero response from the server or
    /// if the transmission protocol is violated.
    /// </exception>
    public async Task<(long, Byte[])> GetAsync(string path)
    {
        await this.writer.WriteLineAsync($"2 {path}");
        await this.writer.FlushAsync();

        var fileSize = await this.GetSizeAsync();

        var fileBuffer = new byte[fileSize];

        var byteCount = await this.stream.ReadAsync(fileBuffer, 0, (int)fileSize);

        if (byteCount != fileSize)
        {
            throw new ClientException("data loss during transmission, incomplete specifier");
        }

        return (fileSize, fileBuffer);
    }

    private async Task<long> GetSizeAsync()
    {
        var sizeInBytes = new byte[SizeLengthInBytes];

        var count = await this.stream.ReadAsync(sizeInBytes.AsMemory(0, SizeLengthInBytes));

        if (count != SizeLengthInBytes)
        {
            throw new ClientException("data loss during transmission, incomplete specifier");
        }

        return BitConverter.ToInt64(sizeInBytes);
    }

    /// <summary>
    /// Method for releasing unmanaged resources.
    /// </summary>
    public void Dispose() //TODO()
    {
        this.writer.Dispose();
        this.reader.Dispose();
        this.client.Dispose();
    }
}
