// See https://aka.ms/new-console-template for more information

using ThreadPool;

class PoolMain
{
    public static void Main()
    {
        var pool = new MyThreadPool(12);

        Console.WriteLine("start process");

        for (var i = 0; i < 100; i++)
        {
            Task.Delay(100);
            var task = pool.Submit(() =>
            {
                Console.WriteLine("printed in thread pool");
                return 3;
            });
            
            Task.Delay(1000);

            task.ContinueWith(result =>
            {
                Console.WriteLine($"result = {result}");
                return 3;
            });
            
            Task.Delay(1000);
            Console.WriteLine($"i = {i}");
        }
        
        pool.ShutDown();
    }
}