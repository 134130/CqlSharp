using CqlSharp.Sql.Expressions.Columns;

namespace CqlSharp.Sql.Queries;

internal class OrderBy : ISql
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