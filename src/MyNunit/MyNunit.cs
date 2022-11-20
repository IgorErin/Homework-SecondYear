namespace MyNunit;

using System.Diagnostics;
using System.Reflection;
using Attributes;
using Exceptions;
using Optional;
using Optional.Unsafe;

public class MyNunit
{
    private static readonly Stopwatch stopwatch = new ();
    private static readonly object[] emptyArgs = Array.Empty<object>();

    public (IEnumerable<(long, string)>, IEnumerable<string>) RunTests(string pathToAssembly)
    {
        var assembly = Assembly.LoadFrom(pathToAssembly);

        foreach (var type in assembly.ExportedTypes)
        {
            var typeInfo = type.GetTypeInfo();

            try
            {
                RunStaticMethodsWithEmptyArgs(typeInfo, typeof(BeforeClassAttribute));

                var instance = Activator.CreateInstance(typeInfo) ??
                               throw new Exception("message that indicate that class is not instanced");

                var beforeTestMethods = GetMethodsWithAttribute(typeof(BeforeAttribute), typeInfo);
                var afterTestMethods = GetMethodsWithAttribute(typeof(AfterAttribute), typeInfo);

                var testMethods = GetMethodsWithAttribute(typeof(TestAttribute), typeInfo);
                var results = new TestInfo[testMethods.Count];

                for (var i = 0; i < testMethods.Count; i++)
                {
                    var method = testMethods[i];

                    if (IsMethodHasAttribute(method, typeof(TestAttribute)))
                    {
                        Exception resultException = new SuccessException();
                        try
                        {
                            foreach (var beforeMethod in beforeTestMethods)
                            {
                                RunInstanceMethodWithEmptyArgs(instance, beforeMethod);
                            }

                            stopwatch.Reset();
                            stopwatch.Start();
                            RunInstanceMethodWithEmptyArgs(instance, method);
                            stopwatch.Stop();

                            foreach (var afterMethod in afterTestMethods)
                            {
                                RunInstanceMethodWithEmptyArgs(instance, afterMethod);
                            }
                        }
                        catch (Exception e) //TODO()
                        {
                            resultException = e;
                        }


                        results[i] = new TestInfo(method.Name, stopwatch.ElapsedMilliseconds, resultException);
                    }
                }

                RunStaticMethodsWithEmptyArgs(typeInfo, typeof(AfterClassAttribute));
            }catch (Exception e) //TODO()
            {
            }
        }

        return null; //TODO()
    }

    private bool IsMethodHasAttribute(MethodInfo methodInfo, Type attributeType)
        => methodInfo.GetCustomAttributes(attributeType).Any();


    private void RunInstanceMethodWithEmptyArgs(object type, MethodInfo methodInfo)
        => methodInfo.Invoke(type, emptyArgs);


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

    private class TestInfo
    {
        private readonly string name;
        private readonly Exception exceptionResult;
        private readonly Option<Exception> expectedException;
        private readonly Option<string> ignoreMessage;
        private readonly long time;

        public string Name => this.name;

        public TestStatus Status
        {
            get
            {
                if (this.ignoreMessage.HasValue)
                {
                    return TestStatus.Ignored;
                }

                if (this.expectedException.HasValue
                    && this.exceptionResult.GetType() == this.expectedException.GetType())
                {
                    return TestStatus.Passed;
                }

                return TestStatus.Failed;
            }
        }

        public string Message
        {
            get
            {
                if (this.ignoreMessage.HasValue)
                {
                    return $"Ignore, {ignoreMessage.ValueOrFailure()}";
                }

                if (this.expectedException.HasValue)
                {
                    if (this.expectedException.ValueOrFailure().GetType() == exceptionResult.GetType())
                    {
                        return $"Passed with expected exception, message = {this.exceptionResult.Message}";
                    }

                    return
                        $"Failed with unexpected exception: {this.exceptionResult.GetType()}," +
                        $" message = {this.exceptionResult.Message}";
                }

                return $"Failed with exception: {this.exceptionResult.GetType()}, message = {this.exceptionResult.Message}";
            }
        }

        private TestInfo(
            string name,
            Exception exceptionResult,
            Option<Exception> expectedException,
            Option<string> ignoreReason,
            long time)
        {
            this.name = name;
            this.exceptionResult = exceptionResult;
            this.expectedException = expectedException;
            this.ignoreMessage = ignoreReason;
            this.time = time;
        }

        public TestInfo(string name, long time, Exception exceptionResult)
            : this (name, exceptionResult, Option.None<Exception>(), Option.None<string>(), time)
        {
            this.name = name;
            this.exceptionResult = exceptionResult;
        }

        public TestInfo(string name, long time, Exception exceptionResult, Exception expectedException)
            : this(name, exceptionResult, expectedException.Some(), Option.None<string>(), time)
        {
        }

        public TestInfo(string name, long time, Exception exceptionResult, string ignoreReason)
            : this(name, exceptionResult, Option.None<Exception>(), ignoreReason.Some(), time)
        {
        }
    }

    private enum TestStatus
    {
        Failed,
        Ignored,
        Passed
    }
}