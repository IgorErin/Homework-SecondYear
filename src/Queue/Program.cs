using Queue;

var queue = new BlockingQueue<int>();

queue.Enqueue(1, 1);

queue.Enqueue(2, 1);

queue.Enqueue(3, 2);

queue.Enqueue(4, 3);

Console.WriteLine(queue.Dequeue());

Console.WriteLine(queue.Dequeue());

Console.WriteLine(queue.Dequeue());

Console.WriteLine(queue.Dequeue());