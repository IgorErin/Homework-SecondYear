namespace MyNunit;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Attributes;
using Exceptions;
using TestsInfo;
using Optional;

/// <summary>
/// Class for testing assemblies and classes.
/// </summary>
public static class MyNunit
{
    private const string TypePassedMessage = "Type passed";
    private const string TypeFailedMessage = "Type failed";
    private const string TypeNotMatchMessage = "Class doesn't match to test class";

    private static readonly Stopwatch AssemblyStopWatch = new ();

    private static readonly object[] EmptyArgs = Array.Empty<object>();

    /// <summary>
    /// A method that tests all suitable classes in the assembly.
    /// </summary>
    /// <param name="assembly">Assembly for the test.</param>
    /// <returns>Returns information about test results in the form of <see cref="TestAssemblyInfo"/>.</returns>
    public static TestAssemblyInfo RunAssemblyTests(Assembly assembly)
    {
        var resultCollection = new ConcurrentQueue<TestClassInfo>();

        AssemblyStopWatch.Reset();
        AssemblyStopWatch.Start();

        Parallel.ForEach(assembly.ExportedTypes, type =>
        {
            if (type.GetConstructor(Type.EmptyTypes) != null && !type.IsAbstract)
            {
                Task.Run(() => resultCollection.Enqueue(RunTypeTests(type)));
            }
            else
            {
                resultCollection.Enqueue(new TestClassInfo(TypeNotMatchMessage, type.GetTypeInfo()));
            }
        });

        AssemblyStopWatch.Stop();
        return new TestAssemblyInfo(AssemblyStopWatch.ElapsedMilliseconds, resultCollection, assembly);
    }

    /// <summary>
    /// Method for running tests in a class.
    /// </summary>
    /// <param name="type">Type for testing.</param>
    /// <returns>Returns the results of type tests in the form <see cref="TestClassInfo"/>.</returns>
    /// <exception cref="ClassInstantiationException">Will be thrown out if the class cannot be instantiated.</exception>
    public static TestClassInfo RunTypeTests(Type type)
    {
        var typeInfo = type.GetTypeInfo();
        var typeStopWatch = new Stopwatch();

        var testMethods = GetMethodsWithAttribute(typeof(TestAttribute), typeInfo);
        var results = new List<TestInfo>();

        typeStopWatch.Reset();
        typeStopWatch.Start();

        try
        {
            RunStaticMethodsWithEmptyArgs(typeInfo, typeof(BeforeClassAttribute));

            var instance = Activator.CreateInstance(type) ??
                           throw new ClassInstantiationException("Class cannot be instantiated");

            var beforeTestMethods = GetMethodsWithAttribute(typeof(BeforeAttribute), typeInfo);
            var afterTestMethods = GetMethodsWithAttribute(typeof(AfterAttribute), typeInfo);

            foreach (var method in testMethods)
            {
                var testInfo = RunMethodTest(instance, beforeTestMethods, method, afterTestMethods);
                results.Add(testInfo);
            }

            RunStaticMethodsWithEmptyArgs(typeInfo, typeof(AfterClassAttribute));
        }
        catch (Exception testRunTimeException)
        {
            typeStopWatch.Stop();
            return new TestClassInfo(typeStopWatch.ElapsedMilliseconds, results, testRunTimeException.Some(), TypeFailedMessage, typeInfo);
        }

        typeStopWatch.Stop();
        return new TestClassInfo(typeStopWatch.ElapsedMilliseconds, results, TypePassedMessage, typeInfo);
    }

    private static TestInfo RunMethodTest(
        object instance,
        List<MethodInfo> beforeTestMethods,
        MethodInfo testMethod,
        List<MethodInfo> afterTestMethods)
    {
        var resultException = Option.None<Exception>();
        var methodStopwatch = new Stopwatch();

        try
        {
            RunInstanceMethodsWithEmptyArgs(instance, beforeTestMethods);

            methodStopwatch.Reset();
            methodStopwatch.Start();

            try
            {
                RunInstanceMethodWithEmptyArgs(instance, testMethod);
            }
            catch (Exception testRunTimeException)
            {
                methodStopwatch.Stop();

                var firstAttribute = GetTestAttribute(testMethod);
                RunInstanceMethodsWithEmptyArgs(instance, afterTestMethods);

                return new TestInfo(
                    testMethod,
                    testRunTimeException.InnerException?.Some<>() ?? Option.None<Exception>(),
                    firstAttribute.Expected?.Some<>() ?? Option.None<Exception>(),
                    firstAttribute.Ignore?.Some<>() ?? Option.None<string>(),
                    methodStopwatch.ElapsedMilliseconds);
            }

            methodStopwatch.Stop();

            RunInstanceMethodsWithEmptyArgs(instance, afterTestMethods);
        }
        catch (Exception testRunTimeException)
        {
            methodStopwatch.Stop();
            resultException = testRunTimeException.Some<>();
        }

        var attribute = GetTestAttribute(testMethod);

        return new TestInfo(
            testMethod,
            resultException,
            attribute.Expected?.Some<>() ?? Option.None<Exception>(),
            attribute.Ignore?.Some<string>() ?? Option.None<string>(),
            methodStopwatch.ElapsedMilliseconds);
    }

    private static bool IsMethodHasAttribute(MethodInfo methodInfo, Type attributeType)
        => methodInfo.GetCustomAttributes(attributeType).Any();

    private static void RunInstanceMethodWithEmptyArgs(object type, MethodInfo methodInfo)
        => methodInfo.Invoke(type, EmptyArgs);

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
            method.Invoke(null, EmptyArgs);
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
