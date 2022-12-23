namespace CqlSharp.Sql.Queries;

internal class OrderByComparer : IComparer<string[]>
{
    private Func<string[], string[], int> _delegate;

    public OrderByComparer(Func<string[], string[], int> delegete)
    {
        _delegate = delegete;
    }

    public int Compare(string[]? x, string[]? y)
    {
        if (x is null || y is null)
            throw new InvalidOperationException();

        return _delegate(x, y);
    }
}