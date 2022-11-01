namespace MatrixMultiplication.Writers;

using System.Collections.Generic;
using System.Text;

/// <summary>
/// Class that builds tables in string format.
/// Wraps over StringBuilder.
/// </summary>
public class StringTableBuilder
{
    /// <summary>
    /// StringBuilder storing a table as a string.
    /// </summary>
    private readonly StringBuilder stringBuilder;

    /// <summary>
    /// Table cell length.
    /// </summary>
    private readonly int cellLength;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="StringTableBuilder"/> class.
    /// </summary>
    /// <param name="cellLength">The result length of cell.</param>
    public StringTableBuilder(int cellLength)
    {
        this.stringBuilder = new StringBuilder();
        this.cellLength = cellLength;
    }
    
    /// <summary>
    /// Method for adding a bound to a table.
    /// </summary>
    /// <param name="length">Length of bound</param>
    /// <param name="messageDelta">Increase for name cell</param>
    public void WriteBound(int length, int messageDelta = 0)
    {
        this.stringBuilder.AppendFormat("+{0}", new string('-', this.cellLength + messageDelta));

        for (var i = 1; i < length; i++)
        {
            this.stringBuilder.AppendFormat("+{0}", new string('-', this.cellLength));
        }

        this.stringBuilder.Append("+");
        this.stringBuilder.AppendLine();
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
        this.stringBuilder.AppendFormat("|{0}", GetAlignString(name, this.cellLength + messageDelta));
        
        foreach (var element in elements)
        {
            this.stringBuilder.AppendFormat("|{0}", GetAlignString(element?.ToString() ?? string.Empty, this.cellLength));
        }

        this.stringBuilder.Append("|");
        this.stringBuilder.AppendLine();
    }
    
    /// <summary>
    /// Method returning a string representation of a table.
    /// </summary>
    /// <returns>String representation of a table.</returns>
    public override string ToString()
        => this.stringBuilder.ToString();

    /// <summary>
    /// Gives aligned string on the right and left.
    /// </summary>
    /// <param name="stringElement">A string value written to an aligned string.</param>
    /// <param name="cellLengthToAlign">Length of cell</param>
    /// <returns>Aligned string</returns>
    private static string GetAlignString(string stringElement, int cellLengthToAlign)
    {
        if (stringElement.Length >= cellLengthToAlign)
        {
            return stringElement;
        }
        
        var leftPadding = (cellLengthToAlign - stringElement.Length) / 2;
        var rightPadding = cellLengthToAlign - stringElement.Length - leftPadding;
        
        return new string(' ', leftPadding) + stringElement + new string(' ', rightPadding);
    }
}
