using CqlSharp.Expressions;

namespace CqlSharp;

public class Table : ITable
{
    public string? Alias { get; set; }

    public QualifiedIdentifier[] Columns { get; }

    public IEnumerable<string[]> Rows { get; }

    public Table(IEnumerable<IColumn> columns, IEnumerable<string[]> rows, string? alias = null)
    {
        Columns = columns.Select(x =>
        {
            if (x is QualifiedIdentifier qualifiedIdentifier)
                return qualifiedIdentifier;

            if (x is ExpressionColumn { Alias: not null } aliased)
                return new QualifiedIdentifier(aliased.Alias);

            if (x is ExpressionColumn { Expression: QualifiedIdentifier qi })
                return qi;

            throw new NotImplementedException();
        }).ToArray();
        Rows = rows;
        Alias = alias;
    }
}

public interface ITable
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