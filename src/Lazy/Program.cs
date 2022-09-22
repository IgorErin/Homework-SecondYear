using Lazy.Lazy;

namespace Lazy;

/// <summary>
/// Main class in Lazy proj with Main method with lazy examples.
/// </summary>
public static class LazyMain
{
    private const int TryCount = 10;
    
    public static void Main()
    {
        Console.WriteLine("Lazy computation example: ");
        
        var seqLazy = new SequentialSafeLazy<int>(() =>
        {
            Console.WriteLine("I computed!");
            
            return 1;
        });

        for (var tryIndex = 0; tryIndex < TryCount; tryIndex++)
        {
            var result = seqLazy.Get();
        }

        var parLazy = new ParallelSafeLazy<int>(() =>
        {
            Console.WriteLine("I computed!");
            
            return 1;
        });

        for (var tryIndex = 0; tryIndex < TryCount; tryIndex++)
        {
            var result = parLazy.Get();
        }
    }
}