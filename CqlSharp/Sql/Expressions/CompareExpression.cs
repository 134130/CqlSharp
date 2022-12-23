using CqlSharp.Exceptions;
using CqlSharp.Sql.Expressions.Columns;
using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions;

internal class CompareExpression : IExpression
{
    public IExpression Left { get; }

    public IExpression Right { get; }

    public CompareOperator CompareOperator { get; }

    public CompareExpression(IExpression left, CompareOperator compareOperator, IExpression right)
    {
        Left = left;
        CompareOperator = compareOperator;
        Right = right;
    }

    public IExpression GetOptimizedExpression()
    {
        var left = Left.GetOptimizedExpression();
        var right = Right.GetOptimizedExpression();

        var optimized = new CompareExpression(left, CompareOperator, right);

        if (left is Literal && right is Literal)
            return optimized.Calculate(null, null);

        return optimized;
    }

    public string GetSql()
    {
        var compareOperatorString = CompareOperator switch
        {
            CompareOperator.Equal => "=",
            CompareOperator.NotEqual => "!=",
            CompareOperator.GreaterOrEqual => ">=",
            CompareOperator.GreaterThan => ">",
            CompareOperator.LessOrEqual => "<=",
            CompareOperator.LessThan => "<",
            _ => throw new ArgumentOutOfRangeException()
        };

        return $"{Left.GetSql()} {compareOperatorString} {Right.GetSql()}";
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        var left = Left.Calculate(columns, row);
        var right = Right.Calculate(columns, row);

        if (left is NumberLiteral numLeft && right is NumberLiteral numRight)
            return new BooleanLiteral(Operate(numLeft.Value, numRight.Value));

        if (left is TextLiteral textLeft && right is TextLiteral textRight)
            return new BooleanLiteral(Operate(textLeft.Value, textRight.Value));

        throw new CqlExpressionException(
            $"Cant compare. left: {left.GetType().Name}, right: {right.GetType().Name}");
    }

    private bool Operate(string left, string right)
    {
        return CompareOperator switch
        {
            CompareOperator.Equal => left == right,
            CompareOperator.NotEqual => left != right,
            CompareOperator.GreaterOrEqual => string.CompareOrdinal(left, right) <= 0,
            CompareOperator.GreaterThan => string.CompareOrdinal(left, right) < 0,
            CompareOperator.LessOrEqual => string.CompareOrdinal(left, right) >= 0,
            CompareOperator.LessThan => string.CompareOrdinal(left, right) > 0,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private bool Operate(int left, int right)
    {
        return CompareOperator switch
        {
            CompareOperator.Equal => left == right,
            CompareOperator.NotEqual => left != right,
            CompareOperator.GreaterOrEqual => left >= right,
            CompareOperator.GreaterThan => left > right,
            CompareOperator.LessOrEqual => left <= right,
            CompareOperator.LessThan => left < right,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}