namespace Lazy.Lazy;

public interface ILazy<T>
{
    public T Get();
    
    protected enum ComputationStatus
    {
        NotComputedYet,
        SuccessComputed,
        ComputedWithException
    }
}
