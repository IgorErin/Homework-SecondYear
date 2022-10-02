using Optional;

namespace ThreadPool;

public class ResultCell<TResult>
{
    private Lazy<TResult> _lazyResult;

    public ResultCell(Func<TResult> func)
    {
        _lazyResult = new Lazy<TResult>(func);
    }

    public TResult Result()
    {
        return _lazyResult.Value;
    }
}
