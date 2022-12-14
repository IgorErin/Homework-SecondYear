using System.Net;
using Test2;

var noUnitTypeInCSharp = args switch
{
    [var port]     => RunServer(port),
    [var ipAddress, var port] => RunClient(ipAddress, port),

    _   => throw new InvalidOperationException($"strange parameters, try again"),
};

object RunServer(string port)
{
    var printer = new ConsolePrinter();
    
    var server = new Server(8888, printer);
    server.Start();
    
    var writer = new ConsoleReader(server);
    writer.Start();

    return null;
}

object RunClient(string stringPort, string stringIpAddress)
{
    var port = Int32.Parse(stringPort);

    if (IPAddress.TryParse(stringIpAddress, out var ipAddress))
    {
        throw new Exception("ip address incompatible");
    }
    
    var printer = new ConsolePrinter();
    
    var client = new Client(port, ipAddress, printer);
    client.Start();
    
    var writer = new ConsoleReader(client);
    writer.Start();
    
    return null;
}

