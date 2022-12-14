using Lazy.Lazy;

Console.WriteLine("Lazy computation example: ");

var unsafeLazy = new UnsafeLazy<int>(() =>
{
    Console.WriteLine("I computed in UnsafeLazy! Should be printed once...");

    return 1;
});

for (var tryIndex = 0; tryIndex < 3; tryIndex++)
{
    Console.WriteLine($"Computed and stored value: {unsafeLazy.Get()}");
}

var safeLazy = new SafeLazy<int>(() =>
{
    Console.WriteLine("I computed in SafeLazy! Should be printed once...");

    return 1;
});

for (var tryIndex = 0; tryIndex < 3; tryIndex++)
{
    Console.WriteLine($"Computed and stored: {safeLazy.Get()}");
}
