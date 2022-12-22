using CqlSharp.Expressions;

namespace CqlSharp.Query;

public class Select
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
}