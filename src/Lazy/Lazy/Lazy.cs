namespace Lazy.Lazy;

/// <summary>
/// A base class for <see cref="ThreadSafeLazy{T}"/> and <see cref="SequencePosition"/>
/// That encapsulates the status of a computation <see cref="ComputationStatus"/>.
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
