using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions;

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
        var left = (BooleanLiteral)Left.Calculate(columns, row);
        var right = new Lazy<BooleanLiteral>(() => (BooleanLiteral)Right.Calculate(columns, row));

        //TODO
        return Type switch
        {
            AndOrType.And => new BooleanLiteral
            {
                Value = left.Value && right.Value.Value
            },
            AndOrType.Or => new BooleanLiteral
            {
                Value = left.Value || right.Value.Value
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}