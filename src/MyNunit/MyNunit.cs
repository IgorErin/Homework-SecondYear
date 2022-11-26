namespace MyNunit;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Attributes;
using Exceptions;
using TestsInfo;
using Optional;

public class MyNunit
{
    private static readonly Stopwatch methodStopwatch = new ();
    private static readonly Stopwatch typeStopWatch = new();
    private static readonly Stopwatch assemblyStopWatch = new();
    private static readonly object[] emptyArgs = Array.Empty<object>();

    private const string typePassedMessage = "Type passed";
    private const string typeFailedMessage = "Type failed";
    private const string typeNotMatchMessage = "Class doesn't match to test class";

    public TestAssemblyInfo RunTestsFrom(string pathToAssembly)
    {
        var assembly = Assembly.LoadFrom(pathToAssembly);

        return RunAssemblyTests(assembly);
    }

    public TestAssemblyInfo RunAssemblyTests(Assembly assembly)
    {
        var resultCollection = new ConcurrentQueue<TestClassInfo>();

        assemblyStopWatch.Reset();
        assemblyStopWatch.Start();

        foreach (var type in assembly.ExportedTypes)
        {
            if (type.GetConstructor(Type.EmptyTypes) != null)
            {
                Task.Run(() => resultCollection.Enqueue(RunTypeTests(type)));
            }
            else
            {
                resultCollection.Enqueue(new TestClassInfo(typeNotMatchMessage, type.GetTypeInfo()));
            }
        }

        assemblyStopWatch.Stop();
        return new TestAssemblyInfo(assemblyStopWatch.ElapsedMilliseconds, resultCollection, assembly);
    }

    public static TestClassInfo RunTypeTests(Type type)
    {
        var typeInfo = type.GetTypeInfo();

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
            return new TestClassInfo(typeStopWatch.ElapsedMilliseconds, results, testRunTimeException.Some(), typeFailedMessage, typeInfo);
        }

        typeStopWatch.Stop();
        return new TestClassInfo(typeStopWatch.ElapsedMilliseconds, results, typePassedMessage, typeInfo);
    }

    private static TestInfo RunMethodTest(
        object instance,
        List<MethodInfo> beforeTestMethods,
        MethodInfo testMethod,
        List<MethodInfo> afterTestMethods)
    {
        Exception resultException = new SuccessException();

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
                    testRunTimeException.InnerException ?? throw new NullReferenceException("Inner exception of target call"),
                    firstAttribute.Expected,
                    firstAttribute.Ignore,
                    methodStopwatch.ElapsedMilliseconds);
            }
            methodStopwatch.Stop();

            RunInstanceMethodsWithEmptyArgs(instance, afterTestMethods);
        }
        catch (Exception testRunTimeException)
        {
            resultException = testRunTimeException;
        }

        var attribute = GetTestAttribute(testMethod);

        return new TestInfo(
            testMethod,
            resultException,
            attribute.Expected,
            attribute.Ignore,
            methodStopwatch.ElapsedMilliseconds);
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
            method.Invoke(null, emptyArgs);
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