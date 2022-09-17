namespace MatrixMul;

public static class IntExtension
{
    public static int IntPow(this int number, int powValue)
        => (int) Math.Pow((double)number, (double)powValue);
}
