using System.Diagnostics;
using Antlr4.Runtime;
using CqlSharp.Exceptions;
using CqlSharp.Parser;
using CqlSharp.Parser.Visitor;
using CqlSharp.Sql;
using CqlSharp.Sql.Queries;

namespace CqlSharp;

using static CqlParser;

public static class CqlEngine
{
    public static async ValueTask<QueryResult> ProcessAsync(Query query)
    {
        var stopwatch = Stopwatch.StartNew();
        switch (query)
        {
            case Select select:
                var table = await SelectService.ProcessAsync(select);

                return new SelectResult
                {
                    AffectedRows = table.RowSize,
                    Columns = table.Columns.Select(x => x.Name).ToArray(),
                    Rows = table.Rows,
                    Elapsed = stopwatch.Elapsed
                };
            case Insert insert:
                var affectedRows = await InsertService.ProcessAsync(insert);

                return new InsertResult
                {
                    AffectedRows = affectedRows,
                    Elapsed = stopwatch.Elapsed
                };
            default:
                throw new ArgumentOutOfRangeException(nameof(query), query, null);
        }
    }

    public static Query Parse(string sql)
    {
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

        return QueryVisitor.VisitQueryStatement(queryStatement);
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