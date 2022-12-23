using CqlSharp.Sql.Expressions.Columns;

namespace CqlSharp.Sql.Tables;

internal class Table : ITable
{
    public string? Alias { get; set; }

    public QualifiedIdentifier[] Columns { get; }

    public IEnumerable<string[]> Rows { get; }

    public int RowSize => _rowSize.Value;

    private Lazy<int> _rowSize;

    public Table(IEnumerable<string> columns, IEnumerable<string[]> rows, string? alias = null) :
        this(columns.Select(x => new QualifiedIdentifier(x)), rows, alias)
    {
    }

    public Table(IEnumerable<IColumn> columns, IEnumerable<string[]> rows, string? alias = null)
    {
        Columns = columns.Select(x =>
        {
            if (x is QualifiedIdentifier qualifiedIdentifier)
                return qualifiedIdentifier;

            if (x is ExpressionColumn expressionColumn)
            {
                if (expressionColumn.Alias is not null)
                    return new QualifiedIdentifier(expressionColumn.Alias);

                if (expressionColumn.Expression is QualifiedIdentifier qi)
                    return qi;

                return new QualifiedIdentifier(expressionColumn.Expression.GetSql());
            }

            if (x is CountColumn countColumn)
                return new QualifiedIdentifier(countColumn.GetSql());

            throw new ArgumentOutOfRangeException($"Unexpected column type: {x.GetType().Name}");
        }).ToArray();
        Rows = rows;
        Alias = alias;
        _rowSize = new Lazy<int>(() => Rows.Count());
    }
}