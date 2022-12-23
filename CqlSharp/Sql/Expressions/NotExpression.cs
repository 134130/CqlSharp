using CqlSharp.Exceptions;
using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions;

internal class NotExpression : IExpression
{
    public IExpression Expression { get; }

    public NotExpression(IExpression expression)
    {
        Expression = expression;
    }

    public IExpression GetOptimizedExpression()
    {
        var optimizedExpression = Expression.GetOptimizedExpression();
        var optimized = new NotExpression(optimizedExpression);

        if (optimizedExpression is Literal)
            return optimized.Calculate(null, null);

        return optimized;
    }

    public string GetSql()
    {
        return $"NOT {Expression.GetSql()}";
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        var result = Expression.Calculate(columns, row);

        if (result is not BooleanLiteral booleanLiteral)
            throw new CqlExpressionException($"Result must be {nameof(BooleanLiteral)}. but {result.GetType().Name}");

        return !booleanLiteral;
    }
}