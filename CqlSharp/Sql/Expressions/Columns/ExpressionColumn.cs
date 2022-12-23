using CqlSharp.Sql.Expressions.Literals;

namespace CqlSharp.Sql.Expressions.Columns;

internal class ExpressionColumn : IColumn
{
    public IExpression Expression { get; }

    public string? Alias { get; }

    public ExpressionColumn(IExpression expression, string? alias = null)
    {
        Expression = expression;
        Alias = alias;
    }

    public Literal Calculate(QualifiedIdentifier[] columns, string[] row)
    {
        return Expression.Calculate(columns, row);
    }

    public IExpression GetOptimizedExpression()
    {
        var optimizedExpression = Expression.GetOptimizedExpression();
        return new ExpressionColumn(optimizedExpression, Alias);
    }

    public string GetSql()
    {
        if (Alias is null)
            return Expression.GetSql();

        return $"{Expression.GetSql()} AS {Alias}";
    }
}