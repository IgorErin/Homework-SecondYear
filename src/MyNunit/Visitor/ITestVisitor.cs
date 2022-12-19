namespace MyNunit.Visitor;

using Tests.MethodTest;
using Tests.TypeTest;
using Tests.AssemblyTest;

/// <summary>
/// Visitor for <see cref="AssemblyTest"/>, <see cref="TypeTest"/>, <see cref="MethodTest"/>.
/// </summary>
public interface ITestVisitor
{
    /// <summary>
    /// Visit <see cref="AssemblyTest"/>.
    /// </summary>
    /// <param name="assembly">Assembly to visit.</param>
    public void Visit(AssemblyTest assembly);

    /// <summary>
    /// Visit <see cref="TypeTest"/>.
    /// </summary>
    /// <param name="typeTest">Type test to visit.</param>
    public void Visit(TypeTest typeTest);

    /// <summary>
    /// Visit <see cref="MethodTest"/>.
    /// </summary>
    /// <param name="methodTest">Method test to visit.</param>
    public void Visit(MethodTest methodTest);
}