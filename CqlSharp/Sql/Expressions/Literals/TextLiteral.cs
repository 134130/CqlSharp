namespace CqlSharp.Sql.Expressions.Literals;

internal sealed class TextLiteral : Literal
{
    public string Value { get; set; }

    public TextLiteral()
    {
    }

    public TextLiteral(string value)
    {
        Value = value;
    }

    public override string GetSql()
    {
        return $"'{Value}'";
    }
}