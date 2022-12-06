namespace MyNunit.TestsInfo;

/// <summary>
/// Method <see cref="MyNunit"/> status.
/// </summary>
public enum MehtodStatus
{
    Compatible,
    Abstract,
    HaveParameters,
    AmbiguousAnnotations,
    Constrctor,
    SpecialName,
    ReturnParameter,
    ReturnedNotVoid,
}