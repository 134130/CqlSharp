using CqlSharp.Expressions;

namespace CqlSharp.Query;

public class OrderBy
{
    public QualifiedIdentifier Column { get; set; }

    public SortType SortType { get; set; }
}