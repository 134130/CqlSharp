using CqlSharp.Sql.Expressions.Literals;
using CqlSharp.Sql.Expressions.Predicate.Operations;

namespace CqlSharp.Sql.Expressions.Predicate;

internal class PredicateExpression : IExpression
{
    public Literal? Literal { get; }

    public QualifiedIdentifier? QualifiedIdentifier { get; }

    public PredicateOperation PredicateOperation { get; }

    public bool IsNot { get; set; }

    public PredicateExpression(Literal literal, PredicateOperation predicateOperation)
    {
        Literal = literal;
        PredicateOperation = predicateOperation;
    }

    public PredicateExpression(QualifiedIdentifier identifier, PredicateOperation predicateOperation)
    {
        QualifiedIdentifier = identifier;
        PredicateOperation = predicateOperation;
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        var qualifiedIdentifier = QualifiedIdentifier?.Calculate(columns, row);

        if (PredicateOperation is null)
            throw new ArgumentNullException($"{nameof(PredicateOperation)} can not be null");

        // TODO: remove literal, qidentifier and switch to IExpression
        var result = PredicateOperation.Calculate(Literal ?? qualifiedIdentifier);
        return IsNot ? !result : result;
    }

    public IExpression GetOptimizedExpression()
    {
        if (QualifiedIdentifier is not null)
            return this;

        if (Literal is null)
            throw new ArgumentNullException($"{nameof(Literal)} cat not be null");

        var result = PredicateOperation.Calculate(Literal);
        return IsNot ? !result : result;
    }

    public string GetSql()
    {
        var left = QualifiedIdentifier?.GetSql() ?? Literal?.GetSql() ?? "???";
        return $"{left} {PredicateOperation.GetSql()}";
    }
}