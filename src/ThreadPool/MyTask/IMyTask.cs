using System;

namespace ThreadPool.MyTask;

/// <summary>
/// Abstraction interface over a task computed in a thread pull.
/// </summary>
/// <typeparam name="TResult">
/// Result type of task.
/// </typeparam>
public interface IMyTask<out TResult>
{
    /// <summary>
    /// A property indicating whether the calculation was performed or not.
    /// </summary>
    public bool IsCompleted
    {
        get;
    }

    /// <summary>
    /// A property that returns the result of the evaluation or,
    /// if it has not yet been evaluated,
    /// blocks the calling thread until it is evaluated and returns the value.
    /// </summary>
    public TResult Result
    {
        get;
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
    /// Abstraction over computation in form of <see cref="IMyTask{TResult}"/>
    /// </returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
}
