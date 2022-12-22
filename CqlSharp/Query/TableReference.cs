namespace CqlSharp.Query;

public abstract class TableReference
{
    public string Alias { get; protected init; }
}