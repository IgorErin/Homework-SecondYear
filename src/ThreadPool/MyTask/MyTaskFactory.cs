using System.Collections.Concurrent;

namespace ThreadPool.MyTask;

public static class MyTaskFactory
{
    public static (MyTask<T>, ComputationCell<T>) CreateNewTaskAndCell<T>(Func<T> newFunc, MyThreadPool threadPool)
    {
        var newCollection = new BlockingCollection<Action>();
            
        var subCell = new ComputationCell<T>(newFunc);
            
        var resultFunc = () =>
        {
            subCell.Compute();
            
            lock (newCollection)
            {
                newCollection.CompleteAdding();
            }

            foreach (var action in newCollection.GetConsumingEnumerable())
            {
                action.Invoke();
            }
            
            return subCell.Result;
        };
            
        var newCell = new ComputationCell<T>(resultFunc);

        var newTask = new MyTask<T>(threadPool, newCell, newCollection);

        return (newTask, newCell);
    }
}