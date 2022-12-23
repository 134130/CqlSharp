using CqlSharp.Sql.Expressions;

namespace CqlSharp.Exceptions;

public class CqlSharpException : Exception
{
    public CqlSharpException(string message) : base(message)
    {
    }
}

public class ColumnNotfoundException : CqlSharpException
{
    public ColumnNotfoundException(IColumn column) : base(CreateMessage(column))
    {
    }

    private static string CreateMessage(IColumn column)
    {
        var columnName = column switch
        {
            ExpressionColumn expressionColumn => expressionColumn.Expression.GetSql(),
            QualifiedIdentifier qualifiedIdentifier => qualifiedIdentifier.GetSql(),
            _ => throw new ArgumentOutOfRangeException(nameof(column), column, null)
        };

        return $"Column '{columnName}' does not exist";
    }
}