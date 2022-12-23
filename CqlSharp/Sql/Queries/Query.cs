namespace CqlSharp.Sql.Queries;

public abstract class Query : ISql
{
    public abstract string GetSql();
}