namespace FTP;

using System.Net.Sockets;
using System.Text;
using Exceptions;
using Extensions;

/// <summary>
/// Client class for <see cref="Server"/>
/// </summary>
public sealed class Client : IDisposable
{
    private const int SizeLengthInBytes = 8;
    private const int StringCharSizeInBytes = 2;
    private const int BoolSizeInBytes = 1;

    private readonly TcpClient client;

    private readonly StreamWriter writer;
    private readonly StreamReader reader;

    private readonly NetworkStream stream;

    private readonly byte[] charBuffer = new byte[StringCharSizeInBytes];
    private readonly byte[] boolBuffer = new byte[BoolSizeInBytes];

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

        var size = await this.GetSizeAsync();

        if (size == -1)
        {
            return (-1, new List<(string, bool)>());
        }

        if (size < 0)
        {
            throw new ClientException();
        }

        var directoryDataList = new List<(string, bool)>((int)size);

        for (var i = 0; i < size; i++)
        {
            var name = await this.GetNameAsync();
            var isDirectory = await this.GetDirectoryFlagAsync();

            directoryDataList[i] = (name, isDirectory);

            await this.ReadWhiteSpaceAsync();
        }

        return ((int)size, directoryDataList);
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

    public void Dispose()
    {
        this.reader.Dispose();
        this.writer.Dispose();
        this.stream.Dispose();
    }

    private async Task ReadWhiteSpaceAsync()
    {
        var charWhiteSpaceBuffer = new byte[StringCharSizeInBytes];
        await this.stream.ConfigureReadAsyncWhithCheck(charWhiteSpaceBuffer);

        if (Encoding.Unicode.GetString(charWhiteSpaceBuffer) != " ")
        {
            throw new Exception();
        }
    }

    private async Task<string> GetNameAsync()
    {
        await this.stream.ConfigureReadAsyncWhithCheck(charBuffer);

        var charList = new LinkedList<string>();
        var symbol = Encoding.Unicode.GetString(this.charBuffer);

        while (symbol != " ")
        {
            charList.AddLast(symbol);

            await this.stream.ConfigureReadAsyncWhithCheck(this.charBuffer);
            symbol = Encoding.Unicode.GetString(this.charBuffer);
        }

        return string.Join(string.Empty, charList);
    }

    private async Task<bool> GetDirectoryFlagAsync()
    {
        await this.stream.ConfigureReadAsyncWhithCheck(this.boolBuffer);

        return System.Convert.ToBoolean(this.boolBuffer);
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
}
