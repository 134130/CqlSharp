using Antlr4.Runtime;
using CqlSharp.Exceptions;
using CqlSharp.Parser;
using CqlSharp.Parser.Visitor;
using CqlSharp.Sql;
using CqlSharp.Sql.Query;
using CqlSharp.Sql.Tables;

namespace CqlSharp;

using static CqlParser;

public static class CqlEngine
{
    public static async ValueTask<Table> ProcessAsync(Select select)
    {
        return await SelectService.ProcessAsync(select);
    }

    public static Select Parse(string sql)
    {
        Console.WriteLine(sql);

        QueryContext context;
        try
        {
            var parser = CreateParser(sql);
            context = parser.query();
        }
        catch (Exception)
        {
            throw new CqlSharpException($"Failed to parse sql: {sql}");
        }

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
        lexer.AddErrorListener(new CqlLexerErrorHandler());

        var tokenStream = new CommonTokenStream(lexer);
        var parser = new CqlParser(tokenStream);

        parser.RemoveErrorListeners();
        parser.AddErrorListener(new CqlParserErrorHandler());

        return parser;
    }
}