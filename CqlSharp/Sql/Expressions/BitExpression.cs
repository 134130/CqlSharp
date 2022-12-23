using CqlSharp.Exceptions;
using CqlSharp.Sql.Expressions.Columns;
using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions;

internal class BitExpression : IExpression
{
    public IExpression Left { get; }

    public BitOperator Operator { get; }

    public IExpression Right { get; }

    public BitExpression(IExpression left, BitOperator @operator, IExpression right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }

    public string GetSql()
    {
        var operatorString = Operator switch
        {
            BitOperator.Plus => "+",
            BitOperator.Minus => "-",
            _ => throw new ArgumentOutOfRangeException()
        };

        return $"{Left.GetSql()} {operatorString} {Right.GetSql()}";
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        var left = Left.Calculate(columns, row);
        var right = Right.Calculate(columns, row);

        if (left is NumberLiteral numLeft && right is NumberLiteral numRight)
            return Operate(numLeft, numRight);

        if (left is TextLiteral textLeft && right is TextLiteral textRight)
            return Operate(textLeft, textRight);

        if (left is BooleanLiteral && right is BooleanLiteral)
            throw new CqlExpressionException($"Can not operate between boolean types: '{GetSql()}'");

        throw new CqlExpressionException($"Can not operate between different types: '{GetSql()}'");
    }

    private TextLiteral Operate(TextLiteral left, TextLiteral right)
    {
        return Operator switch
        {
            BitOperator.Plus => new TextLiteral(left.Value + right.Value),
            BitOperator.Minus => throw new CqlExpressionException(
                $"Can not operate '-' between string types: '{GetSql()}'"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private NumberLiteral Operate(NumberLiteral left, NumberLiteral right)
    {
        return Operator switch
        {
            BitOperator.Plus => left + right,
            BitOperator.Minus => left - right,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public IExpression GetOptimizedExpression()
    {
        var left = Left.GetOptimizedExpression();
        var right = Right.GetOptimizedExpression();

        if (left is Literal && right is Literal)
            return Calculate(null, null);

        return new BitExpression(left, Operator, right);
    }
}