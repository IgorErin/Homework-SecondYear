namespace ThreadPool;

public class ActionState
{
    private readonly object _actionStartLocker;
    private readonly object _actionCompleteLocker;

    public ActionState()
    {
        _actionStartLocker = new object();

        _actionCompleteLocker = new object();
        Monitor.Enter(_actionCompleteLocker);
    }

    public void GetResultBlocking()
    {
        Monitor.Enter(_actionCompleteLocker);
    }

    public bool TryStartAction()
        => Monitor.TryEnter(_actionStartLocker);

    public void ReleaseResultBlocking()
    {
        Monitor.Exit(_actionCompleteLocker);
    }
}
