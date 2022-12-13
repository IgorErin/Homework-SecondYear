namespace MyNunit.Tests;

using System.Diagnostics;
using System.Reflection;
using Attributes;
using Exceptions;
using Optional;

public class TypeTest : ITest
{
    private readonly Type type;
    private readonly TypeInfo typeInfo;

    private readonly Stopwatch stopwatch = new ();
    private static readonly object[] emptyArgs = Array.Empty<object>();

    private Option<IEnumerable<MethodTest>> results;
    private Option<Exception> exception = Option.None<Exception>();
    private Option<long> time = Option.None<long>();

    private TypeStatus typeStatus = TypeStatus.NotInit;

    public long Time
    {
        get;
    }

    public TestStatus Status
    {
        get;
        private set;
    }

    public string Message
    {
        get;
    }

    public TypeTest(Type type)
    {
        this.type = type;
        this.typeInfo = type.GetTypeInfo();
    }

    private enum TypeStatus
    {
        NotInit,
        AbstractType,
        IncompatibleConstructorParameters,
        Compatible,
    }

    public void Run()
    {
        this.typeStatus = GetTypeStatus(this.typeInfo);

        if (this.typeStatus != TypeStatus.Compatible)
        {
            this.time = 0.Some<>();
            this.Status = TestStatus.Ignored;

            return;
        }

        try
        {
            RunStaticMethodsWithEmptyArgs(this.typeInfo, typeof(BeforeClassAttribute));
        }
        catch (Exception testRunTimeException)
        {
            this.stopwatch.Stop();

            this.exception = testRunTimeException.Some<>();
            this.time = this.stopwatch.ElapsedMilliseconds.Some<>();
            this.Status = TestStatus.BeforeFailed; // TODO()

            return;
        }

        var instance = Activator.CreateInstance(this.type) ??
                       throw new ClassInstantiationException("Class cannot be instantiated");

        var beforeTestMethods = GetMethodsWithAttribute(typeof(BeforeAttribute), this.typeInfo);
        var afterTestMethods = GetMethodsWithAttribute(typeof(AfterAttribute), this.typeInfo);

        var testMethods = GetMethodsWithAttribute(typeof(TestAttribute), this.typeInfo);
        var typeTests = new List<MethodTest>();

        this.stopwatch.Reset();
        this.stopwatch.Start();

        foreach (var method in testMethods)
        {
            var test = new MethodTest(instance, beforeTestMethods, method, afterTestMethods);

            test.Run();

            typeTests.Add(test);
        }

        this.stopwatch.Stop();

        this.time = this.stopwatch.ElapsedMilliseconds.Some<>();
        this.results = typeTests.Some<>();

        try
        {
            RunStaticMethodsWithEmptyArgs(this.typeInfo, typeof(AfterClassAttribute));
        }
        catch (Exception testRunTimeException)
        {
            this.stopwatch.Stop();

            this.exception = testRunTimeException.Some<>();
            this.time = this.stopwatch.ElapsedMilliseconds.Some<>();

            this.Status = TestStatus.AfterFaild; //TODO()
        }
    }

    private static TypeStatus GetTypeStatus(Type typeInfo)
    {
        if (typeInfo.GetConstructor(Type.EmptyTypes) == null)
        {
            return TypeStatus.IncompatibleConstructorParameters;
        }

        if (typeInfo.IsAbstract)
        {
            return TypeStatus.AbstractType;
        }

        return TypeStatus.Compatible;
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