using CqlSharp.Sql.Tables;

namespace CqlSharp.Sql.Queries;

internal class Insert : Query
{
    public string DestinationFilePath { get; }

    public SubQueryTableReference? SubQueryTableReference { get; }

    public Table? ReferenceTable { get; set; }

    public Insert(string filePath, SubQueryTableReference subQueryTableReference)
    {
        DestinationFilePath = filePath;
        SubQueryTableReference = subQueryTableReference;
    }

    public Insert(string filePath, Table referenceTable)
    {
        DestinationFilePath = filePath;
        ReferenceTable = referenceTable;
    }

    public override string GetSql()
    {
        throw new NotImplementedException();
    }
}