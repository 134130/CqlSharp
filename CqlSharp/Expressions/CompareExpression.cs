using CqlSharp.Expressions.Literals;
using CqlSharp.Expressions.Predicate;

namespace CqlSharp.Expressions;

public class CompareExpression : IExpression
{
    public IExpression Left { get; set; }

    public Literal? RightLiteral { get; set; }

    public QualifiedIdentifier? RightIdentifier { get; set; }

    public PredicateExpression? RightPredicateExpression { get; set; }

    public CompareOperator CompareOperator { get; set; }

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

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        var left = Left.Calculate(columns, row);
        var right = RightLiteral ?? RightIdentifier?.Calculate(columns, row) ??
            RightPredicateExpression?.Calculate(columns, row);

        if (left is NumberLiteral numLeft && right is NumberLiteral numRight)
            return new BooleanLiteral(Operate(numLeft.Value, numRight.Value));

        if (left is TextLiteral textLeft && right is TextLiteral textRight)
            return new BooleanLiteral(Operate(textLeft.Value, textRight.Value));

        throw new InvalidOperationException();
    }

    public IExpression GetOptimizedExpression()
    {
        return this;
    }
}

public enum CompareOperator
{
    Equal,
    NotEqual,
    GreaterOrEqual,
    GreaterThan,
    LessOrEqual,
    LessThan
}