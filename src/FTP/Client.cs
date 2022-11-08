namespace FTP;

using System.Net.Sockets;

public class Client : IDisposable
{
    private readonly int port;

    private readonly TcpClient client;

    private readonly StreamWriter writer;
    private readonly StreamReader reader;

    private readonly NetworkStream stream;

    public Client(int port)
    {
        this.port = port;

        this.client = new TcpClient("localHost", port);

        this.stream = this.client.GetStream();
        this.writer = new StreamWriter(this.stream);
        this.reader = new StreamReader(this.stream);
    }

    public async Task<(int, List<(string, bool)>)> List(string path)
    {
        await this.writer.WriteLineAsync($"1 {path}\n");
        await this.writer.FlushAsync();

        var data = await this.reader.ReadLineAsync();
        var splitData = data.Split();

        Console.WriteLine($"should be string: {data}");

        if (!int.TryParse(splitData[0], out var count))
        {
            throw new Exception();
        }

        var resultList = new List<(string, bool)>();

        var index = 1;

        while (index + 1 < splitData.Length)
        {
            if (!bool.TryParse(splitData[index + 1], out var flag))
            {
                throw new Exception(); //TODO()
            }

            resultList.Add((splitData[index], flag));

            index += 2;
        }

        return (count, resultList);
    }

    public async Task<(int, Byte[])> Get(string path)
    {
        await this.writer.WriteLineAsync($"2 {path}");

        throw new NotImplementedException();
    }

    public void Dispose()
    {
        this.writer.Close();
        this.reader.Close();

        this.client.Dispose();
    }
}