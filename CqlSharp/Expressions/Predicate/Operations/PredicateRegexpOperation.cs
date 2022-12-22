using System.Text.RegularExpressions;
using CqlSharp.Expressions.Literals;

namespace CqlSharp.Expressions.Predicate;

public sealed class PredicateRegexpOperation : PredicateOperation
{
    private Regex _regex;

    public PredicateRegexpOperation(string pattern)
    {
        _regex = new Regex(pattern);
    }

    public override BooleanLiteral Calculate(Literal literal)
    {
        if (literal is not TextLiteral textLiteral)
            throw new InvalidOperationException();

        return new BooleanLiteral(_regex.IsMatch(textLiteral.Value));
    }
}