namespace CqlSharp.Sql.Expressions.Literals;

internal sealed class BooleanLiteral : Literal
{
    public static BooleanLiteral True => new(true);
    public static BooleanLiteral False => new(false);

    public bool Value { get; set; }

    public BooleanLiteral()
    {
    }

    public BooleanLiteral(bool value)
    {
        Value = value;
    }

    public static BooleanLiteral operator !(BooleanLiteral x)
    {
        x.Value = !x.Value;
        return x;
    }

    public static bool operator true(BooleanLiteral x) => x.Value;
    public static bool operator false(BooleanLiteral x) => x.Value;

    public override string GetSql()
    {
        return Value ? "TRUE" : "FALSE";
    }
}