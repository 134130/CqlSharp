using CqlSharp.Exceptions;
using CqlSharp.Extension;
using CqlSharp.Sql.Expressions;
using CqlSharp.Sql.Expressions.Columns;
using CqlSharp.Sql.Expressions.Literals;
using CqlSharp.Sql.Query;
using CqlSharp.Sql.Tables;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace CqlSharp.Sql;

internal class SelectService
{
    private static readonly ILogger Logger = new LoggerConfiguration()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
        .MinimumLevel.Information()
        .CreateLogger();

    internal static async ValueTask<Table> ProcessAsync(Select selectQuery)
    {
        Logger.Information("    Plain SQL: {PlainSql}", selectQuery.GetSql());
        selectQuery.WhereExpression = selectQuery.WhereExpression?.GetOptimizedExpression();
        Logger.Information("Optimized SQL: {OptimizedSql}", selectQuery.GetSql());

        if (selectQuery.From is CsvTableReference)
            return await CsvSelectService.ProcessAsync(selectQuery);

        Table table;
        switch (selectQuery.From)
        {
            case SubQueryTableReference subQueryTableReference:
                table = await ProcessAsync(subQueryTableReference.SubQuery);
                table.Alias = subQueryTableReference.Alias;
                break;
            default:
                table = new Table(Array.Empty<QualifiedIdentifier>(), new[] { Array.Empty<string>() });
                break;
        }

        EscapeWildcards(selectQuery.Columns, table.Columns);

        return ProcessCore(selectQuery, table);
    }

    internal static Table ProcessCore(Select selectQuery, Table table)
    {
        // Order By
        var orderedRows = GetOrderedRows(table.Columns, table.Rows, selectQuery.OrderBys);

        // Where
        var wheredRows = GetWheredRows(table.Columns, orderedRows, selectQuery.WhereExpression);

        if (selectQuery.IsCountQuery)
            return new Table(selectQuery.Columns, wheredRows);

        // Offset
        var skippedRows = wheredRows.Skip(selectQuery.Offset);

        // Select
        var rows = GetSelectedRows(selectQuery.Columns, skippedRows, table);

        // Limit
        if (selectQuery.Limit > 0)
            rows = rows.Take(selectQuery.Limit);

        return new Table(selectQuery.Columns, rows);
    }

    internal static void EscapeWildcards(
        List<IColumn> columns, QualifiedIdentifier[] atColumns)
    {
        Logger.Verbose("{MethodName} >>", nameof(EscapeWildcards));
        Logger.Verbose("  atColumns: {AtColumns}", atColumns.Select(x => x.GetSql()));
        Logger.Verbose("     before: {BeforeColumns}", columns.Select(x => x.GetSql()));

        for (var i = 0; i < columns.Count; i++)
        {
            if (columns[i] is not QualifiedIdentifier { IsWildcard: true } wildcard)
                continue;

            var escaped = atColumns.Select(x =>
                new QualifiedIdentifier(new List<string>(wildcard[..^1]) { x.Name })
            );

            columns.RemoveAt(i);
            columns.InsertRange(i, escaped);
        }

        Logger.Verbose("      after: {AfterColumns}", columns.Select(x => x.GetSql()));
    }

    internal static IEnumerable<IndexedColumn> GetIndexedColumns(IEnumerable<IColumn> columns, ITable table)
    {
        return columns
            .Select(column =>
            {
                switch (column)
                {
                    case QualifiedIdentifier qi:
                        var qiIndex = table.IndexOfColumn(qi);

                        if (qiIndex is -1)
                            throw new ColumnNotfoundException(qi);

                        return new IndexedColumn(column, qiIndex);

                    case ExpressionColumn ec:
                        if (ec.Expression is not QualifiedIdentifier ecqi)
                            return new IndexedColumn(column, -1);

                        var ecqiIndex = table.IndexOfColumn(ecqi);
                        if (ecqiIndex is -1)
                            throw new ColumnNotfoundException(ecqi);
                        return new IndexedColumn(column, ecqiIndex);
                }

                throw new ArgumentOutOfRangeException();
            });
    }

    private static IEnumerable<string[]> GetSelectedRows(
        IEnumerable<IColumn> columns,
        IEnumerable<string[]> rows,
        ITable table)
    {
        var indexedColumns = GetIndexedColumns(columns, table).ToArray();

        return rows.Select(fetchedRow =>
        {
            var row = new string[indexedColumns.Length];

            // inject calculated column
            for (var i = 0; i < indexedColumns.Length; i++)
            {
                if (indexedColumns[i].IndexOfRow >= 0)
                {
                    row[i] = fetchedRow[indexedColumns[i].IndexOfRow];
                    continue;
                }

                row[i] = indexedColumns[i].OriginalColumn
                    .Calculate(table.Columns, fetchedRow)
                    .GetSql();
            }

            return row;
        });
    }

    private static IEnumerable<string[]> GetWheredRows(
        QualifiedIdentifier[] columns,
        IEnumerable<string[]> rows,
        IExpression? whereExpression)
    {
        if (whereExpression is null)
            return rows;

        return rows.Where(row =>
        {
            if (whereExpression.Calculate(columns, row) is not BooleanLiteral booleanLiteral)
                throw new InvalidOperationException();

            return booleanLiteral.Value;
        });
    }

    private static IEnumerable<string[]> GetOrderedRows(
        IEnumerable<QualifiedIdentifier> columns,
        IEnumerable<string[]> rows,
        IEnumerable<OrderBy>? orderBys)
    {
        if (orderBys is null)
            return rows;

        var comparer = new OrderByComparer((row1, row2) => orderBys
            .Select(orderBy =>
            {
                var i = columns.IndexOf(orderBy.Column);
                return string.CompareOrdinal(row1[i], row2[i]);
            })
            .FirstOrDefault(x => x is not 0));

        return rows.Order(comparer);
    }
}