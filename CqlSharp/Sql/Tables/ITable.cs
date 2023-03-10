using CqlSharp.Sql.Expressions.Columns;

namespace CqlSharp.Sql.Tables;

internal interface ITable
{
    public string? Alias { get; }
    public QualifiedIdentifier[] Columns { get; }

    public virtual int IndexOfColumn(QualifiedIdentifier target)
    {
        var targetName = target[0] == Alias ? string.Join(".", target[1..]) : target.Name;

        foreach (var (x, i) in Columns.Select((x, i) => (x, i)))
        {
            if (x.Name == targetName)
                return i;
        }

        return -1;
    }
}