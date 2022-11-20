Console.WriteLine("Hello, World!");

var nunit = new MyNunit.MyNunit();

var assemblyTest = nunit.RunTests("D:\\Projects\\Homework-SecondYear\\src\\TestAssembly\\bin\\Debug\\net6.0\\TestAssembly.dll");

Console.WriteLine(assemblyTest.ToString());
