using CqlSharp.Expressions;

namespace CqlSharp.Extension;

internal static class ExpressionColumnExtensions
{
    public static int IndexOf(this IEnumerable<QualifiedIdentifier> columns, QualifiedIdentifier column)
    {
        foreach (var (x, i) in columns.Select((x, i) => (x, i)))
        {
            if (x.Name == column.Name)
                return i;
        }

        return -1;
    }

    public static int IndexOf(this IEnumerable<QualifiedIdentifier> columns, string column)
    {
        foreach (var (x, i) in columns.Select((x, i) => (x, i)))
        {
            if (x.Name == column)
                return i;
        }

        return -1;
    }
}