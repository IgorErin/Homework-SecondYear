namespace Lazy.Lazy;

/// <summary>
/// A base class for <see cref="SafeLazy{T}"/> and <see cref="SequencePosition"/>
/// Inherits <inheritdoc cref="ILazy{T}"/>
/// See <see cref="ILazy{T}"/>
/// </summary>
/// <typeparam name="T">Result type of lazy computed expression, see <see cref="ILazy{T}"/></typeparam>
public abstract class Lazy<T> : ILazy<T>
{
    public abstract T Get();
    protected readonly ComputationCell<T> ComputationCell;

    protected Lazy(Func<T> func)
    {
        ComputationCell = new ComputationCell<T>(func);
    }
}
