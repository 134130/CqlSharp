namespace CqlSharp;

public abstract class QueryResult
{
    public int AffectedRows { get; init; }

    public TimeSpan Elapsed { get; init; }
}

public sealed class SelectResult : QueryResult
{
    public string[] Columns { get; init; }

    public IEnumerable<string[]> Rows { get; init; }
}

public sealed class InsertResult : QueryResult
{
}