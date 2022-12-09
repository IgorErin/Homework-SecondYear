// See https://aka.ms/new-console-template for more information

using System.Threading.Tasks.Dataflow;

class MainFlow
{
    private class Message
    {
        public bool IsCompatible
        {
            get;
            set;
        }

        public int Value
        {
            get;
            set;
        }
    }
    
    static async Task Main()
    {
        // var actionBlock = new ActionBlock<Message>(async message =>
        // {
        //     var _ = await SomeAsyncComputation(message.Value);
        //     Console.WriteLine(message);
        // });
        //
        // var actionBlock1 = new ActionBlock<Message>(async message =>
        // {
        //     var _ = await SomeAsyncComputation(message.Value);
        //     Console.WriteLine(message);
        // });
        //
        // var actionBlock2 = new ActionBlock<Message>(async message =>
        // {
        //     var _ = await SomeAsyncComputation(message.Value);
        //     Console.WriteLine(message);
        // });
        //
        //
        // var transformBlock = new TransformBlock<Message, Message>(async message =>
        // {
        //     var response = await SomeAsyncComputation(message.Value);
        //     message.Value += 1;
        //     return message;
        // }, new ExecutionDataflowBlockOptions
        // {
        //     MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
        //     EnsureOrdered = false
        // });
        //
        // transformBlock.LinkTo(actionBlock1);
        // transformBlock.LinkTo(actionBlock2);
        //
        // transformBlock.LinkTo(actionBlock, new DataflowLinkOptions()
        // {
        //     PropagateCompletion = true
        // }, message => message.IsCompatible);
        //
        // transformBlock.Post(new Message {IsCompatible = true});
        // transformBlock.Post(new Message {IsCompatible = false});
        // transformBlock.Post(new Message {IsCompatible = true});
        //
        // transformBlock.Complete();
        // transformBlock.Completion.Wait();
        //
        // actionBlock.Complete();
        // actionBlock.Completion.Wait();
        //
        // var buffer = new BufferBlock<Message>(new DataflowBlockOptions() { BoundedCapacity = 5 });
        
        var producer1 = new BufferBlock<int>();
        var producer2 = new BufferBlock<int>();

        var batchBlock = new BatchBlock<int>(2, new GroupingDataflowBlockOptions
        {
            Greedy = true
        });

        producer1.LinkTo(batchBlock);
        producer2.LinkTo(batchBlock);

        var actionBlock = new ActionBlock<IEnumerable<int>>(values =>
        {
            Console.WriteLine(values.Average());
        });

        batchBlock.LinkTo(actionBlock);

        producer1.Post(10);
        producer1.Post(10);
        Thread.Sleep(10);
        producer2.Post(20);
        producer2.Post(20);
    }
    
    public static async Task<int> SomeAsyncComputation(int message)
    {
        await Task.Delay(1000);
        
        return message;
    }
}