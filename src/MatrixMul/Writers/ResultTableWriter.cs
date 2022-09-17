using System.Data;
using System.Text;

namespace MatrixMul;

public static class ResultTableWriter
{
    public static string WriteBound(int length, int maxLength)
    {
        var stringBuilder = new StringBuilder();

        for (var i = 0; i < length; i++)
        {
            stringBuilder.AppendFormat("+{0}", new String('-', maxLength));
        }

        stringBuilder.Append("+");
        stringBuilder.AppendLine();
        
        return stringBuilder.ToString();
    }
    
    public static string WriteRow<T>(IEnumerable<T> elements, int maxLength, string name)
    {
        var stringBuilder = new StringBuilder();
        
        stringBuilder.AppendFormat("|{0}", GetAlignString(name ?? "", maxLength));
        
        foreach(var element in elements)
        {
            stringBuilder.AppendFormat("|{0}", GetAlignString(element?.ToString() ?? "", maxLength)); // TODO
        }

        stringBuilder.Append("|");
        stringBuilder.AppendLine();

        return stringBuilder.ToString();
    }

    public static string GetAlignString(string stringElement, int maxLength)
    {
        if (stringElement.Length >= maxLength)
        {
            return stringElement;
        }
        
        var leftPadding = (maxLength - stringElement.Length) / 2;
        var rightPadding = maxLength - stringElement.Length - leftPadding;
        
        return new string(' ', leftPadding) + stringElement + new string(' ', rightPadding);
    }
}