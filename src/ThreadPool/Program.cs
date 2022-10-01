// See https://aka.ms/new-console-template for more information

using ThreadPool;

class PoolMain
{
    public static void Main()
    {
        var pool = new MyThreadPool(4);

        Console.WriteLine("start process");
        
        var task = pool.Submit(() =>
        {
            Console.Write("printed in thread pool");
            return 3;
        });

        Task.Delay(1000);

        task.ContinueWith(result =>
        {
            Console.Write($"result = {result}");
            return 3;
        });
        
        pool.ShutDown();
    }
}