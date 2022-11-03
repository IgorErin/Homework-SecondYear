namespace Lazy.Lazy;

using ComputationCellExceptions;
using Optional;

/// <summary>
/// Lazy evaluation class for multi thread execution.
/// <inheritdoc cref="Lazy{T}"/>
/// </summary>
/// <typeparam name="T">Result type of lazy computed expression, see <see cref="Lazy{T}"/></typeparam>
public class SafeLazy<T> : Lazy<T>
{
    private readonly object locker = new ();

    private Func<T>? func;

    private Option<T> result = Option.None<T>();
    private Exception cachedException = new NotCachedResultException();

    private volatile bool isComputed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeLazy{T}"/> class.
    /// </summary>
    /// <param name="func">A function that will be lazily evaluated.</param>
    public SafeLazy(Func<T> func)
        => this.func = func;

    /// <summary>
    /// A method that returns a lazily evaluated value or throws an exception.
    /// Returning the same object each time in the case of a reference type.
    /// Or thrown the same exception each time.
    /// </summary>
    /// <returns>Expression result.</returns>
    public T Get()
    {
        if (!this.isComputed)
        {
            this.ComputeWithLock();
        }

        return this.result.Match(
            some: value => value,
            none: () => throw this.cachedException);
    }

    private void ComputeWithLock()
    {
        lock (this.locker)
        {
            if (!this.isComputed)
            {
                try
                {
                    this.result = this.func!.Invoke().Some<T>();
                }
                catch (Exception e)
                {
                    this.cachedException = e;
                }
                finally
                {
                    this.func = null;
                    this.isComputed = true;
                }
            }
            else
            {
                // if the value is calculated in another thread,
                // then exit the lock without calculating it.
            }
        }
    }
}
