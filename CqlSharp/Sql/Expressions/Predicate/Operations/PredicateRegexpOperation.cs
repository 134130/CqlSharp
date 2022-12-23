using System.Text.RegularExpressions;
using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions.Predicate.Operations;

internal sealed class PredicateRegexpOperation : PredicateOperation
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

    public override string GetSql()
    {
        return $"REGEXP {_regex}";
    }
}