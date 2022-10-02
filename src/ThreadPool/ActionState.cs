namespace ThreadPool;

public class ActionState
{
    private readonly object _actionStartLocker;
    private readonly object _actionCompleteLocker;

    public bool IsCompleted
    {
        get
        {
            if (Monitor.IsEntered(_actionCompleteLocker))
            {
                Monitor.Exit(_actionCompleteLocker); //cringe

                return true;
            }

            return false;
        }
    }
    
    public ActionState()
    {
        _actionStartLocker = new object();

        _actionCompleteLocker = new object();
        Monitor.Enter(_actionCompleteLocker);
    }

    public void BlockUntilCompleted()
    {
        Monitor.Enter(_actionCompleteLocker);
        Monitor.Exit(_actionCompleteLocker);
    }

    public bool TryStartAction()
        => Monitor.TryEnter(_actionStartLocker);

    public void TaskCompleted()
    {
        Monitor.Exit(_actionCompleteLocker);
    }
}
