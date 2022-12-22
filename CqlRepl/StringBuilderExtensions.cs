using System.Text;

namespace CqlRepl;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendSeperater(this StringBuilder sb, IEnumerable<int> maxLengthOfColumns)
    {
        sb.Append("+");
        foreach (var lengthOfColumn in maxLengthOfColumns)
        {
            sb.Append(new string('-', lengthOfColumn + 2));
            sb.Append("+");
        }

        return sb.AppendLine();
    }
}