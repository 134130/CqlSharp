using Antlr4.Runtime.Tree;
using CqlSharp.Expressions;
using CqlSharp.Extension;
using CqlSharp.Query;

namespace CqlSharp.Parser.Visitor;

using static CqlParser;

public static class SelectVisitor
{
    public static Select VisitSelectStatement(SelectStatementContext context)
    {
        return context.GetChild(0) switch
        {
            SelectExpressionContext expressionContext => VisitSelectExpression(expressionContext),
            SelectExpressionWithParensContext withParensContext => VisitSelectExpressionWithParens(withParensContext),
            _ => throw new InvalidOperationException()
        };
    }

    private static Select VisitSelectExpressionWithParens(SelectExpressionWithParensContext context)
    {
        return context.GetChild(1) switch
        {
            SelectExpressionContext expressionContext => VisitSelectExpression(expressionContext),
            SelectExpressionWithParensContext withParensContext => VisitSelectExpressionWithParens(withParensContext),
            _ => throw new InvalidOperationException()
        };
    }

    private static Select VisitSelectExpression(SelectExpressionContext context)
    {
        var selectQuery = new Select();
        foreach (var child in context.children)
        {
            switch (child)
            {
                case SelectItemListContext itemList:
                    selectQuery.Columns = VisitSelectItemList(itemList).ToList();
                    break;
                case FromClauseContext fromClause:
                    selectQuery.From = VisitFromClause(fromClause);
                    break;
                case WhereClauseContext whereClause:
                    selectQuery.WhereExpression = VisitWhereClause(whereClause);
                    break;
                case OrderClauseContext orderClause:
                    selectQuery.OrderBys = VisitOrderClause(orderClause).ToList();
                    break;
                case LimitClauseContext limitClause:
                    var (limit, offset) = VisitLimitClause(limitClause);
                    selectQuery.Limit = limit;
                    selectQuery.Offset = offset;
                    break;
            }
        }

        return selectQuery;
    }

    private static IEnumerable<IColumn> VisitSelectItemList(SelectItemListContext context)
    {
        return context.children
            .OfType<SelectItemContext>()
            .Select(VisitSelectItem);
    }

    private static IColumn VisitSelectItem(SelectItemContext context)
    {
        switch (context)
        {
            case SingleItemSelectContext singleItemSelectContext:
                return VisitSingleItemSelect(singleItemSelectContext);
            case MultItemSelectContext multItemSelectContext:
                if (multItemSelectContext.GetChild(0) is IdentifierContext identifier)
                    return new QualifiedIdentifier(identifier.GetText(), "*");

                if (multItemSelectContext.GetChild(0) is ITerminalNode { Symbol.Type: MULT_OPERATOR })
                    return new QualifiedIdentifier("*");
                break;
        }

        throw new InvalidOperationException();
    }

    private static IColumn VisitSingleItemSelect(SingleItemSelectContext context)
    {
        var expressionContext = (ExpressionContext)context.GetChild(0);
        var expression = ExpressionVisitor.VisitExpression(expressionContext);

        var alias = context.children.GetContext<IdentifierContext>()?.GetText() ??
                    context.children.GetContext<TextStringLiteralContext>()?.GetText();

        return new ExpressionColumn(expression, alias);
    }

    private static TableReference VisitFromClause(FromClauseContext context)
    {
        var tableReferenceContext = context.GetChild<TableReferenceContext>(0);
        switch (tableReferenceContext.GetChild(0))
        {
            case SingleTableContext singleTable:
                return VisitSingleTable(singleTable);
            case SingleTableWithParensContext singleTableWithParens:
                return VisitSingleTableWithParens(singleTableWithParens);
            case SelectExpressionWithParensContext expressionWithParens:
                var subQuery = VisitSelectExpressionWithParens(expressionWithParens);

                var identifierNode = tableReferenceContext.GetChild<IdentifierContext>(0);
                var alias = identifierNode?.GetText();

                return new SubQueryTableReference(subQuery, alias);
        }

        throw new InvalidOperationException();
    }

    private static TableReference VisitSingleTableWithParens(SingleTableWithParensContext context)
    {
        return context.GetChild(1) switch
        {
            SingleTableContext singleTable => VisitSingleTable(singleTable),
            SingleTableWithParensContext singleTableWithParens => VisitSingleTableWithParens(singleTableWithParens),
            _ => throw new InvalidOperationException()
        };
    }

    private static TableReference VisitSingleTable(SingleTableContext context)
    {
        var identifierNode = context.GetChild(2) ?? context.GetChild(1);
        var alias = identifierNode?.GetText();

        switch (context.GetChild(0))
        {
            case IdentifierContext identifier:
                var column = new QualifiedIdentifier(identifier.GetText());
                return new SingleTableReference(column, alias);
            case CsvFilePathContext csvFilePath:
                return new CsvTableReference(csvFilePath.GetText()[1..^1], alias);
        }

        throw new InvalidOperationException();
    }

    private static IExpression VisitWhereClause(WhereClauseContext context)
    {
        var child = context.GetChild<ExpressionContext>(0);
        if (ExpressionVisitor.VisitExpression(child) is not IExpression whereExpression)
            throw new InvalidOperationException();

        return whereExpression;
    }

    private static IEnumerable<OrderBy> VisitOrderClause(OrderClauseContext context)
    {
        var orderListContext = (OrderListContext)context.GetChild(2);

        var orderBys = orderListContext.children
            .OfType<OrderExpressionContext>()
            .Select(x => VisitOrderExpression(x));

        return orderBys;
    }

    private static OrderBy VisitOrderExpression(OrderExpressionContext context)
    {
        var orderBy = new OrderBy();

        // Identifier
        var identifier = context.GetChild(0).GetText();
        orderBy.Column = new QualifiedIdentifier(identifier);

        // Direction
        if (context.GetChild(1)?.GetChild(0) is ITerminalNode directionNode)
        {
            orderBy.SortType = directionNode.Symbol.Type switch
            {
                ASC_SYMBOL => SortType.Asc,
                DESC_SYMBOL => SortType.Desc,
                _ => orderBy.SortType
            };
        }

        return orderBy;
    }

    private static (int, int) VisitLimitClause(LimitClauseContext context)
    {
        var limit = int.Parse(context.GetChild(1).GetText());
        var offsetText = context.GetChild(3)?.GetText();
        var offset = offsetText is not null ? int.Parse(offsetText) : 0;

        return (limit, offset);
    }
}