namespace ThreadPool.ResultCell;

/// <summary>
/// Enum that determine <see cref="ResultCell{TResult}"/> state.
/// </summary>
public enum ResultCellStatus 
{
    ResultNotComputed,
    ResultSuccessfullyComputed,
    ComputedWithException,
}
