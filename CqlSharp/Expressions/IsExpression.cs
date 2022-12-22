using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions;

public class IsExpression : IExpression
{
    public IExpression Expression { get; set; }

    public IsType Type { get; set; }

    public IsExpression(IExpression expression, IsType type)
    {
        Expression = expression;
        Type = type;
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        var literal = Expression.Calculate(columns, row);

        if (literal is not BooleanLiteral booleanLiteral)
            throw new InvalidOperationException();

        switch (Type)
        {
            case IsType.IsTrue:
                return booleanLiteral;
            case IsType.IsFalse:
                return !booleanLiteral;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public IExpression GetOptimizedExpression()
    {
        Expression = Expression.GetOptimizedExpression();
        return this;
    }
}