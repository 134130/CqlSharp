using System.Text.RegularExpressions;
using CqlSharp.Extension;
using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions.Predicate.Operations;

internal sealed class PredicateLikeOperation : PredicateOperation
{
    private string _originalPattern;
    private Regex _regex;

    public PredicateLikeOperation(string pattern)
    {
        _originalPattern = pattern;
        _regex = new Regex($"^{pattern.Replace('%', ".*", '\\')}$");
    }

    public override BooleanLiteral Calculate(Literal literal)
    {
        if (literal is not TextLiteral textLiteral)
            throw new InvalidOperationException();

        return new BooleanLiteral(_regex.IsMatch(textLiteral.Value));
    }

    public override string GetSql()
    {
        return $"LIKE {_originalPattern}";
    }
}