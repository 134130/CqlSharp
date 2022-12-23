using Antlr4.Runtime.Tree;

namespace CqlSharp.Exceptions;

internal class CqlNotSupportedTreeException : CqlSharpException
{
    public CqlNotSupportedTreeException(IParseTree tree) : base($"Not supported tree: {tree.GetText()}")
    {
    }
}