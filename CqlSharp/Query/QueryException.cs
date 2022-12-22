namespace CqlSharp.Query;

public class QueryException : Exception
{
    public QueryException(string message) : base(message)
    {
    }
}