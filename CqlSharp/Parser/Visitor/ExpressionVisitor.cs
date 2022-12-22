using Antlr4.Runtime.Tree;
using CqlSharp.Expressions;
using CqlSharp.Expressions.Literals;
using CqlSharp.Expressions.Predicate;
using CqlSharp.Extension;

namespace CqlSharp.Parser.Visitor;

using static CqlParser;

public static class ExpressionVisitor
{
    public static IExpression VisitExpression(ExpressionContext context)
    {
        switch (context)
        {
            case ExpressionDefaultContext defaultContext:
                var defaultVisited = VisitBoolPrimitive(defaultContext.GetChild<BoolPrimitiveContext>(0));
                if (defaultVisited is not IExpression defaultExpression)
                    throw new InvalidOperationException();
                return defaultExpression;

            case ExpressionIsContext isContext:
                var isTrue = isContext.type.Type == TRUE_SYMBOL;
                var notSymbol = isContext.children.GetToken(NOT_SYMBOL);

                var type = isTrue
                    ? (notSymbol is null ? IsType.IsTrue : IsType.IsNotTrue)
                    : (notSymbol is null ? IsType.IsFalse : IsType.IsNotFalse);

                var isVisited = VisitBoolPrimitive(isContext.GetChild<BoolPrimitiveContext>(0));

                if (isVisited is not IExpression isExpression)
                    throw new InvalidOperationException();

                return new IsExpression(isExpression, type);

            case ExpressionNotContext notContext:
                return new NotExpression
                {
                    Expression = VisitExpression(notContext.GetChild<ExpressionContext>(0))
                };

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

        throw new InvalidOperationException();
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

                if (left is not IExpression leftExpression)
                    throw new InvalidOperationException();

                var compareExpression = new CompareExpression
                {
                    Left = leftExpression,
                    CompareOperator = compareOperator
                };

                if (right is PredicateExpression rightPredicate)
                    compareExpression.RightPredicateExpression = rightPredicate;

                else if (right is Literal rightLiteral)
                    compareExpression.RightLiteral = rightLiteral;

                else if (right is QualifiedIdentifier rightIdentifier)
                    compareExpression.RightIdentifier = rightIdentifier;

                return compareExpression;
        }

        throw new InvalidOperationException();
    }

    private static CompareOperator VisitCompareOperator(CompareOperatorContext context)
    {
        return ((ITerminalNode)context.GetChild(0)).Symbol.Type switch
        {
            EQUAL_OPERATOR => CompareOperator.Equal,
            NOT_EQUAL_OPERATOR => CompareOperator.NotEqual,
            GREATER_OR_EQUAL_OPERATOR => CompareOperator.GreaterOrEqual,
            GREATER_THAN_OPERATOR => CompareOperator.GreaterThan,
            LESS_OR_EQUAL_OPERATOR => CompareOperator.LessOrEqual,
            LESS_THAN_OPERATOR => CompareOperator.LessThan,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static IExpression VisitPredicate(PredicateContext context)
    {
        var simpleExpression = VisitSimpleExpression(context.GetChild<SimpleExpressionContext>(0));

        PredicateOperation predicateOperation = null;
        if (context.GetChild<PredicateOperationContext>(0) is { } predicateOperationContext)
        {
            predicateOperation = VisitPredicateOperation(predicateOperationContext);
        }

        PredicateExpression predicateExpression = null;
        switch (simpleExpression)
        {
            case Literal literal:
                if (predicateOperation is null)
                    return literal;

                predicateExpression = new PredicateExpression(literal, predicateOperation);
                break;
            case QualifiedIdentifier identifier:
                if (predicateOperation is null)
                    return identifier;

                predicateExpression = new PredicateExpression(identifier, predicateOperation);
                break;
        }

        if (predicateExpression is null)
            throw new InvalidOperationException();

        predicateExpression.IsNot = context.children.GetToken(NOT_SYMBOL) is not null;
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

        throw new InvalidOperationException();
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

        throw new InvalidOperationException();
    }

    private static string VisitTextStringLiteral(TextStringLiteralContext context)
    {
        return context.value.Text[1..^1];
    }
}