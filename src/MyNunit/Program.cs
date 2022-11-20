var nunit = new MyNunit.MyNunit();

var assemblyTest =
    nunit.RunTestsFrom("D:\\Projects\\Homework-SecondYear\\src\\TestAssembly\\bin\\Debug\\net6.0\\TestAssembly.dll");

Console.WriteLine(assemblyTest.ToString());
