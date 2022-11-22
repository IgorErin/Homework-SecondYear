using MD5;

public class MDTest
{
    private readonly string currentDir = Directory.GetCurrentDirectory();

    [Test]
    public void ParalleAndSequentialResultsAreEqual()
    {
        var sequentialResult = Hash.getSum(currentDir).GetAwaiter().GetResult();

        var parallelResult = ParallelHash.getSum(currentDir).GetAwaiter().GetResult();

        Assert.AreEqual(sequentialResult, parallelResult);
    }

    [Test]
    public void SequentialResultInvariant()
    {
        var sequentialResult = Hash.getSum(currentDir).GetAwaiter().GetResult();

        for (var i = 0; i < 10; i++)
        {
            Assert.AreEqual(sequentialResult, Hash.getSum(currentDir).GetAwaiter().GetResult());
        }
    }

    [Test]
    public void ParallelResultInvariant()
    {
        var sequentialResult = ParallelHash.getSum(currentDir).GetAwaiter().GetResult();

        for (var i = 0; i < 10; i++)
        {
            Assert.AreEqual(sequentialResult, ParallelHash.getSum(currentDir).GetAwaiter().GetResult());
        }
    }
}