using Antlr4.Runtime.Tree;
using CqlSharp.Exceptions;
using CqlSharp.Extension;
using CqlSharp.Sql.Expressions;
using CqlSharp.Sql.Expressions.Literals;
using CqlSharp.Sql.Expressions.Predicate;
using CqlSharp.Sql.Expressions.Predicate.Operations;

namespace CqlSharp.Parser.Visitor;

using static CqlParser;

internal static class ExpressionVisitor
{
    public static IExpression VisitExpression(ExpressionContext context)
    {
        switch (context)
        {
            case ExpressionDefaultContext defaultContext:
                var defaultExpression = VisitBoolPrimitive(defaultContext.GetChild<BoolPrimitiveContext>(0));
                return defaultExpression;

            case ExpressionIsContext isContext:
                var isTrue = isContext.type.Type == TRUE_SYMBOL;

                var notSymbol = isContext.GetToken(NOT_SYMBOL, 0);

                var type = isTrue
                    ? (notSymbol is null ? IsType.IsTrue : IsType.IsNotTrue)
                    : (notSymbol is null ? IsType.IsFalse : IsType.IsNotFalse);

                var isVisited = VisitBoolPrimitive(isContext.GetChild<BoolPrimitiveContext>(0));

                return new IsExpression(isVisited, type);

            case ExpressionNotContext notContext:
                return new NotExpression(VisitExpression(notContext.GetChild<ExpressionContext>(0)));

            case ExpressionAndContext andContext:
                return new AndOrExpression(
                    VisitExpression(andContext.GetChild<ExpressionContext>(0)),
                    AndOrType.And,
                    VisitExpression(andContext.GetChild<ExpressionContext>(1)));

            case ExpressionOrContext orContext:
                return new AndOrExpression(
                    VisitExpression(orContext.GetChild<ExpressionContext>(0)),
                    AndOrType.Or,
                    VisitExpression(orContext.GetChild<ExpressionContext>(1)));
        }

        throw new CqlNotSupportedException(context);
    }

    private static IExpression VisitBoolPrimitive(BoolPrimitiveContext context)
    {
        var child = context.GetChild(0);

        switch (child)
        {
            case PredicateContext predicateContext:
                var visited = VisitPredicate(predicateContext);
                return visited;

            case BoolPrimitiveContext boolPrimitive:
                var left = VisitBoolPrimitive(boolPrimitive);
                var right = VisitPredicate(context.predicate());
                var compareOperator = VisitCompareOperator(context.compareOperator());

                return right switch
                {
                    PredicateExpression predicate => new CompareExpression(left, compareOperator, predicate),
                    Literal literal => new CompareExpression(left, compareOperator, literal),
                    QualifiedIdentifier qIdentifier => new CompareExpression(left, compareOperator, qIdentifier),
                };
        }

        throw new CqlNotSupportedException(child);
    }

    private static CompareOperator VisitCompareOperator(CompareOperatorContext context)
    {
        var symbol = ((ITerminalNode)context.GetChild(0)).Symbol;
        return symbol.Type switch
        {
            EQUAL_OPERATOR => CompareOperator.Equal,
            NOT_EQUAL_OPERATOR => CompareOperator.NotEqual,
            GREATER_OR_EQUAL_OPERATOR => CompareOperator.GreaterOrEqual,
            GREATER_THAN_OPERATOR => CompareOperator.GreaterThan,
            LESS_OR_EQUAL_OPERATOR => CompareOperator.LessOrEqual,
            LESS_THAN_OPERATOR => CompareOperator.LessThan,
            _ => throw new ArgumentOutOfRangeException(symbol.Text)
        };
    }

    private static IExpression VisitPredicate(PredicateContext context)
    {
        var simpleExpression = VisitSimpleExpression(context.GetChild<SimpleExpressionContext>(0));

        var predicateOperationContext = context.GetChild<PredicateOperationContext>(0);
        if (context.GetChild<PredicateOperationContext>(0) is null)
            return simpleExpression;

        var predicateOperation = VisitPredicateOperation(predicateOperationContext);

        var predicateExpression = simpleExpression switch
        {
            Literal literal => new PredicateExpression(literal, predicateOperation),
            QualifiedIdentifier identifier => new PredicateExpression(identifier, predicateOperation),
            _ => throw new ArgumentOutOfRangeException(simpleExpression.GetType().ToString())
        };

        predicateExpression.IsNot = context.GetToken(NOT_SYMBOL, 0) is not null;
        return predicateExpression;
    }

    private static IExpression VisitSimpleExpression(SimpleExpressionContext context)
    {
        switch (context)
        {
            case SimpleExpressionLiteralContext simpleExpressionLiteral:
                var literal = VisitLiteral(simpleExpressionLiteral.GetChild<LiteralContext>(0));
                return literal;

            case SimpleExpressionColumnReferenceContext columnReference:
                var qualifiedIdentifier = new QualifiedIdentifier(
                    columnReference.children
                        .OfType<IdentifierContext>()
                        .Select(x => x.GetText()));
                return qualifiedIdentifier;
        }

        throw new CqlNotSupportedException(context);
    }

    private static Literal VisitLiteral(LiteralContext context)
    {
        return context.GetChild(0) switch
        {
            TextLiteralContext textLiteral => new TextLiteral(textLiteral.GetText()[1..^1]),
            NumLiteralContext numLiteral => new NumberLiteral(int.Parse(numLiteral.GetText())),
            BoolLiteralContext boolLiteral => new BooleanLiteral(bool.Parse(boolLiteral.GetText())),
            _ => throw new InvalidOperationException()
        };
    }

    private static PredicateOperation VisitPredicateOperation(PredicateOperationContext context)
    {
        switch (context)
        {
            case PredicateOperationLikeContext likeContext:
                var likePattern = VisitTextStringLiteral(likeContext.GetChild<TextStringLiteralContext>(0));

                return new PredicateLikeOperation(likePattern);

            case PredicateOperationInContext inContext:
                var inItems = inContext.children
                    .OfType<TextStringLiteralContext>()
                    .Select(VisitTextStringLiteral);

                return new PredicateInOperation(inItems);

            case PredicateOperationBetweenContext betweenContext:
                var betweenItems = betweenContext.children
                    .OfType<TextStringLiteralContext>()
                    .Select(VisitTextStringLiteral)
                    .ToArray();

                return new PredicateBetweenOperation(betweenItems.First(), betweenItems.Last());

            case PredicateOperationRegexpContext regexpContext:
                var regexpPattern = VisitTextStringLiteral(regexpContext.GetChild<TextStringLiteralContext>(0));

                return new PredicateRegexpOperation(regexpPattern);
        }

        throw new CqlNotSupportedException(context);
    }

    private static string VisitTextStringLiteral(TextStringLiteralContext context)
    {
        return context.value.Text[1..^1];
    }
}