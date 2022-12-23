namespace CqlSharp.Sql.Queries;

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

    public override string GetSql()
    {
        if (Alias is null)
            return $"({SubQuery.GetSql()})";

        return $"({SubQuery.GetSql()}) AS {Alias}";
    }
}