using System.Diagnostics;
using System.Reflection;
using MyNunit.Attributes;
using MyNunit.Exceptions;
using MyNunit.Method;
using Optional;

namespace MyNunit.TestsInfo;

public class TypeTest
{
    private readonly Type type;
    private readonly TypeInfo typeInfo;
    private static readonly object[] emptyArgs = Array.Empty<object>();

    private TypeStatus status;
    private IEnumerable<MethodTest> results;

    private Option<Exception> exceptionMessage = Option.None<Exception>();
    private Option<long> time = Option.None<long>();

    public TypeTest(Type type)
    {
        this.type = type;
        this.typeInfo = type.GetTypeInfo();
        this.status = TypeStatus.HaveConstructorParameters;
    }


    public void Run()
    {
        var typeStopWatch = new Stopwatch();

        var testMethods = GetMethodsWithAttribute(typeof(TestAttribute), this.typeInfo);
        var methodTests = new List<MethodTest>();

        typeStopWatch.Reset();
        typeStopWatch.Start();

        try
        {
            RunStaticMethodsWithEmptyArgs(this.typeInfo, typeof(BeforeClassAttribute));

            var instance = Activator.CreateInstance(this.type) ??
                           throw new ClassInstantiationException("Class cannot be instantiated");

            var beforeTestMethods = GetMethodsWithAttribute(typeof(BeforeAttribute), this.typeInfo);
            var afterTestMethods = GetMethodsWithAttribute(typeof(AfterAttribute), this.typeInfo);

            foreach (var method in testMethods)
            {
                var testInfo = new MethodTest(instance, beforeTestMethods, method, afterTestMethods);

                testInfo.Run();

                methodTests.Add(testInfo);
            }

            RunStaticMethodsWithEmptyArgs(this.typeInfo, typeof(AfterClassAttribute));
        }
        catch (Exception testRunTimeException)
        {
            typeStopWatch.Stop();
            this.exceptionMessage = testRunTimeException.Some<>();
        }

        typeStopWatch.Stop();

        this.time = typeStopWatch.ElapsedMilliseconds.Some<>();
        this.results = methodTests.Some<>();
    }

    private static bool IsMethodHasAttribute(MethodInfo methodInfo, Type attributeType)
        => methodInfo.GetCustomAttributes(attributeType).Any();

    private static void RunInstanceMethodWithEmptyArgs(object type, MethodInfo methodInfo)
        => methodInfo.Invoke(type, emptyArgs);

    private static TestAttribute GetTestAttribute(MethodInfo type)
        => (TestAttribute)(Attribute.GetCustomAttribute(type, typeof(TestAttribute))
                           ?? throw new NullReferenceException("Empty methods attributes"));

    private static void RunInstanceMethodsWithEmptyArgs(object instance, IEnumerable<MethodInfo> methods)
    {
        foreach (var afterMethod in methods)
        {
            RunInstanceMethodWithEmptyArgs(instance, afterMethod);
        }
    }

    private static void RunStaticMethodsWithEmptyArgs(TypeInfo typeInfo, Type methodAttributeType)
    {
        var staticMethods = GetMethodsWithAttribute(methodAttributeType, typeInfo);

        foreach (var method in staticMethods)
        {
            if (method.IsStatic)
            {
                method.Invoke(null, emptyArgs);
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
}