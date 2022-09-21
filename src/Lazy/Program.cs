using Lazy.Lazy;
using Microsoft.CSharp.RuntimeBinder;

namespace Lazy;
public static class LazyMain
{
    private const int tryCount = 10;
    public static void Main()
    {
        Console.WriteLine("Lazy computation example: ");
        
        var seqLazy = new SequentialSafeLazy<int>(() =>
        {
            Console.WriteLine("I computed!");
            
            return 1;
        });

        for (var tryIndex = 0; tryIndex < tryCount; tryIndex++)
        {
            var result = seqLazy.Get();
        }

        var parLazy = new SequentialSafeLazy<int>(() =>
        {
            Console.WriteLine("I computed!");
            
            return 1;
        });

        for (var tryIndex = 0; tryIndex < tryCount; tryIndex++)
        {
            var result = parLazy.Get();
        }
    }
}