namespace Queue;

/// <summary>
/// <see cref="BlockingQueue{TValue}"/> test class.
/// </summary>
public class Tests
{
    /// <summary>
    /// Blocking test when calling the dequeue at an empty <see cref="BlockingQueue{TValue}"/>.
    /// </summary>
    [Test]
    public void EmptyQueueInAnotherThreadDequeue()
    {
        var queue = new BlockingQueue<int>();

        Task.Run(() => queue.Enqueue(0, 0));

        Assert.That(queue.Dequeue(), Is.EqualTo(0));
    }

    /// <summary>
    /// Enqueue and dequeue test.
    /// </summary>
    [Test]
    public void EnqueueDequeueTest()
    {
        var queue = new BlockingQueue<int>();
        var value = 2;
        var priority = 2;

        queue.Enqueue(value, priority);

        Assert.That(queue.Dequeue(), Is.EqualTo(value));
    }

    /// <summary>
    /// Correct removal of items with the same priority from the queue.
    /// </summary>
    /// <param name="firstValue"></param>
    /// <param name="secondValue"></param>
    [Test]
    [TestCase(1, 2)]
    [TestCase(2, 1)]
    public void SamePriorityDequeueOrder(int firstValue, int secondValue)
    {
        var queue = new BlockingQueue<int>();
        var priority = 1;

        queue.Enqueue(firstValue, priority);
        queue.Enqueue(secondValue, priority);

        Assert.That(queue.Dequeue(), Is.EqualTo(firstValue));
    }

    /// <summary>
    /// Test that the elements with the highest priority are retrieved earlier
    /// from <see cref="BlockingQueue{TValue}"/>.
    /// </summary>
    [Test]
    public void HigherPriorityHasAHigherPriority()
    {
        var queue = new BlockingQueue<int>();

        queue.Enqueue(1, 1);
        queue.Enqueue(2, 2);

        Assert.That(queue.Dequeue(), Is.EqualTo(2));
    }

    /// <summary>
    /// Empty <see cref="BlockingQueue{TValue}.Size()"/> test.
    /// </summary>
    [Test]
    public void EmptyQueueSizeTest() =>
        Assert.That(new BlockingQueue<int>().Size(), Is.EqualTo(0));

    /// <summary>
    /// <see cref="BlockingQueue{TValue}"/> size test.
    /// </summary>
    /// <param name="size"></param>
    [Test]
    [TestCase(1)]
    [TestCase(5)]
    [TestCase(10)]
    public void QueueSizeTest(int size)
    {
        var queue = new BlockingQueue<int>();
        
        for (var _ = 0; _ < size; _++)
        {
            queue.Enqueue(1, 1);
        }
        
        Assert.That(queue.Size(), Is.EqualTo(size));
    }
}