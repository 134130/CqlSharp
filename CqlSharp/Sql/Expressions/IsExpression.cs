using CqlSharp.Exceptions;
using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions;

internal class IsExpression : IExpression
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
        var result = Expression.Calculate(columns, row);

        if (result is not BooleanLiteral booleanLiteral)
            throw new CqlExpressionException($"Result must be {nameof(BooleanLiteral)}. but {result.GetType().Name}");

        switch (Type)
        {
            case IsType.IsTrue:
            case IsType.IsNotFalse:
                return booleanLiteral;
            case IsType.IsFalse:
            case IsType.IsNotTrue:
                return !booleanLiteral;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public IExpression GetOptimizedExpression()
    {
        var optimizedExpression = Expression.GetOptimizedExpression();
        return new IsExpression(optimizedExpression, Type);
    }

    public string GetSql()
    {
        var isTypeString = Type switch
        {
            IsType.IsTrue => "IS TRUE",
            IsType.IsFalse => "IS FALSE",
            IsType.IsNotTrue => "IS NOT TRUE",
            IsType.IsNotFalse => "IS NOT FALSE",
            _ => throw new ArgumentOutOfRangeException()
        };

        return $"{Expression.GetSql()} {isTypeString}";
    }
}