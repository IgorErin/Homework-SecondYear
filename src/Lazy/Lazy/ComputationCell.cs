using Lazy.ComputationCellExceptions;

namespace Lazy.Lazy;

/// <summary>
/// Class that encapsulates calculations and caches the result.
/// Thread unsafe.
/// </summary>
/// <typeparam name="T">Result type.</typeparam>
public class ComputationCell<T>
{
    private readonly Func<T> _func;
    private Func<T> _funcResult = () => throw new NotCachedResultComputationCellException();

    private volatile bool _isComputed;
    
    /// <summary>
    /// Property that allows you to get the result.
    /// If the result has not yet been calculated, calls the method <see cref="Compute"/> and return the result.
    /// </summary>
    public T Result
    {
        get
        {
            if (!_isComputed)
            {
                Compute();
            }

            return _funcResult.Invoke();
        }
    }

    /// <summary>
    /// Property indicating whether the result is calculated.
    /// </summary>
    public bool IsComputed
    {
        get => _isComputed;
    }
    
    /// <summary>
    /// Class constructor.
    /// </summary>
    /// <param name="func">Function to calculate</param>
    public ComputationCell(Func<T> func)
    {
        _func = func;
    }
    
    /// <summary>
    /// function to calculate.
    /// </summary>
    public void Compute()
    {
        try
        {
            var result = _func.Invoke();

            _funcResult = () => result;
        }
        catch (Exception resultException)
        {
            _funcResult = () => throw resultException;
        }
        finally
        {
            _isComputed = true;
        }
    }
}
