using System.Reflection;

Console.WriteLine("Hello, World!");

//D:\Projects\Homework-SecondYear\src\MyNunit\bin\Debug\net6.0\MyNunit.exe

Assembly.LoadFrom("D:\\Projects\\Homework-SecondYear\\src\\TestAssembly\\bin\\Debug\\net6.0\\nunit.framework.dll");
var assembly = Assembly.LoadFrom("D:\\Projects\\Homework-SecondYear\\src\\TestAssembly\\bin\\Debug\\net6.0\\TestAssembly.dll");

bool IsTestMethod(MethodInfo methodInfo)
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

foreach (var type in assembly.ExportedTypes)
{
    foreach (var method in type.GetTypeInfo().DeclaredMethods)
    {

        method.GetParameters();

        if (IsTestMethod(method))
        {
            var obj = System.Activator.CreateInstance(type);

            method.Invoke(obj, null);
        }
    }
}




