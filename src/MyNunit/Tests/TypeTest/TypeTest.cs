namespace MyNunit.Tests.TypeTest;

using System.Reflection;
using Attributes;
using Exceptions;
using MethodTest;
using Optional;
using Optional.Unsafe;
using Printer;

public class TypeTest
{
    private static readonly object[] EmptyArgs = Array.Empty<object>();

    private readonly TypeInfo typeInfo;

    private Option<IEnumerable<MethodTest>> optionTestMethods = Option.None<IEnumerable<MethodTest>>();
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
            this.exception = testRunTimeException.Some<>();
            this.typeStatus = TypeTestStatus.BeforeFailed;

            return;
        }

        var instance = Activator.CreateInstance(this.typeInfo) ??
                       throw new ClassInstantiationException("Class cannot be instantiated");

        var beforeTestMethods = GetMethodsWithAttribute(typeof(BeforeAttribute), this.typeInfo);
        var afterTestMethods = GetMethodsWithAttribute(typeof(AfterAttribute), this.typeInfo);

        var testMethods = GetMethodsWithAttribute(typeof(TestAttribute), this.typeInfo);
        var typeTests = new List<MethodTest>();

        foreach (var method in testMethods)
        {
            var test = new MethodTest(instance, beforeTestMethods, method, afterTestMethods);

            test.Run();
            typeTests.Add(test);
        }

        try
        {
            RunStaticMethodsWithEmptyArgs(this.typeInfo, typeof(AfterClassAttribute));
        }
        catch (Exception testRunTimeException)
        {
            this.exception = testRunTimeException.Some<>();
            this.typeStatus = TypeTestStatus.AfterFailed;

            return;
        }

        this.typeStatus = TypeTestStatus.Passed;
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
        foreach (var methodTest in this.optionTestMethods.ValueOrFailure()) //TODO()
        {
            methodTest.Print(printer);
        }
    }
}