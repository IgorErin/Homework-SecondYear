namespace ThreadPool;

using Optional;
using Exceptions;

/// <summary>
/// The class encapsulates the calculation, its result and state.
/// </summary>
/// <typeparam name="TResult">
/// Result type.
/// </typeparam>
public class ComputationCell<TResult>
{
    private readonly Func<TResult> func;

    private readonly object locker = new ();

    private Option<Func<TResult>> optionResult = Option.None<Func<TResult>>();

    private volatile bool funcIsComputed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComputationCell{TResult}"/> class.
    /// </summary>
    /// <param name="func">Function that will be computed.</param>
    public ComputationCell(Func<TResult> func)
    {
        this.func = func;
    }

    /// <summary>
    /// Gets a value indicating whether result is computed.
    /// </summary>
    public bool IsComputed => this.funcIsComputed;

    /// <summary>
    /// Gets the result of a calculation.
    /// </summary>
    /// <exception cref="ComputationCellException">
    /// If the result is not calculated, an exception will be thrown, see <see cref="ComputationCellException"/>.
    /// </exception>
    public TResult Result
    {
        get => this.optionResult.Match(
                some: result => result.Invoke(),
                none: () => this.ComputeAndGetResult());
    }

    /// <summary>
    /// The method that calculates the result.
    /// Thread safe.
    /// </summary>
    public void Compute()
    {
        lock (this.locker)
        {
            if (!this.funcIsComputed)
            {
                return;
            }

            try
            {
                var result = this.func.Invoke();
                var newResultFunc = () => result;

                this.optionResult = newResultFunc.Some();
            }
            catch (Exception exceptionResult)
            {
                var newExceptionFunc = new Func<TResult>(() => throw exceptionResult);

                this.optionResult = newExceptionFunc.Some();
            }
            finally
            {
                this.funcIsComputed = true;
            }
        }
    }

    private TResult ComputeAndGetResult()
    {
        this.Compute();

        return this.Result;
    }
}
