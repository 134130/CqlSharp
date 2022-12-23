using CqlSharp.Exceptions;
using CqlSharp.Sql.Queries;

namespace CqlSharp.Parser.Visitor;

using static CqlParser;

internal static class QueryVisitor
{
    public static Query VisitQueryStatement(QueryStatementContext context)
    {
        var child = context.GetChild(0);
        switch (child)
        {
            case SelectStatementContext select:
                return SelectVisitor.VisitSelectStatement(select);

            case InsertStatementContext insert:
                return InsertVisitor.VisitInsertStatement(insert);
        }

        throw new CqlNotSupportedTreeException(child);
    }
}