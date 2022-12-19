namespace MyNunit.Exceptions;

using Visitor;

/// <summary>
/// <see cref="ITestVisitor"/> exception.
/// </summary>
public class TestVisitorException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestVisitorException"/> class.
    /// </summary>
    public TestVisitorException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestVisitorException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public TestVisitorException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestVisitorException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception.</param>
    public TestVisitorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}