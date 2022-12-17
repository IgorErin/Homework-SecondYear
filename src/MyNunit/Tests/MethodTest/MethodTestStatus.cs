namespace MyNunit.Tests.MethodTest;

public enum MethodTestStatus
{
    Compatible,
    Constructor,
    Generic,
    SpecialName,
    IncompatibleParameters,
    IncompatibleReturnType,

    Passed,
    BeforeFailed,
    AfterFailed,
    ReceivedExpectedException,
    ReceivedUnexpectedException,
    IgnoredWithMessage,
}