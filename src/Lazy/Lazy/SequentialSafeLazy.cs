using Lazy.LazyExceptions;
using Optional;

namespace Lazy.Lazy;

/// <summary>
/// Lazy evaluation class for single-threaded execution.
/// <inheritdoc cref="Lazy{T}"/>
/// </summary>
/// <typeparam name="T">Result type of lazy computed expression, see <see cref="Lazy{T}"/></typeparam>
public class SequentialSafeLazy<T> : Lazy<T>
{

    /// <summary>
    /// Constructor for init lazy computation class, see <see cref="ThreadSafeLazy{T}"/>.
    /// </summary>
    /// <param name="func">A function that will be lazily evaluated</param>
    public SequentialSafeLazy(Func<T> func) : base(func)
    {
    }

    /// <summary>
    /// A method that returns a lazily evaluated value or throws an exception.
    /// Returning the same object each time in the case of a reference type.
    /// Or thrown the same exception each time.
    /// </summary>
    /// <returns>Expression result</returns>
    /// <exception cref="ComputedStatusNotMatchLazyException">Throws an exception on status mismatch</exception>
    public override T Get()
    {
        if (ComputationCell.IsComputed)
        {
            return ComputationCell.Result;
        }

        ComputationCell.Compute();

        return ComputationCell.Result;
    }
}
