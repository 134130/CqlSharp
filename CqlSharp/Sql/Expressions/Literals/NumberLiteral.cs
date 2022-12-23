namespace CqlSharp.Sql.Expressions.Literals;

internal sealed class NumberLiteral : Literal
{
    public int Value { get; set; }

    public NumberLiteral()
    {
    }

    public NumberLiteral(int value)
    {
        Value = value;
    }

    public override string GetSql()
    {
        return Value.ToString();
    }

    public static NumberLiteral operator +(NumberLiteral x, NumberLiteral y)
    {
        return new NumberLiteral(x.Value + y.Value);
    }

    public static NumberLiteral operator -(NumberLiteral x, NumberLiteral y)
    {
        return new NumberLiteral(x.Value - y.Value);
    }
}