using CqlSharp.Expressions;
using CqlSharp.Expressions.Literals;
using CqlSharp.Extension;
using CqlSharp.Query;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace CqlSharp;

internal class SelectService
{
    private static readonly ILogger Logger = new LoggerConfiguration()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
        .MinimumLevel.Verbose()
        .CreateLogger();

    internal static async ValueTask<Table> ProcessAsync(Select selectQuery)
    {
        Logger.Verbose("    Plain SQL: {PlainSql}", selectQuery.GetSql());
        selectQuery.WhereExpression = selectQuery.WhereExpression?.GetOptimizedExpression();
        Logger.Verbose("Optimized SQL: {OptimizedSql}", selectQuery.GetSql());

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
                throw new NotImplementedException();
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

    internal static IEnumerable<int> GetColumnIndexes(IEnumerable<IColumn> columns, ITable table)
    {
        return columns
            .Select(x =>
            {
                var qualifiedIdentifier = x switch
                {
                    ExpressionColumn ec => ec.Expression as QualifiedIdentifier,
                    QualifiedIdentifier qi => qi,
                    _ => null
                };

                if (qualifiedIdentifier is null)
                    throw new InvalidDataException($"Column '{qualifiedIdentifier}' does not exists");

                var index = table.IndexOfColumn(qualifiedIdentifier);

                if (index is -1)
                    throw new InvalidDataException($"Column '{qualifiedIdentifier}' does not exists");

                return index;
            });
    }

    private static IEnumerable<string[]> GetSelectedRows(
        IEnumerable<IColumn> columns,
        IEnumerable<string[]> rows,
        ITable table)
    {
        var columnIndexes = GetColumnIndexes(columns, table);
        return rows.Select(x => x.ElementsAt(columnIndexes).ToArray());
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