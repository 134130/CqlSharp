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
        throw new NotImplementedException();
    }

    public IExpression GetOptimizedExpression()
    {
        return this;
    }
}