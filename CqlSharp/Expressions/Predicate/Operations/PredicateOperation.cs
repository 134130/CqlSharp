using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions.Predicate.Operations;

internal abstract class PredicateOperation
{
    public abstract BooleanLiteral Calculate(Literal literal);
}