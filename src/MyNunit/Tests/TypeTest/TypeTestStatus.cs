namespace MyNunit.Tests.TypeTest;

using Attributes;

/// <summary>
/// Type test states.
/// </summary>
public enum TypeTestStatus
{
    /// <summary>
    /// Type is abstract.
    /// </summary>
    AbstractType,

    /// <summary>
    /// Type has incompatible constructor.
    /// </summary>
    IncompatibleConstructorParameters,

    /// <summary>
    /// Type compatible for test.
    /// </summary>
    Compatible,

    /// <summary>
    /// Error executing the marked by <see cref="BeforeClassAttribute"/> static method.
    /// </summary>
    BeforeFailed,

    /// <summary>
    /// Error executing the marked by <see cref="AfterClassAttribute"/> static method.
    /// </summary>
    AfterFailed,

    /// <summary>
    /// Test passed without exception.
    /// </summary>
    Passed,

    /// <summary>
    /// No test found.
    /// </summary>
    NoTestsFound,
}