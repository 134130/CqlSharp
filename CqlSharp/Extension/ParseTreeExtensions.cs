using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CqlSharp.Extension;

public static class ParseTreeExtensions
{
    public static T? GetContext<T>(this IList<IParseTree> children) where T : ParserRuleContext
    {
        return children.OfType<T>().FirstOrDefault();
    }

    public static ITerminalNode? GetToken(this IList<IParseTree> children, int ttoken)
    {
        return children.OfType<ITerminalNode>().FirstOrDefault(x => x.Symbol.Type == ttoken);
    }
}