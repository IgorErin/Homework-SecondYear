using System.Data;
using System.Text;

namespace MatrixMul;

public class ResultTableWriter
{

    private const int MaxLength = 100;
    
    public static string WriteTableToFile(double[] parTimes, double[] seqTimes, int[] itemCount)
    {
        var stringBuilder = new StringBuilder();
        
        IEnumerable<Tuple<int, string, string>> authors =
            new[]
            {
                Tuple.Create(1, "Isaac", "Asimov"),
                Tuple.Create(2, "Robert", "Heinlein"),
                Tuple.Create(3, "Frank", "Herbert"),
                Tuple.Create(4, "Aldous", "Huxley"),
            };
        
        for (var genIndex = 0; genIndex < )
    }
}