namespace CqlSharp.Sql.Queries;

internal class CsvTableReference : TableReference
{
    public string FilePath { get; init; }

    public CsvTableReference(string filePath)
    {
        FilePath = filePath;
    }

    public CsvTableReference(string filePath, string alias)
    {
        FilePath = filePath;
        Alias = alias;
    }

    public override string GetSql()
    {
        if (Alias is null)
            return $"\"{FilePath}\"";

        return $"\"{FilePath}\" AS {Alias}";
    }
}