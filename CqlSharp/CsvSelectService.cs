using System.Runtime.CompilerServices;
using CqlSharp.Expressions;
using CqlSharp.Expressions.Literals;
using CqlSharp.Extension;
using CqlSharp.Query;

namespace CqlSharp;

public class CsvSelectService : SelectService
{
    public new static async ValueTask<Table> ProcessAsync(Select selectQuery)
    {
        var csvTableReference = selectQuery.From as CsvTableReference ?? throw new InvalidOperationException();

        var filePath = csvTableReference.FilePath;

        // Order By
        if (selectQuery.OrderBys is { Count: > 0 })
            return ProcessCore(selectQuery,
                await FetchCsvFileAsync(csvTableReference.FilePath, csvTableReference.Alias));

        using var csv = new CsvFile(filePath, csvTableReference.Alias);

        EscapeWildcards(selectQuery.Columns, csv.Columns);

        // Where, Offset
        SkipOffsetAsync(csv, selectQuery.Offset, selectQuery.WhereExpression);

        // Select, Where, Limit
        if (selectQuery.WhereExpression is not null)
            return new Table(csv.Columns, await GetWheredRowsAsync(csv, selectQuery));

        // Select
        var rows = new List<string[]>();

        var columnIndexes = GetColumnIndexes(selectQuery.Columns, csv).ToArray();

        // Limit
        if (selectQuery.Limit > 0)
        {
            while (rows.Count < selectQuery.Limit && await csv.ReadAsync())
            {
                rows.Add(csv.FetchRow(columnIndexes));
            }
        }
        else
        {
            while (await csv.ReadAsync())
            {
                rows.Add(csv.FetchRow(columnIndexes));
            }
        }

        return new Table(selectQuery.Columns, rows);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async ValueTask<IEnumerable<string[]>> GetWheredRowsAsync(CsvFile csv, Select selectQuery)
    {
        if (selectQuery.WhereExpression is null)
            throw new InvalidOperationException();

        var columnIndexes = GetColumnIndexes(selectQuery.Columns, csv).ToArray();

        var rows = new List<string[]>();
        if (selectQuery.Limit > 0)
        {
            while (rows.Count < selectQuery.Limit && await csv.ReadAsync())
            {
                var row = csv.FetchRow();

                if (!IsExpectedRow(selectQuery.WhereExpression, csv.Columns, row))
                    continue;

                var selectedRow = row.ElementsAt(columnIndexes);
                rows.Add(selectedRow.ToArray());
            }
        }
        else
        {
            while (await csv.ReadAsync())
            {
                var row = csv.FetchRow();

                if (selectQuery.WhereExpression.Calculate(csv.Columns, row) is not BooleanLiteral booleanLiteral)
                    throw new InvalidOperationException();

                if (booleanLiteral.Value is false)
                    continue;

                var selectedRow = row.ElementsAt(columnIndexes).ToArray();
                rows.Add(selectedRow);
            }
        }

        return rows;
    }

    private static bool IsExpectedRow(
        IExpression whereExpression, QualifiedIdentifier[] columns, string[] row)
    {
        if (whereExpression.Calculate(columns, row) is not BooleanLiteral booleanLiteral)
            throw new InvalidOperationException();

        return booleanLiteral.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async void SkipOffsetAsync(CsvFile csv, int offset, IExpression? whereExpression)
    {
        if (offset == 0)
            return;

        var currentOffset = 0;

        if (whereExpression is null)
        {
            while (currentOffset < offset && await csv.ReadAsync())
            {
                currentOffset++;
            }

            return;
        }

        while (currentOffset < offset && await csv.ReadAsync())
        {
            var row = csv.FetchRow();

            if (whereExpression.Calculate(csv.Columns, row) is not BooleanLiteral booleanLiteral)
                throw new InvalidOperationException();

            if (booleanLiteral)
                currentOffset++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<Table> FetchCsvFileAsync(string filePath, string alias = null)
    {
        var rows = new List<string[]>();
        using var csv = new CsvFile(filePath, alias);

        while (await csv.ReadAsync())
        {
            rows.Add(csv.FetchRow());
        }

        return new Table(csv.Columns, rows);
    }
}