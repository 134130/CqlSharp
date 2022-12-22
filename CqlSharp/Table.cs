using System.Formats.Asn1;
using CqlSharp.Expressions;

namespace CqlSharp;

public class Table : ITable
{
    public string? Alias { get; }

    public QualifiedIdentifier[] Columns { get; }

    public IEnumerable<string[]> Rows { get; }

    public Table(QualifiedIdentifier[] columns, IEnumerable<string[]> rows, string alias = null)
    {
        Columns = columns;
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