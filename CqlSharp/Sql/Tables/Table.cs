using CqlSharp.Sql.Expressions;

namespace CqlSharp.Sql.Tables;

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

            throw new ArgumentOutOfRangeException($"Unexpected column type: {x.GetType().Name}");
        }).ToArray();
        Rows = rows;
        Alias = alias;
    }
}