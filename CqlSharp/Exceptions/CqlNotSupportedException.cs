using Antlr4.Runtime.Tree;

namespace CqlSharp.Exceptions;

internal class CqlNotSupportedException : CqlSharpException
{
    public CqlNotSupportedException(IParseTree tree) : base($"Not supported tree: {tree.GetText()}")
    {
    }
}