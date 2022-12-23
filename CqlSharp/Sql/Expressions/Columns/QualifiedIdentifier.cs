using CqlSharp.Exceptions;
using CqlSharp.Extension;
using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions.Columns;

internal class QualifiedIdentifier : IColumn
{
    public int Length => Identifiers.Length;

    public string Name => string.Join('.', Identifiers);

    public string[] Identifiers { get; set; }

    public string this[int index] => Identifiers[index];

    public string this[Index index] => Identifiers[index];

    public string[] this[Range range] => Identifiers[range];

    public static bool operator ==(QualifiedIdentifier a, QualifiedIdentifier b) =>
        a.Identifiers.SequenceEqual(b.Identifiers);

    public static bool operator !=(QualifiedIdentifier a, QualifiedIdentifier b) =>
        !a.Identifiers.SequenceEqual(b.Identifiers);

    public bool IsWildcard => Identifiers[^1] == "*";

    public QualifiedIdentifier(params string[] identifiers)
    {
        Identifiers = identifiers;
    }

    public QualifiedIdentifier(IEnumerable<string> identifiers)
    {
        Identifiers = identifiers.ToArray();
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        var index = columns.IndexOf(this);

        if (index < 0)
            throw new ColumnNotfoundException(this);

        return new TextLiteral(row[index]);
    }

    public IExpression GetOptimizedExpression()
    {
        return this;
    }

    public string GetSql()
    {
        return Name;
    }
}