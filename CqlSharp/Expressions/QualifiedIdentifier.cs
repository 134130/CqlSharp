using CqlSharp.Expressions.Literals;
using CqlSharp.Extension;

namespace CqlSharp.Expressions;

public class QualifiedIdentifier : IColumn
{
    public int Length => Identifiers.Length;

    public string Name => string.Join('.', Identifiers);
    public string[] Identifiers { get; set; }

    public string this[int index] => Identifiers[index];

    public string this[Index index] => Identifiers[index];

    public string[] this[Range range] => Identifiers[range];

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
            throw new Exception($"Column '{Name}' doesn't exists");

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