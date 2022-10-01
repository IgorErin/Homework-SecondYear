// See https://aka.ms/new-console-template for more information
class PoolMain
{
    private static readonly HttpClient _httpClient = new HttpClient();
    
    public static async Task Main()
    {
        Print("start async in main");

        var task = TestAsync();
        
        Print("start await in main");

        var lol = await task;
        
        Print("after async in main");


        System.Console.Write($"{lol}");
    }
    static async Task<string> TestAsync()
    {
        Print("start first async method");
        
        var html = await GetIntSync();
        

        Print("end of first async method, return html");
        
        return html;
    }

    static async Task<string> GetIntSync()
    {
        
        Print("start getting http clent");

        var html = await _httpClient.GetStringAsync("https://dotnetfoundation.org");

        
        Print("end getting http clent");


        return html;
    }
    
    static void Print(string message)
    {
        Console.WriteLine($"doSome work {message}");
    }
}