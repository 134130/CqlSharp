using System.Text.RegularExpressions;
using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions.Predicate.Operations;

internal sealed class PredicateLikeOperation : PredicateOperation
{
    private Regex _regex;

    public PredicateLikeOperation(string pattern)
    {
        // TODO: Replace escaping
        _regex = new Regex($"^{pattern.Replace("%", ".+")}$");
    }

    public override BooleanLiteral Calculate(Literal literal)
    {
        if (literal is not TextLiteral textLiteral)
            throw new InvalidOperationException();

        return new BooleanLiteral(_regex.IsMatch(textLiteral.Value));
    }
}