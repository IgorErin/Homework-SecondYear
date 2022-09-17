using System.Text;

namespace MatrixMul.Writers;

public static class ResultTableWriter
{
    public static string WriteBound(int length, int maxLength, int messageDelta = 0)
    {
        var stringBuilder = new StringBuilder();
        
        stringBuilder.AppendFormat("+{0}", new String('-', maxLength + messageDelta));

        for (var i = 1; i < length; i++)
        {
            stringBuilder.AppendFormat("+{0}", new String('-', maxLength));
        }

        stringBuilder.Append("+");
        stringBuilder.AppendLine();
        
        return stringBuilder.ToString();
    }
    
    public static string WriteRow<T>(IEnumerable<T> elements, int maxLength, string name, int messageDelta = 0)
    {
        var stringBuilder = new StringBuilder();
        
        stringBuilder.AppendFormat("|{0}", GetAlignString(name ?? "", maxLength + messageDelta));
        
        foreach(var element in elements)
        {
            stringBuilder.AppendFormat("|{0}", GetAlignString(element?.ToString() ?? "", maxLength));
        }

        stringBuilder.Append("|");
        stringBuilder.AppendLine();

        return stringBuilder.ToString();
    }

    private static string GetAlignString(string stringElement, int maxLength)
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