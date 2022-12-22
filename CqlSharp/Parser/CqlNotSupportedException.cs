using Antlr4.Runtime.Tree;

namespace CqlSharp.Parser;

internal class CqlNotSupportedException : NotSupportedException
{
    public CqlNotSupportedException(IParseTree tree) : base($"Not supported tree: {tree.GetText()}")
    {
    }
}