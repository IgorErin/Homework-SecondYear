namespace Queue;

public class Tests
{
    [Test]
    public void EmptyQueueInAnotherThreadDequeue()
    {
        var queue = new BlockingQueue<int>();

        Task.Run(() => queue.Enqueue(0, 0));

        Assert.That(queue.Dequeue(), Is.EqualTo(0));
    }

    [Test]
    public void EnqueueDequeueTest()
    {
        var queue = new BlockingQueue<int>();
        var value = 2;
        var priority = 2;

        queue.Enqueue(value, priority);

        Assert.That(queue.Dequeue(), Is.EqualTo(value));
    }

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

    [Test]
    public void HigherPriorityHasAHigherPriority()
    {
        var queue = new BlockingQueue<int>();

        queue.Enqueue(1, 1);
        queue.Enqueue(2, 2);

        Assert.That(queue.Dequeue(), Is.EqualTo(2));
    }

    [Test]
    public void EmptyQueueSizeTest() =>
        Assert.That(new BlockingQueue<int>().Size(), Is.EqualTo(0));
}