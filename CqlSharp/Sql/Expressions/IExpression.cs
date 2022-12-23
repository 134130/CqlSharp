using CqlSharp.Sql.Expressions.Columns;
using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions;

internal interface IExpression : ISql
{
    public Literal Calculate(QualifiedIdentifier[] columns, string[] row);

    public IExpression GetOptimizedExpression();
}