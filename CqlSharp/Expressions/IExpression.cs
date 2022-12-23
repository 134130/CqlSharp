using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions;

public interface IExpression : ISql
{
    public Literal Calculate(QualifiedIdentifier[] columns, string[] row);

    public IExpression GetOptimizedExpression();
}