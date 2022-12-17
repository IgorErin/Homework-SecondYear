namespace MyNunit.Tests.MethodTest;

public enum MethodStatus
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
    ReceivedException,
    IgnoredWithMessage,
}