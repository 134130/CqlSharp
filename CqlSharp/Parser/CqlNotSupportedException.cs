using Antlr4.Runtime.Tree;
using CqlSharp.Expressions;

namespace CqlSharp.Parser;

public class CqlNotSupportedException : NotSupportedException
{
    public CqlNotSupportedException(IParseTree tree) : base($"Not supported tree: {tree}")
    {
    }
}