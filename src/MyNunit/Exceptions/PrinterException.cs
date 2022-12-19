namespace MyNunit.Exceptions;

/// <summary>
/// <see cref="ITestPrinter"/>
/// </summary>
public class PrinterException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrinterException"/> class.
    /// </summary>
    public PrinterException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrinterException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public PrinterException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrinterException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception.</param>
    public PrinterException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}