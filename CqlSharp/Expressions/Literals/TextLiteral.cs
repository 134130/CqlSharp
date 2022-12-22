namespace CqlSharp.Expressions.Literals;

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
}