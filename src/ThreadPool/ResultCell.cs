using System.Diagnostics.CodeAnalysis;

namespace ThreadPool;

using Optional;

public class ResultCell<TResult>
{
    private Option<TResult> _optionResult = Option.None<TResult>();

    private readonly object _locker = new object();

    public ResultCell()
    {
    }

    public bool IsCompleted
        () => _optionResult.Match(
            some: value => true,
            none: () => false
        );
    

    public void SetResult(TResult result)
    {
        lock (_locker)
        {
            if (!_optionResult.HasValue)
            {
                _optionResult = result.Some();
            }
            else
            {
                //TODO()
            }
        }
    }

    public TResult GetResult()
        => _optionResult.Match(
            some: value => value,
            none: () => throw new Exception()
            );
}