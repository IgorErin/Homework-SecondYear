using Lazy.Lazy;

namespace Lazy;

/// <summary>
/// Main class in Lazy proj with Main method with lazy examples.
/// </summary>
public static class LazyMain
{
    private const int TryCount = 3;
    
    public static void Main()
    {
        Console.WriteLine("Lazy computation example: ");
        
        var seqLazy = new SequentialSafeLazy<int>(() =>
        {
            Console.WriteLine("I computed in seqSafeLazy! Should be printed once...");
            
            return 1;
        });

        for (var tryIndex = 0; tryIndex < TryCount; tryIndex++)
        {
            Console.WriteLine($"Computed and stored value: {seqLazy.Get()}");
        }

        var parLazy = new ThreadSafeLazy<int>(() =>
        {
            Console.WriteLine("I computed in parSafeLazy! Should be printed once...");
            
            return 1;
        });

        for (var tryIndex = 0; tryIndex < TryCount; tryIndex++)
        {
            Console.WriteLine($"Computed and stored: {parLazy.Get()}");
        }
    }
}