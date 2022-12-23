using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions.Predicate.Operations;

internal abstract class PredicateOperation : ISql
{
    public abstract BooleanLiteral Calculate(Literal literal);

    public abstract string GetSql();
}