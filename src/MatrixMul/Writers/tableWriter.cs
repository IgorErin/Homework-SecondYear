using System.Text;

namespace MatrixMul.Writers;

public class TableWriter
{
    private readonly StringBuilder _stringBuilder;

    public TableWriter()
    {
        this._stringBuilder = new StringBuilder();
    }
    
    public string WriteBound(int length, int maxLength, int messageDelta = 0)
    {
        _stringBuilder.AppendFormat("+{0}", new String('-', maxLength + messageDelta));

        for (var i = 1; i < length; i++)
        {
            _stringBuilder.AppendFormat("+{0}", new String('-', maxLength));
        }

        _stringBuilder.Append("+");
        _stringBuilder.AppendLine();
        
        return _stringBuilder.ToString();
    }
    
    public string WriteRow<T>(IEnumerable<T> elements, int maxLength, string name, int messageDelta = 0)
    {
        _stringBuilder.AppendFormat("|{0}", GetAlignString(name ?? "", maxLength + messageDelta));
        
        foreach(var element in elements)
        {
            _stringBuilder.AppendFormat("|{0}", GetAlignString(element?.ToString() ?? "", maxLength));
        }

        _stringBuilder.Append("|");
        _stringBuilder.AppendLine();

        return _stringBuilder.ToString();
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

    public string ToString()
        => _stringBuilder.ToString();
}
