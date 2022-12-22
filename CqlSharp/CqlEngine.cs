using Antlr4.Runtime;
using CqlSharp.Parser;
using CqlSharp.Parser.Visitor;
using CqlSharp.Query;

namespace CqlSharp;

using static CqlParser;

public class CqlEngine
{
    public static Select Parse(string sql)
    {
        Console.WriteLine(sql);
        var parser = CreateParser(sql);
        var context = parser.query();

        if (context.GetChild(0) is not QueryStatementContext queryStatement)
            return null;

        if (queryStatement.GetChild(0) is not SelectStatementContext selectStatement)
            return null;

        return SelectVisitor.VisitSelectStatement(selectStatement);
    }

    private static CqlParser CreateParser(string input)
    {
        var inputStream = new AntlrInputStream(input);
        var lexer = new CqlLexer(inputStream);

        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(new CQLLexerErrorHandler());

        var tokenStream = new CommonTokenStream(lexer);
        var parser = new CqlParser(tokenStream);

        parser.RemoveErrorListeners();
        parser.AddErrorListener(new CQLParserErrorHandler());

        return parser;
    }
}