using System.Runtime.CompilerServices;
using CqlSharp.Sql.Expressions;
using CqlSharp.Sql.Expressions.Columns;
using CqlSharp.Sql.Expressions.Literals;
using CqlSharp.Sql.Query;
using CqlSharp.Sql.Tables;

namespace CqlSharp.Sql;

internal class CsvSelectService : SelectService
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

        if (selectQuery.IsCountQuery)
        {
            var rowLength = await CountRowsAsync(csv, selectQuery.WhereExpression);
            return new Table(selectQuery.Columns, new[] { new[] { rowLength.ToString() } });
        }

        EscapeWildcards(selectQuery.Columns, csv.Columns);

        // Where, Offset
        SkipOffsetAsync(csv, selectQuery.Offset, selectQuery.WhereExpression);

        // Select, Where, Limit
        if (selectQuery.WhereExpression is not null)
            return new Table(csv.Columns, await GetWheredRowsAsync(csv, selectQuery));

        // Select
        var rows = new List<string[]>();

        var indexedColumns = GetIndexedColumns(selectQuery.Columns, csv).ToArray();
        var columnIndexes = indexedColumns.Select(x => x.IndexOfRow).ToArray();

        // Limit
        if (selectQuery.Limit > 0)
        {
            while (rows.Count < selectQuery.Limit && await csv.ReadAsync())
            {
                var fetchedRow = csv.FetchRow(columnIndexes);

                // inject calculated column
                for (var i = 0; i < indexedColumns.Length; i++)
                {
                    if (indexedColumns[i].IndexOfRow >= 0)
                        continue;

                    if (fetchedRow[i] is not null)
                        throw new InvalidOperationException();

                    fetchedRow[i] = indexedColumns[i].OriginalColumn
                        .Calculate(csv.Columns, fetchedRow)
                        .GetSql();
                }

                rows.Add(fetchedRow!);
            }
        }
        else
        {
            while (await csv.ReadAsync())
            {
                var fetchedRow = csv.FetchRow(columnIndexes);

                // inject calculated column
                for (var i = 0; i < indexedColumns.Length; i++)
                {
                    if (indexedColumns[i].IndexOfRow >= 0)
                        continue;

                    if (fetchedRow[i] is not null)
                        throw new InvalidOperationException();

                    fetchedRow[i] = indexedColumns[i].OriginalColumn
                        .Calculate(csv.Columns, fetchedRow)
                        .GetSql();
                }

                rows.Add(fetchedRow!);
            }
        }

        return new Table(selectQuery.Columns, rows);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async ValueTask<IEnumerable<string[]>> GetWheredRowsAsync(CsvFile csv, Select selectQuery)
    {
        if (selectQuery.WhereExpression is null)
            throw new InvalidOperationException();

        var indexedColumns = GetIndexedColumns(selectQuery.Columns, csv).ToArray();
        var columnIndexes = indexedColumns.Select(x => x.IndexOfRow);

        var rows = new List<string[]>();
        if (selectQuery.Limit > 0)
        {
            while (rows.Count < selectQuery.Limit && await csv.ReadAsync())
            {
                var fetchedRow = csv.FetchRow();

                if (!IsExpectedRow(selectQuery.WhereExpression, csv.Columns, fetchedRow))
                    continue;

                var row = new string?[indexedColumns.Length];

                // inject calculated column
                for (var i = 0; i < indexedColumns.Length; i++)
                {
                    if (indexedColumns[i].IndexOfRow >= 0)
                    {
                        row[i] = fetchedRow[indexedColumns[i].IndexOfRow];
                        continue;
                    }

                    row[i] = indexedColumns[i].OriginalColumn
                        .Calculate(csv.Columns, fetchedRow)
                        .GetSql();
                }

                rows.Add(row!);
            }
        }
        else
        {
            while (await csv.ReadAsync())
            {
                var fetchedRow = csv.FetchRow();

                if (selectQuery.WhereExpression.Calculate(csv.Columns, fetchedRow) is not BooleanLiteral booleanLiteral)
                    throw new InvalidOperationException();

                if (booleanLiteral.Value is false)
                    continue;

                var row = new string?[indexedColumns.Length];

                // inject calculated column
                for (var i = 0; i < indexedColumns.Length; i++)
                {
                    if (indexedColumns[i].IndexOfRow >= 0)
                    {
                        row[i] = fetchedRow[indexedColumns[i].IndexOfRow];
                        continue;
                    }

                    row[i] = indexedColumns[i].OriginalColumn
                        .Calculate(csv.Columns, fetchedRow)
                        .GetSql();
                }

                rows.Add(row!);
            }
        }

        return rows;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    private static async ValueTask<int> CountRowsAsync(CsvFile csv, IExpression? whereExpression)
    {
        var rowCount = 0;
        if (whereExpression is null)
        {
            while (await csv.ReadAsync())
            {
                rowCount++;
            }

            return rowCount;
        }

        while (await csv.ReadAsync())
        {
            var row = csv.FetchRow();

            if (whereExpression.Calculate(csv.Columns, row) is not BooleanLiteral booleanLiteral)
                throw new InvalidOperationException();

            if (booleanLiteral)
                rowCount++;
        }

        return rowCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async ValueTask<Table> FetchCsvFileAsync(string filePath, string? alias = null)
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