

namespace MatrixMul;

public class MatrixMain
{
    public static void Main()
    {
        int[,] lol = { { 1, 2 }, { 1, 2 } };

        var left = new IntSequentialMatrix(lol);
        var right = new IntSequentialMatrix(lol);

        var newLeft = new IntParallelMatrix(lol);
        var newRight = new IntParallelMatrix(lol);

        var newLol = left * right;
        var newnewLol = newLeft * newRight;

        Console.WriteLine(newLol.ToString());
        Console.WriteLine(newnewLol.ToString());
    }
}