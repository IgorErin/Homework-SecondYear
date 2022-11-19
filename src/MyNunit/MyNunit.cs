namespace MyNunit;

using System.Reflection;

public class MyNunit
{
    public MyNunit()
    {
    }

    public (IEnumerable<(long, string)>, IEnumerable<string>) Test(string pathToAssembly)
    {
        var assembly = Assembly.LoadFrom(pathToAssembly);


    }



    private bool IsTestMethod(MethodInfo methodInfo)
    {
        foreach (var att in methodInfo.CustomAttributes)
        {
            if (att.AttributeType == typeof(NUnit.Framework.TestAttribute))
            {
                return true;
            }
        }

        return false;
    }
}