namespace CqlSharp.Sql.Expressions.Literals;

public abstract class Literal : IExpression
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