namespace CqlSharp.Query;

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
}