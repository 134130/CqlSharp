using Antlr4.Runtime;

namespace CqlSharp.Parser;

internal class CQLLexerErrorHandler : IAntlrErrorListener<int>
{
    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line,
        int charPositionInLine,
        string msg, RecognitionException e)
    {
        throw new Exception();
    }
}