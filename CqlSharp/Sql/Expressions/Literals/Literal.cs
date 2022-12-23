using CqlSharp.Sql.Expressions.Columns;

namespace CqlSharp.Sql.Expressions.Literals;

internal abstract class Literal : IExpression
{
    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        return this;
    }

    public IExpression GetOptimizedExpression()
    {
        return this;
    }

    public abstract string GetSql();
}