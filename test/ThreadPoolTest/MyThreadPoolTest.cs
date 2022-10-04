namespace ThreadPool;

public class MyThreadPoolTest
{
    [TestCase(-10)]
    [TestCase(0)]
    [TestCase(10)]
    public void ThreadCountTest(int threadCount)
    {
        using var threadPool = new MyThreadPool(threadCount);

        var expectedResult = 10;
        var task = threadPool.Submit(() => expectedResult);

        var taskResult = task.Result;
        
        
    }
}