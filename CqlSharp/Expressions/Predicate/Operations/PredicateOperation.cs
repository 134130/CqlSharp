using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions.Predicate.Operations;

internal abstract class PredicateOperation : ISql
{
    public abstract BooleanLiteral Calculate(Literal literal);

    public abstract string GetSql();
}