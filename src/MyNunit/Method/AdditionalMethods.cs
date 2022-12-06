using System.Reflection;

namespace MyNunit.Method;

public class AdditionalMethods
{
    private readonly IEnumerable<MethodInfo> methodInfos;

    public AdditionalMethods(IEnumerable<MethodInfo> methodInfos)
    {
        this.methodInfos = methodInfos;
    }

    public AdditionalStatus Run()
    {
        
    }
}