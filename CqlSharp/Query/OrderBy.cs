using CqlSharp.Expressions;

namespace CqlSharp.Query;

public class OrderBy : ISql
{
    public QualifiedIdentifier Column { get; set; }

    public SortType SortType { get; set; }

    public string GetSql()
    {
        var sortTypeString = SortType switch
        {
            SortType.Asc => "ASC",
            SortType.Desc => "DESC",
            _ => throw new ArgumentOutOfRangeException()
        };
        return $"{Column.GetSql()} {sortTypeString}";
    }
}