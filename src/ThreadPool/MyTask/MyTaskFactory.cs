using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using ThreadPool.Exceptions;

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

    public static (MyTask<T>, ComputationCell<object>) CreateContinuation<T>(Func<T> func, MyThreadPool threadPool)
    {
        var newComputationCell = new ComputationCell<T>(func);

        var enqueueFun = () =>
        {
            threadPool.EnqueueCell(newComputationCell);

            return new object();
        };
        
        var exception = new MyTaskException();
        var enqueueCell = new ComputationCell<object>(enqueueFun, () => throw exception);

        var newTaskFun = () =>
        {
            var _ = enqueueCell.Result;

            return newComputationCell.Result;
        };

        var (newTask, _) = CreateNewTaskAndCell(newTaskFun, threadPool);

        return (newTask, enqueueCell);
    }
}