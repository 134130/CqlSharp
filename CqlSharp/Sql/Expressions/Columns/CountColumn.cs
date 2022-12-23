using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions.Columns;

public class CountColumn : IColumn
{
    public string GetSql()
    {
        return "COUNT(*)";
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        throw new InvalidOperationException("This method must not be reached");
    }

    public IExpression GetOptimizedExpression()
    {
        return this;
    }
}