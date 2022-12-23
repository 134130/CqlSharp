namespace CqlSharp.Query;

public abstract class TableReference : ISql
{
    public string? Alias { get; protected init; }
    public abstract string GetSql();
}