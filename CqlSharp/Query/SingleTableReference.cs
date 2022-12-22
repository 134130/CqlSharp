using CqlSharp.Expressions;

namespace CqlSharp.Query;

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
}