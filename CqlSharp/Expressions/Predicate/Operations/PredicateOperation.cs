using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions.Predicate;

public abstract class PredicateOperation
{
    public abstract BooleanLiteral Calculate(Literal literal);
}