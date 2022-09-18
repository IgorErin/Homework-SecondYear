using System.Text;

namespace MatrixMul.Writers;

/// <summary>
/// Class that builds tables in string format.
/// Wraps over StringBuilder.
/// </summary>
public class StringTableBuilder
{
    /// <summary>
    /// StringBuilder storing a table as a string.
    /// </summary>
    private readonly StringBuilder _stringBuilder;

    /// <summary>
    /// Table cell lengt.
    /// </summary>
    private readonly int _cellLength;

    /// <summary>
    /// Constructor initializing StringBuilder.
    /// </summary>
    public StringTableBuilder(int cellLength)
    {
        _stringBuilder = new StringBuilder();
        _cellLength = cellLength;
    }
    
    /// <summary>
    /// Method for adding a bound to a table.
    /// </summary>
    /// <param name="length">Length of bound</param>
    /// <param name="messageDelta">Increase for name cell</param>
    public void WriteBound(int length, int messageDelta = 0)
    {
        _stringBuilder.AppendFormat("+{0}", new String('-', _cellLength + messageDelta));

        for (var i = 1; i < length; i++)
        {
            _stringBuilder.AppendFormat("+{0}", new String('-', _cellLength));
        }

        _stringBuilder.Append("+");
        _stringBuilder.AppendLine();
    }
    
    /// <summary>
    /// Method for adding rows to a table.
    /// </summary>
    /// <param name="elements">Elements of the row to be added.</param>
    /// <param name="name">The string to be written in the first cell.</param>
    /// <param name="messageDelta">Changing the cell length of a name cell</param>
    /// <typeparam name="T">Elements row type</typeparam>
    public void WriteRow<T>(IEnumerable<T> elements, string name, int messageDelta = 0)
    {
        _stringBuilder.AppendFormat("|{0}", GetAlignString(name ?? "", _cellLength + messageDelta));
        
        foreach(var element in elements)
        {
            _stringBuilder.AppendFormat("|{0}", GetAlignString(element?.ToString() ?? "", _cellLength));
        }

        _stringBuilder.Append("|");
        _stringBuilder.AppendLine();
    }

    /// <summary>
    /// Gives aligned string on the right and left.
    /// </summary>
    /// <param name="stringElement">A string value written to an aligned string.</param>
    /// <param name="cellLengthToAlign">Length of cell</param>
    /// <returns>Aligned string</returns>
    private string GetAlignString(string stringElement, int cellLengthToAlign)
    {
        if (stringElement.Length >= cellLengthToAlign)
        {
            return stringElement;
        }
        
        var leftPadding = (cellLengthToAlign - stringElement.Length) / 2;
        var rightPadding = cellLengthToAlign - stringElement.Length - leftPadding;
        
        return new string(' ', leftPadding) + stringElement + new string(' ', rightPadding);
    }

    /// <summary>
    /// Method returning a string representation of a table.
    /// </summary>
    /// <returns>String representation of a table.</returns>
    public override string ToString()
        => _stringBuilder.ToString();
}
