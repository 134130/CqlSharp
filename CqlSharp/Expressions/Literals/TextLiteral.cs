namespace CqlSharp.Expressions.Literals;

public sealed class TextLiteral : Literal
{
    public string Value { get; set; }

    public TextLiteral()
    {
    }

    public TextLiteral(string value)
    {
        Value = value;
    }
}