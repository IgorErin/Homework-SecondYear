namespace ThreadPool;

using Extensions;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Optional;
using Exceptions;
using Lazy.Lazy;

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
            throw new ArgumentException(
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

        lock (this.locker)
        {
            try
            {
                if (!this.isShutDown)
                {
                    var newTask = new MyTask<TResult>(this, func);

                    resultTask = newTask.Some<MyTask<TResult>>();

                    this.queue.Add(newTask.Compute);
                }
            }
            catch (Exception e)
            {
                throw new MyThreadPoolException($"submit error:{e.Message}", e);
            }
        }

        return resultTask.ValueOr(() => throw new MyThreadPoolException("submit error, task not added"));
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

    private void SubmitAction(Action action)
    {
        try
        {
            this.queue.Add(action);
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
        private readonly MyThreadPool threadPool;

        private readonly Lazy<TResult> lazyFun;

        private readonly ActionExecutor actionExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
        /// </summary>
        /// <param name="threadPool">Thread pool that perform computations.</param>
        /// <param name="computationCell">Cell that encapsulates calculation and state of result.</param> //TODO()
        /// <param name="actions">Actions that will be performed immediately after the completion of the task.</param>
        public MyTask(MyThreadPool threadPool, Func<TResult> func)
        {
            this.threadPool = threadPool;
            this.actionExecutor = new ActionExecutor();

            this.lazyFun = new Lazy<TResult>(() =>
            {
                try
                {
                    return func.Invoke();
                }
                catch (Exception e)
                {
                    throw new AggregateException(e);
                }
                finally
                {
                    lock (this.actionExecutor)
                    {
                        this.actionExecutor.CompleteAdding();
                        this.actionExecutor.Execute();
                    }
                }
            });
        }

        /// <summary>
        /// Gets a value indicating whether task is completed.
        /// </summary>
        public bool IsCompleted => this.lazyFun.IsValueCreated;

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
                if (this.lazyFun.IsValueCreated)
                {
                    return this.lazyFun.Value;
                }

                this.ComputeLazy(this.lazyFun);

                return this.lazyFun.Value;
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
            var newTask = new MyTask<TNewResult>(this.threadPool, () =>
            {
                var result = this.lazyFun.Value;

                return continuation.Invoke(result);
            });

            if (this.IsCompleted)
            {
                this.threadPool.SubmitAction(newTask.Compute);

                return newTask;
            }

            lock (this.actionExecutor)
            {
                if (this.IsCompleted)
                {
                    this.threadPool.SubmitAction(newTask.Compute);
                }
                else
                {
                    this.actionExecutor.AddAction(() => this.threadPool.SubmitAction(newTask.Compute));
                }
            }

            return newTask;
        }

        public void Compute()
        {
            this.ComputeLazy(this.lazyFun);
        }

        private void ComputeLazy(Lazy<TResult> lazy)
        {
            try
            {
                lazy.Value.Ignore(); // boxing...
            }
            catch (Exception)
            {
            }
        }

        class ActionExecutor
        {
            private readonly BlockingCollection<Action> actions = new ();

            public void AddAction(Action action)
            {
                this.actions.Add(action);
            }

            public void Execute()
            {
                foreach (var action in this.actions)
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            public void CompleteAdding()
            {
                this.actions.CompleteAdding();
            }
        }
    }
}