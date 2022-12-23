using System.Text;
using CqlSharp.Sql.Expressions;

namespace CqlSharp.Sql.Query;

public class Select : ISql
{
    public TableReference? From { get; set; }

    // Order
    public List<OrderBy>? OrderBys { get; set; }

    // Limit
    public int Offset { get; set; }

    public int Limit { get; set; }

    // Where
    public IExpression? WhereExpression { get; set; }

    // Columns
    public List<IColumn> Columns { get; set; }

    public string GetSql()
    {
        var sb = new StringBuilder();
        sb.Append("SELECT ")
            .Append(string.Join(", ", Columns.Select(x => x.GetSql())));

        if (From is not null)
            sb.Append(" FROM ").Append(From.GetSql());

        if (WhereExpression is not null)
            sb.Append(" WHERE ").Append(WhereExpression.GetSql());

        if (OrderBys is not null)
            sb.Append(" ORDER BY ").Append(string.Join(", ", OrderBys.Select(x => x.GetSql())));

        if (Limit > 0)
            sb.Append($" LIMIT {Limit}");

        if (Offset > 0)
            sb.Append($" OFFSET {Offset}");

        return sb.ToString();
    }
}