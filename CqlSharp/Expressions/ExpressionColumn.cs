using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions;

internal class ExpressionColumn : IColumn
{
    public IExpression Expression { get; }

    public string? Alias { get; }

    public ExpressionColumn(IExpression expression)
    {
        Expression = expression;
    }

    public ExpressionColumn(IExpression expression, string? alias)
    {
        Expression = expression;
        Alias = alias;
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        throw new NotImplementedException();
    }

    public IExpression GetOptimizedExpression()
    {
        // TODO
        return this;
    }

    public string GetSql()
    {
        if (Alias is null)
            return Expression.GetSql();

        return $"{Expression.GetSql()} AS {Alias}";
    }

    public override string ToString()
    {
        if (Expression is QualifiedIdentifier qualifiedIdentifier)
            return qualifiedIdentifier.ToString();

        return Expression.ToString();
    }
}