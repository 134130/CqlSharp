using System.Linq.Expressions;
using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions;

public interface IColumn : IExpression
{
}

public class ExpressionColumn : IColumn
{
    public IExpression Expression { get; }

    public string? Alias { get; }

    public ExpressionColumn(IExpression expression)
    {
        Expression = expression;
    }

    public ExpressionColumn(IExpression expression, string alias)
    {
        Expression = expression;
        Alias = alias;
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        throw new NotImplementedException();
    }

    public IExpression GetOptimizedExpression()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        if (Expression is QualifiedIdentifier qualifiedIdentifier)
            return qualifiedIdentifier.ToString();

        return Expression.ToString();
    }
}