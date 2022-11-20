namespace MyNunit;

using System.Diagnostics;
using System.Reflection;
using Attributes;
using Exceptions;
using Optional;

public class MyNunit
{
    private static readonly Stopwatch methodStopwatch = new ();
    private static readonly Stopwatch typeStopWatch = new();
    private static readonly Stopwatch assemblyStopWatch = new();
    private static readonly object[] emptyArgs = Array.Empty<object>();

    public TestAssemblyInfo RunTests(string pathToAssembly)
    {
        var assembly = Assembly.LoadFrom(pathToAssembly);

        return RunAssemblyTests(assembly);
    }

    private TestAssemblyInfo RunAssemblyTests(Assembly assembly)
    {
        var typeTests = new List<TestClassInfo>();

        assemblyStopWatch.Reset();
        assemblyStopWatch.Start();

        foreach (var type in assembly.ExportedTypes)
        {
            if (type.GetConstructor(Type.EmptyTypes) == null) //TODO()
            {
                typeTests.Add(new TestClassInfo( "Class doesn't match to test class", type));
                continue;
            }

            var resultTestClassInfo = RunTypeTests(type);

            foreach (var test in resultTestClassInfo.Tests)
            {
                Console.WriteLine($"Test: {test.Name} : {test.Status} => {test.Message}, Time: {test.Time}");
            }

            typeTests.Add(resultTestClassInfo);
        }

        assemblyStopWatch.Stop();

        return new TestAssemblyInfo(assemblyStopWatch.ElapsedMilliseconds, typeTests);
    }

    private TestClassInfo RunTypeTests(Type type)
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
                           throw new Exception("message that indicate that class is not instanced");

            var beforeTestMethods = GetMethodsWithAttribute(typeof(BeforeAttribute), typeInfo);
            var afterTestMethods = GetMethodsWithAttribute(typeof(AfterAttribute), typeInfo);

            for (var i = 0; i < testMethods.Count; i++)
            {
                var method = testMethods[i];
                var testInfo = RunMethodTest(instance, beforeTestMethods, method, afterTestMethods);

                results.Add(testInfo);
            }

            RunStaticMethodsWithEmptyArgs(typeInfo, typeof(AfterClassAttribute));
        }
        catch (Exception testRunTimeException)
        {
            typeStopWatch.Stop();
            return new TestClassInfo(typeStopWatch.ElapsedMilliseconds, results, testRunTimeException.Some(), "Type failed", type);
        }

        typeStopWatch.Stop();
        return new TestClassInfo(typeStopWatch.ElapsedMilliseconds, results, "Type passed", type);
    }

    private TestInfo RunMethodTest(
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
                resultException = testRunTimeException;
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

    private bool IsMethodHasAttribute(MethodInfo methodInfo, Type attributeType)
        => methodInfo.GetCustomAttributes(attributeType).Any();

    private void RunInstanceMethodWithEmptyArgs(object type, MethodInfo methodInfo)
        => methodInfo.Invoke(type, emptyArgs);

    private TestAttribute GetTestAttribute(MethodInfo type)
    {
        var attribute = Attribute.GetCustomAttribute(type, typeof(TestAttribute)) ?? throw new Exception(); //TODO()

        return (TestAttribute)attribute;
    }

    private void RunInstanceMethodsWithEmptyArgs(object instance, IEnumerable<MethodInfo> methods)
    {
        foreach (var afterMethod in methods)
        {
            RunInstanceMethodWithEmptyArgs(instance, afterMethod);
        }
    }

    private void RunStaticMethodsWithEmptyArgs(TypeInfo typeInfo, Type methodAttributeType) //TODO() type -> attribyte
    {
        var staticMethods = GetMethodsWithAttribute(methodAttributeType, typeInfo);

        foreach (var method in staticMethods)
        {
            method.Invoke(null, emptyArgs);
        }
    }

    private List<MethodInfo> GetMethodsWithAttribute(Type attributeType, TypeInfo typeInfo)
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