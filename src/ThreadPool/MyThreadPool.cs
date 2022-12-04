namespace ThreadPool;

using Extensions;
using System.Collections.Concurrent;
using Optional;
using Exceptions;

/// <summary>
/// Thread pool that implement <see cref="IDisposable"/>.
/// </summary>
public sealed class MyThreadPool : IDisposable
{
    private readonly BlockingCollection<Action> queue;

    private readonly CountdownEvent threadsCompletedEvent;

    private readonly object locker = new ();

    private volatile bool isDisposed;
    private volatile bool isShutDown;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="threadCount">
    /// Number strictly greater than zero - number of threads processing tasks.
    /// </param>
    public MyThreadPool(int threadCount)
    {
        if (threadCount <= 0)
        {
            throw new MyThreadPoolException(
                $"the number of threads must be greater than 0, but was = {threadCount}");
        }

        this.queue = new BlockingCollection<Action>();

        this.threadsCompletedEvent = new CountdownEvent(threadCount);

        for (var i = 0; i < threadCount; i++)
        {
            new ThreadPoolItem(this.queue, this.threadsCompletedEvent).Ignore();
        }
    }

    /// <summary>
    /// A method that allows you to add a task for execution in the form of <see cref="Func{TResult}"/>.
    /// </summary>
    /// <param name="func">Function whose value will be computed as one.</param>
    /// <typeparam name="TResult">Function result type.</typeparam>
    /// <returns>Abstraction over the task accepted for execution, see <see cref="IMyTask{TResult}"/>.</returns>
    public MyTask<TResult> Submit<TResult>(Func<TResult> func)
    {
        var resultTask = Option.None<MyTask<TResult>>();

        try
        {
            Monitor.Enter(this.locker);

            if (!this.isShutDown)
            {
                var (newTask, newCell) = CreateNewTaskAndCell(func, this);

                resultTask = newTask.Some<MyTask<TResult>>();

                this.queue.Add(() => newCell.Compute());
            }
        }
        catch (Exception e)
        {
            throw new MyThreadPoolException($"submit error:{e.Message}", e);
        }
        finally
        {
            if (Monitor.IsEntered(this.locker))
            {
                Monitor.Exit(this.locker);
            }
        }

        return resultTask.ValueOr(() => throw new MyThreadPoolException("submit error, task not added)"));
    }

    /// <summary>
    /// The blocking thread method in which it will be called,
    /// stopping the work of threads,
    /// all tasks accepted for execution are calculated.
    /// </summary>
    public void ShutDown()
    {
        lock (this.locker)
        {
            if (this.isShutDown)
            {
                return;
            }

            this.queue.CompleteAdding();

            this.threadsCompletedEvent.Wait();

            this.isShutDown = true;
        }
    }

    /// <summary>
    /// Method to release managed resources.
    /// </summary>
    public void Dispose()
    {
        if (!this.isShutDown)
        {
            this.ShutDown();
        }

        lock (this.locker)
        {
            if (!this.isDisposed)
            {
                this.queue.Dispose();
            }

            this.isDisposed = true;
        }
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
    private static (MyTask<T>, ComputationCell<object>) CreateContinuation<T>(Func<T> func, MyThreadPool threadPool)
    {
        var newComputationCell = new ComputationCell<T>(func);

        var enqueueFun = () =>
        {
            threadPool.EnqueueCell(newComputationCell);

            return new object();
        };

        var enqueueCell = new ComputationCell<object>(enqueueFun);
        var (newTask, _) = CreateNewTaskAndCell(() => newComputationCell.Result, threadPool);

        return (newTask, enqueueCell);
    }

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
    private static (MyTask<T>, ComputationCell<T>) CreateNewTaskAndCell<T>(Func<T> newFunc, MyThreadPool threadPool)
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
    /// A method that allows you to put the calculation of the <see cref="ComputationCell{TResult}"/>
    /// on the <see cref="MyThreadPool"/> without blocking.
    ///
    /// </summary>
    /// <param name="cell"><see cref="ComputationCell{TResult}"/> for calculations.</param>
    /// <typeparam name="T">Type of ComputationCell.</typeparam>
    private void EnqueueCell<T>(ComputationCell<T> cell)
    {
        try
        {
            this.queue.Add(() => cell.Compute());
        }
        catch (ObjectDisposedException e)
        {
            throw new MyThreadPoolException("the shutdown is called, the task cannot be completed", e);
        }
        catch (InvalidOperationException e)
        {
            throw new MyThreadPoolException("the shutdown is called, the task cannot be completed", e);
        }
    }

    /// <summary>
    /// Class that implement <see cref="IMyTask{TResult}"/>.<inheritdoc cref="IMyTask{TResult}"/>
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    public class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly ComputationCell<TResult> computationCell;
        private readonly MyThreadPool threadPool;

        private readonly BlockingCollection<Action> actions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
        /// </summary>
        /// <param name="threadPool">Thread pool that perform computations.</param>
        /// <param name="computationCell">Cell that encapsulates calculation and state of result.</param>
        /// <param name="actions">Actions that will be performed immediately after the completion of the task.</param>
        public MyTask(MyThreadPool threadPool, ComputationCell<TResult> computationCell, BlockingCollection<Action> actions)
        {
            this.computationCell = computationCell;
            this.threadPool = threadPool;

            this.actions = actions;
        }

        /// <summary>
        /// Gets a value indicating whether task is completed.
        /// </summary>
        public bool IsCompleted => this.computationCell.IsComputed;

        /// <summary>
        /// Gets the result of the evaluation or, if it has not yet been evaluated,
        /// blocks the calling thread until it is evaluated and returns the value.
        /// </summary>
        /// <exception cref="MyTaskException">Will be thrown on runtime error.</exception>
        /// <exception cref="AggregateException">
        /// Will be thrown at an exception in the computation, will contain it as inner exception.
        /// </exception>
        public TResult Result
        {
            get
            {
                try
                {
                    return this.GetResultFromComputationCell();
                }
                catch (ComputationCellException e)
                {
                    throw new MyTaskException("computation error: \n", e);
                }
                catch (Exception e)
                {
                    throw new AggregateException(e);
                }
            }
        }

        /// <summary>
        /// The method that allows you to get the continuation of the result in the form <see cref="IMyTask{TResult}"/>.
        /// </summary>
        /// <param name="continuation">
        /// The function to be curried function by the previous result.
        /// </param>
        /// <typeparam name="TNewResult">
        /// Result type of curried function.
        /// </typeparam>
        /// <returns>
        /// Abstraction over computation in form of <see cref="IMyTask{TResult}"/>.
        /// </returns>
        /// <exception cref="AggregateException">
        /// An exception will be thrown when the exception is thrown in the original task
        /// or its continuation, when the <see cref="Result"/> property is applied to it.
        /// </exception>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
        {
            var newFunc = () =>
            {
                var result = this.GetResultFromComputationCell();

                return continuation.Invoke(result);
            };

            if (this.IsCompleted || this.actions.IsAddingCompleted)
            {
                return this.threadPool.Submit(newFunc);
            }

            try
            {
                Monitor.Enter(this.actions);

                if (this.IsCompleted || this.actions.IsAddingCompleted)
                {
                    return this.threadPool.Submit(newFunc);
                }

                var (newTask, newCell) = CreateContinuation<TNewResult>(newFunc, this.threadPool);

                this.actions.Add(() => newCell.Compute());

                return newTask;
            }
            catch (Exception e)
            {
                throw new MyTaskException("Continue add error: ", e);
            }
            finally
            {
                if (Monitor.IsEntered(this.actions))
                {
                    Monitor.Exit(this.actions);
                }
            }
        }

        private TResult GetResultFromComputationCell()
        {
            if (!this.computationCell.IsComputed)
            {
                this.computationCell.Compute();
            }

            return this.computationCell.Result;
        }
    }
}