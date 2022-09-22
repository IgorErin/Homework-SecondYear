namespace Lazy.Lazy;

/// <summary>
/// A base class for <see cref="ParallelSafeLazy{T}"/> and <see cref="SequencePosition"/>
/// That encapsulates the status of a computation <see cref="ComputationStatus"/>.
/// Inherits <inheritdoc cref="ILazy{T}"/>
/// See <see cref="ILazy{T}"/>
/// </summary>
/// <typeparam name="T">Result type of lazy computed expression, see <see cref="ILazy{T}"/></typeparam>
public abstract class Lazy<T> : ILazy<T>
{
    public abstract T Get();
    
    protected enum ComputationStatus
    {
        NotComputedYet,
        SuccessComputed,
        ComputedWithException
    }
}
