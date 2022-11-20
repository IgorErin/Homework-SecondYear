using System.Reflection;

Console.WriteLine("Hello, World!");

//D:\Projects\Homework-SecondYear\src\MyNunit\bin\Debug\net6.0\MyNunit.exe

Assembly.LoadFrom("D:\\Projects\\Homework-SecondYear\\src\\TestAssembly\\bin\\Debug\\net6.0\\nunit.framework.dll");
var assembly = Assembly.LoadFrom("D:\\Projects\\Homework-SecondYear\\src\\TestAssembly\\bin\\Debug\\net6.0\\TestAssembly.dll");

var nunit = new MyNunit.MyNunit();

nunit.RunTests("D:\\Projects\\Homework-SecondYear\\src\\TestAssembly\\bin\\Debug\\net6.0\\TestAssembly.dll");




