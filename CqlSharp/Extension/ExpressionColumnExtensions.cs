using CqlSharp.Sql.Expressions;

namespace CqlSharp.Extension;

internal static class ExpressionColumnExtensions
{
    public static int IndexOf(this IEnumerable<QualifiedIdentifier> columns, QualifiedIdentifier column)
    {
        foreach (var (x, i) in columns.Select((x, i) => (x, i)))
        {
            if (x.Length != column.Length)
                continue;

            if (x.Identifiers.SequenceEqual(column.Identifiers))
                return i;
        }

        return -1;
    }
}