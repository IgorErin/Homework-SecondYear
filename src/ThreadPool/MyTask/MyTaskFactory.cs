namespace ThreadPool.MyTask;

using System.Collections.Concurrent;
using Exceptions;

/// <summary>
/// <see cref="MyTask{TResult}"/> factory.
/// </summary>
public static class MyTaskFactory
{
    /// <summary>
    /// Method that allows you to create a new <see cref="ComputationCell{TResult}"/> and a
    /// <see cref="MyTask{TResult}"/> from a <see cref="Func{TResult}"/>.
    /// </summary>
    /// <param name="newFunc">Function for abstracting.</param>
    /// <param name="threadPool"><see cref="MyThreadPool"/> on which the calculations will take place.</param>
    /// <typeparam name="T">Result type of <see cref="Func{TResult}"/>.</typeparam>
    /// <returns>Pair of elements - a <see cref="MyTask{TResult}"/> and a <see cref="ComputationCell{TResult}"/>
    /// encapsulating the calculation of the task.
    /// </returns>
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

    /// <summary>
    /// Method that allows you to create a new <see cref="ComputationCell{TResult}"/> and a
    /// <see cref="MyTask{TResult}"/> from a <see cref="Func{TResult}"/> for continuation.
    /// </summary>
    /// <param name="func">Function for abstracting.</param>
    /// <param name="threadPool"><see cref="MyThreadPool"/> on which the calculations will take place.</param>
    /// <typeparam name="T">Type of <see cref="Func{TResult}"/> result.</typeparam>
    /// <returns>
    /// Pair of elements - a <see cref="MyTask{TResult}"/> and a <see cref="ComputationCell{TResult}"/>
    ///  encapsulating an attempt to put a task on a <see cref="MyThreadPool"/>.
    /// </returns>
    public static (MyTask<T>, ComputationCell<object>) CreateContinuation<T>(Func<T> func, MyThreadPool threadPool)
    {
        var newComputationCell = new ComputationCell<T>(func);

        var enqueueFun = () =>
        {
            threadPool.EnqueueCell(newComputationCell);

            return new object();
        };

        var enqueueCell = new ComputationCell<object>(enqueueFun);

        var newTaskFun = () =>
        {
            var _ = enqueueCell.Result;

            return newComputationCell.Result;
        };

        var (newTask, _) = CreateNewTaskAndCell(newTaskFun, threadPool);

        return (newTask, enqueueCell);
    }
}
