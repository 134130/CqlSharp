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

        return new AndOrExpression(optimizedLeft, Type, optimizedRight);
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