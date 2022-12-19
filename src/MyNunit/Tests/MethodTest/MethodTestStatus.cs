namespace MyNunit.Tests.MethodTest;

using Attributes;

/// <summary>
/// <see cref="MethodTest"/> states.
/// </summary>
public enum MethodTestStatus
{
    /// <summary>
    /// Method compatible for tests.
    /// </summary>
    Compatible,

    /// <summary>
    /// Method is constructor.
    /// </summary>
    Constructor,

    /// <summary>
    /// Generic method.
    /// </summary>
    Generic,

    /// <summary>
    /// Method has special name.
    /// </summary>
    SpecialName,

    /// <summary>
    /// Method has incompatible parameters.
    /// </summary>
    IncompatibleParameters,

    /// <summary>
    /// Method has incompatible return type.
    /// </summary>
    IncompatibleReturnType,

    /// <summary>
    /// Test passed.
    /// </summary>
    Passed,

    /// <summary>
    /// Error executing the marked <see cref="BeforeAttribute"/> method.
    /// </summary>
    BeforeFailed,

    /// <summary>
    /// Error executing the marked <see cref="AfterAttribute"/> method.
    /// </summary>
    AfterFailed,

    /// <summary>
    /// An exception was received during test execution.
    /// </summary>
    ReceivedExpectedException,

    /// <summary>
    /// An expected exception was received during test execution.
    /// </summary>
    ReceivedUnexpectedException,

    /// <summary>
    /// Test marked as ingored.
    /// </summary>
    IgnoredWithMessage,
}