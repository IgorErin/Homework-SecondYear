using System.Dynamic;

namespace Lazy.Lazy;

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