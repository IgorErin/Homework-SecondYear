namespace Lazy.Lazy;

using Exceptions;
using Optional;

/// <summary>
/// Lazy evaluation class for single-threaded execution.
/// <inheritdoc cref="Lazy{T}"/>
/// </summary>
/// <typeparam name="T">Result type of lazy computed expression, see <see cref="Lazy{T}"/>.</typeparam>
public class UnsafeLazy<T> : Lazy<T>
{
    private Func<T>? func;

    private Option<T> result = Option.None<T>();
    private Exception cachedException = new NotCachedResultException();

    private bool isComputed;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeLazy{T}"/> class.
    /// </summary>
    /// <param name="func">A function that will be lazily evaluated.</param>
    public UnsafeLazy(Func<T> func)
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
            this.Compute();
        }

        return this.result.Match(
            some: value => value,
            none: () => throw this.cachedException);
    }

    private void Compute()
    {
        try
        {
            this.result = this.func!.Invoke().Some<T>();
        }
        catch (Exception exception)
        {
            this.cachedException = exception;
        }
        finally
        {
            this.func = null;
            this.isComputed = true;
        }
    }
}
