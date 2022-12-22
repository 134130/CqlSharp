using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions;

public interface IExpression
{
    public Literal Calculate(QualifiedIdentifier[] columns, string[] row);
    public IExpression GetOptimizedExpression();
}