namespace TestAssembly;

public class SimpleClassWithoutAttributes
{
    private readonly int value = 4;
    private readonly int constructorValue;

    public int Value => value;

    public SimpleClassWithoutAttributes(int value)
    {
        this.constructorValue = value;
    }

    public void SomePublicMethod()
    {
    }

    private void somePrivateMethod()
    {
    }
}