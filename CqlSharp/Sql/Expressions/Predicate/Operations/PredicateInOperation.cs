using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions.Predicate.Operations;

internal sealed class PredicateInOperation : PredicateOperation
{
    private IEnumerable<string> _literals;

    public PredicateInOperation(IEnumerable<string> literals)
    {
        _literals = literals;
    }

    public override BooleanLiteral Calculate(Literal literal)
    {
        if (literal is not TextLiteral textLiteral)
            throw new InvalidOperationException();

        return new BooleanLiteral(_literals.Any(x => textLiteral.Value == x));
    }

    public override string GetSql()
    {
        return $"IN ({string.Join(", ", _literals)})";
    }
}