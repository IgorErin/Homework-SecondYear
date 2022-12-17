namespace MyNunit.Tests.TypeTest;

using System.Reflection;
using Attributes;
using Exceptions;
using MethodTest;
using Optional;
using Printer;

public class TypeTest
{
    private static readonly object[] EmptyArgs = Array.Empty<object>();

    private readonly TypeInfo typeInfo;

    private List<MethodTest> resultTests = new ();
    private Option<Exception> exception = Option.None<Exception>();

    private TypeTestStatus typeStatus;

    public TypeTest(Type type)
    {
        this.typeInfo = type.GetTypeInfo();
        this.typeStatus = GetTypeStatus(this.typeInfo);
    }

    public void Run()
    {
        if (this.typeStatus != TypeTestStatus.Compatible)
        {
            return;
        }

        try
        {
            RunStaticMethodsWithEmptyArgs(this.typeInfo, typeof(BeforeClassAttribute));
        }
        catch (Exception testRunTimeException)
        {
            this.exception = testRunTimeException.Some<Exception>();
            this.typeStatus = TypeTestStatus.BeforeFailed;

            return;
        }

        var instance = Activator.CreateInstance(this.typeInfo) ??
                       throw new ClassInstantiationException("Class cannot be instantiated");

        var beforeTestMethods = GetMethodsWithAttribute(typeof(BeforeAttribute), this.typeInfo);
        var afterTestMethods = GetMethodsWithAttribute(typeof(AfterAttribute), this.typeInfo);

        var testMethods = GetMethodsWithAttribute(typeof(TestAttribute), this.typeInfo);

        this.resultTests.Clear();

        foreach (var method in testMethods)
        {
            var test = new MethodTest(instance, beforeTestMethods, method, afterTestMethods);

            test.Run();
            this.resultTests.Add(test);
        }

        try
        {
            RunStaticMethodsWithEmptyArgs(this.typeInfo, typeof(AfterClassAttribute));
        }
        catch (Exception testRunTimeException)
        {
            this.exception = testRunTimeException.Some<Exception>();
            this.typeStatus = TypeTestStatus.AfterFailed;

            return;
        }

        this.typeStatus = this.resultTests.Count switch
        {
            0 => TypeTestStatus.NoTestsFound,
            _ => this.typeStatus = TypeTestStatus.Passed
        };
    }

    private static TypeTestStatus GetTypeStatus(Type typeInfo)
    {
        if (typeInfo.GetConstructor(Type.EmptyTypes) == null)
        {
            return TypeTestStatus.IncompatibleConstructorParameters;
        }

        if (typeInfo.IsAbstract)
        {
            return TypeTestStatus.AbstractType;
        }

        return TypeTestStatus.Compatible;
    }

    private static bool IsMethodHasAttribute(MethodInfo methodInfo, Type attributeType)
        => methodInfo.GetCustomAttributes(attributeType).Any();

    private static void RunStaticMethodsWithEmptyArgs(TypeInfo typeInfo, Type methodAttributeType)
    {
        var staticMethods = GetMethodsWithAttribute(methodAttributeType, typeInfo);

        foreach (var method in staticMethods)
        {
            if (method.IsStatic)
            {
                method.Invoke(null, EmptyArgs);
            }
        }
    }

    private static List<MethodInfo> GetMethodsWithAttribute(Type attributeType, TypeInfo typeInfo)
    {
        var compatibleMethods = new List<MethodInfo>();

        foreach (var method in typeInfo.DeclaredMethods)
        {
            if (IsMethodHasAttribute(method, attributeType))
            {
                compatibleMethods.Add(method);
            }
        }

        return compatibleMethods;
    }

    public void Print(ITestPrinter printer)
    {
        printer.PrintTypeTest(this.typeInfo, this.typeStatus, this.exception);

        foreach (var methodTest in this.resultTests)
        {
            methodTest.Print(printer);
        }
    }
}