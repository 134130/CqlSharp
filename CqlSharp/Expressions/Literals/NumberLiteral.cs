namespace CqlSharp.Expressions.Literals;

public sealed class NumberLiteral : Literal
{
    public int Value { get; set; }

    public NumberLiteral()
    {
    }

    public NumberLiteral(int value)
    {
        Value = value;
    }
}