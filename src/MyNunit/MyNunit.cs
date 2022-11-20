namespace MyNunit;

using System.Diagnostics;
using System.Reflection;
using Attributes;

public class MyNunit
{
    private static readonly LinkedList<string> loadedPaths = new ();
    private static readonly Stopwatch stopwatch = new ();

    public (IEnumerable<(long, string)>, IEnumerable<string>) RunTests(string pathToAssembly)
    {
        var assembly = Assembly.LoadFrom(pathToAssembly);
        loadedPaths.AddLast(pathToAssembly);

        foreach (var type in assembly.ExportedTypes)
        {
            var typeInfo = type.GetTypeInfo();
            RunBeforeClass(typeInfo);

            foreach (var method in typeInfo.GetMethods())
            {
                var instance = Activator.CreateInstance(typeInfo);

                var beforeTest = GetMethodsWithAttribute(BeforTestAttribute, type);
                var afterTest = GetMethodsWithAttribute(AfterTestAttribute, type);

                if (IsTestMethod(method) && instance != null)
                {
                    RunBefore(typeInfo);

                    RunTestMethod(instance, method);

                    RunAfter(typeInfo);
                }
            }

            RunAfterClass(typeInfo);
        }
    }

    private bool IsTestMethod(MethodInfo methodInfo)
    {
        foreach (var att in methodInfo.CustomAttributes)
        {
            if (att.AttributeType == typeof(TestAttribute))
            {
                return true;
            }
        }

        return false;
    }

    private void RunTestMethod(object instance, MethodInfo method)
    {

    }

    private void RunBefore(TypeInfo type)
    {

    }

    private void RunAfter(TypeInfo type)
    {

    }

    private void RunBeforeClass(TypeInfo type)
    {
    }

    private void RunAfterClass(TypeInfo type)
    {
    }

    private IEnumerable<MethodInfo> GetMethodsWithAttribute(CustomAttributeData attribute, TypeInfo typeInfo)
    {
        var compatibleMethods = new LinkedList<MethodInfo>();

        foreach (var method in typeInfo.DeclaredMethods)
        {
            if (method.CustomAttributes.Contains(attribute))
            {
                compatibleMethods.AddLast(method);
            }
        }

        return compatibleMethods;
    }
}