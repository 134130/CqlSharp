using CqlSharp.Exceptions;
using CqlSharp.Sql.Expressions.Columns;
using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions;

internal class AndOrExpression : IExpression
{
    public AndOrType Type { get; }
    public IExpression Left { get; }
    public IExpression Right { get; }

    public AndOrExpression(IExpression left, AndOrType type, IExpression right)
    {
        Left = left;
        Type = type;
        Right = right;
    }

    public IExpression GetOptimizedExpression()
    {
        var optimizedLeft = Left.GetOptimizedExpression();
        var optimizedRight = Right.GetOptimizedExpression();

        switch (Type)
        {
            case AndOrType.And when optimizedLeft is BooleanLiteral { Value: false } ||
                                    optimizedRight is BooleanLiteral { Value: false }:
                return BooleanLiteral.False;

            case AndOrType.Or when optimizedLeft is BooleanLiteral { Value: true } ||
                                   optimizedRight is BooleanLiteral { Value: true }:
                return BooleanLiteral.True;
        }

        var optimized = new AndOrExpression(optimizedLeft, Type, optimizedRight);

        return optimized;
    }

    public string GetSql()
    {
        var typeString = Type switch
        {
            AndOrType.And => "AND",
            AndOrType.Or => "OR",
            _ => throw new ArgumentOutOfRangeException()
        };

        return $"{Left.GetSql()} {typeString} {Right.GetSql()}";
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        var left = Left.Calculate(columns, row);
        if (left is not BooleanLiteral boolLeft)
            throw new CqlExpressionException($"Left must be boolean type: {left.GetType().Name}: {GetSql()}");

        if (Type is AndOrType.Or && boolLeft.Value)
            return BooleanLiteral.True;

        if (Type is AndOrType.And && !boolLeft.Value)
            return BooleanLiteral.False;

        var right = Right.Calculate(columns, row);

        if (right is not BooleanLiteral)
            throw new CqlExpressionException($"Right must be boolean type: {right.GetType().Name}: {GetSql()}");

        return right;
    }
}