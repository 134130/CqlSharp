using Antlr4.Runtime;

namespace CqlSharp.Parser;

internal class CqlParserErrorHandler : IAntlrErrorListener<IToken>
{
    public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line,
        int charPositionInLine,
        string msg, RecognitionException e)
    {
        throw new Exception();
    }
}