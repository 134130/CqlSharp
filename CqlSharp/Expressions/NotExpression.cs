using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions;

public class NotExpression : IExpression
{
    public IExpression Expression { get; set; }

    public IExpression GetOptimizedExpression()
    {
        Expression = Expression.GetOptimizedExpression() ?? Expression;
        return this;
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        var result = Expression.Calculate(columns, row);

        if (result is not BooleanLiteral booleanLiteral)
            throw new InvalidOperationException();

        return !booleanLiteral;
    }
}