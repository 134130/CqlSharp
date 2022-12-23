using CqlSharp.Sql.Expressions.Columns;

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
    }
}