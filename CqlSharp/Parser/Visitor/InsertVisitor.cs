using CqlSharp.Exceptions;
using CqlSharp.Sql.Queries;
using CqlSharp.Sql.Tables;

namespace CqlSharp.Parser.Visitor;

using static CqlParser;

internal static class InsertVisitor
{
    public static Insert VisitInsertStatement(InsertStatementContext context)
    {
        return context switch
        {
            InsertStatementSelectContext selectContext => VisitInsertStatementSelect(selectContext),
            InsertStatementValuesContext valuesContext => VisitInsertStatementValues(valuesContext),
            _ => throw new CqlNotSupportedTreeException(context)
        };
    }

    private static Insert VisitInsertStatementSelect(InsertStatementSelectContext context)
    {
        var csvFilePath = VisitIntoClause(context.GetChild<IntoClauseContext>(0));
        var selectQuery = SelectVisitor.VisitSelectStatement(context.GetChild<SelectStatementContext>(0));

        return new Insert(csvFilePath, new SubQueryTableReference(selectQuery));
    }

    private static Insert VisitInsertStatementValues(InsertStatementValuesContext context)
    {
        var csvFilePath = VisitIntoClause(context.GetChild<IntoClauseContext>(0));
        var columnNames = VisitColumnNames(context.GetChild<ColumnNamesContext>(0));
        var values = VisitValuesClause(context.GetChild<ValuesClauseContext>(0));

        var referenceTable = new Table(columnNames, values);
        return new Insert(csvFilePath, referenceTable);
    }

    private static string VisitIntoClause(IntoClauseContext context)
    {
        var csvFilePath = context.GetChild<CsvFilePathContext>(0).GetText()[1..^1];

        if (string.IsNullOrWhiteSpace(csvFilePath))
            throw new ArgumentException(csvFilePath);

        return csvFilePath;
    }

    private static string[] VisitColumnNames(ColumnNamesContext context)
    {
        return context.GetRuleContexts<IdentifierContext>()
            .Select(x => x.GetText())
            .ToArray();
    }

    private static string[][] VisitValuesClause(ValuesClauseContext context)
    {
        return context.GetChild<ValueItemListContext>(0)
            .GetRuleContexts<ValueItemContext>()
            .Select(VisitValueItem)
            .ToArray();
    }

    private static string[] VisitValueItem(ValueItemContext context)
    {
        return context.GetRuleContexts<TextStringLiteralContext>()
            .Select(x => x.GetText()[1..^1])
            .ToArray();
    }
}