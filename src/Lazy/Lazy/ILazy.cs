namespace Lazy.Lazy;

/// <summary>
/// Lazy evaluation class interface.
/// </summary>
/// <typeparam name="T">Expression type for lazy evaluation.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// A method that returns a lazily evaluated value or throws an exception.
    /// Returning the same object each time in the case of a reference type.
    /// </summary>
    /// <returns>Expression result</returns>
    public T Get();
}
