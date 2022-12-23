using CqlSharp.Sql.Expressions;
using CqlSharp.Sql.Expressions.Columns;

namespace CqlSharp.Sql.Query;

internal class SingleTableReference : TableReference
{
    public QualifiedIdentifier QualifiedIdentifier { get; init; }

    public SingleTableReference(QualifiedIdentifier qualifiedIdentifier)
    {
        QualifiedIdentifier = qualifiedIdentifier;
    }

    public SingleTableReference(QualifiedIdentifier qualifiedIdentifier, string alias)
    {
        QualifiedIdentifier = qualifiedIdentifier;
        Alias = alias;
    }

    public override string GetSql()
    {
        if (Alias is null)
            return $"\"{QualifiedIdentifier.GetSql()}\"";

        return $"\"{QualifiedIdentifier.GetSql()}\" AS {Alias}";
    }
}