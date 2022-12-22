using CqlSharp.Expressions;

namespace CqlSharp.Query;

public class Select
{
    public TableReference? From { get; set; }

    // Order
    public List<OrderBy>? OrderBys { get; set; }

    // Limit
    public int Offset { get; set; }

    public int Limit { get; set; }

    // Where
    public IExpression? WhereExpression { get; set; }

    // Columns
    public List<IColumn> Columns { get; set; }
}

public abstract class TableReference
{
    public string Alias { get; init; }
}

public class SingleTableReference : TableReference
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

public class SubQueryTableReference : TableReference
{
    public Select SubQuery { get; init; }

    public SubQueryTableReference(Select subQuery)
    {
        SubQuery = subQuery;
    }

    public SubQueryTableReference(Select subQuery, string alias)
    {
        SubQuery = subQuery;
        Alias = alias;
    }
}

public class CsvTableReference : TableReference
{
    public string FilePath { get; init; }

    public CsvTableReference(string filePath)
    {
        FilePath = filePath;
    }

    public CsvTableReference(string filePath, string alias)
    {
        FilePath = filePath;
        Alias = alias;
    }
}