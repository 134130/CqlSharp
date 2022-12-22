namespace CqlSharp.Query;

internal class SubQueryTableReference : TableReference
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