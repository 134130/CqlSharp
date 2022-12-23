using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions.Predicate.Operations;

internal sealed class PredicateBetweenOperation : PredicateOperation
{
    private string _startLiteral;
    private string _endLiteral;

    public PredicateBetweenOperation(string start, string end)
    {
        _startLiteral = start;
        _endLiteral = end;
    }

    public override BooleanLiteral Calculate(Literal literal)
    {
        if (literal is not TextLiteral textLiteral)
            throw new InvalidOperationException();

        return new BooleanLiteral(
            string.CompareOrdinal(_startLiteral, textLiteral.Value) <= 0 &&
            string.CompareOrdinal(textLiteral.Value, _endLiteral) <= 0);
    }

    public override string GetSql()
    {
        return $"{_startLiteral} BETWEEN {_endLiteral}";
    }
}