using Lazy.Lazy;
using Microsoft.CSharp.RuntimeBinder;

namespace Lazy;
public static class LazyMain
{
    public static void Main()
    {
        var lazy = new SequentialSafeLazy<int>(() =>
        {
            Console.WriteLine("I computed!");
            throw new RuntimeBinderException("lol");
            return 1;
        });


        var excList = new List<Exception>();
        
        for (var i = 0; i < 10; i++)
        {
            try
            {
                Console.WriteLine($"result = {lazy.Get()}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                excList.Add(e);
            }
        }

        foreach (var exc in excList)
        {
            Console.WriteLine($"exception = {exc.Message}");
        }
    }
}