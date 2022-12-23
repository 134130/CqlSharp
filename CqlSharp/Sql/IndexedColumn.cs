using CqlSharp.Sql.Expressions.Columns;

namespace CqlSharp.Sql;

internal sealed class IndexedColumn
{
    public IColumn OriginalColumn { get; set; }

    public int IndexOfRow { get; set; }

    public IndexedColumn(IColumn column, int index)
    {
        OriginalColumn = column;
        IndexOfRow = index;
    }
}