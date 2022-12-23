using System.Diagnostics;
using System.Text;
using CqlSharp;
using CqlSharp.Exceptions;
using CqlSharp.Sql.Tables;

namespace CqlRepl;

public static class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Welcome to CqlSharp.  Commands end with ;.\n");

        do
        {
            var sql = ReadSql();

            if (sql == "quit")
                break;

            try
            {
                var result = await RunAsync(sql);

                if (result is SelectResult selectResult)
                {
                    PrintTableAscii(selectResult);
                    Console.WriteLine("{0} row{1} is set ({2:%s\\.ffff} sec)",
                        result.AffectedRows, result.AffectedRows > 1 ? "s" : "", result.Elapsed);
                }

                if (result is InsertResult insertResult)
                {
                    Console.WriteLine("{0} row{1} affected ({2:%s\\.ffff} sec)",
                        result.AffectedRows, result.AffectedRows > 1 ? "s" : "", result.Elapsed);
                }
            }
            catch (CqlSharpException e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }
        } while (true);

        Console.WriteLine("Bye");
    }

    public static void PrintTableAscii(SelectResult result)
    {
        var maxLengthOfColumns = GetMaxLengthOfColumns(result);

        var sb = new StringBuilder();
        sb.AppendSeperater(maxLengthOfColumns);

        // Column
        sb.Append("|");
        for (var i = 0; i < maxLengthOfColumns.Length; i++)
        {
            var baseLength = maxLengthOfColumns[i];
            var columnLength = result.Columns[i].Length;

            var leftPadding = (baseLength - columnLength) / 2;
            var rightPadding = baseLength - columnLength - leftPadding;

            sb.Append($" {new string(' ', leftPadding)}{result.Columns[i]}{new string(' ', rightPadding)} |");
        }

        sb.AppendLine();

        sb.AppendSeperater(maxLengthOfColumns);

        // Rows
        var lineFormatter = GetStringFormatter(maxLengthOfColumns);
        foreach (var row in result.Rows)
        {
            sb.AppendFormat(lineFormatter, row);
            sb.AppendLine();
        }

        sb.AppendSeperater(maxLengthOfColumns);

        Console.WriteLine(sb.ToString());
    }

    private static string GetStringFormatter(IReadOnlyList<int> maxLengthOfColumns)
    {
        var formatSb = new StringBuilder();
        formatSb.Append("|");
        for (var i = 0; i < maxLengthOfColumns.Count; i++)
        {
            formatSb.Append($" {{{i}, -{maxLengthOfColumns[i]}}} ");
            formatSb.Append("|");
        }

        return formatSb.ToString();
    }

    private static int[] GetMaxLengthOfColumns(SelectResult result)
    {
        var maxLengthOfColumns = new int[result.Columns.Length];

        for (var i = 0; i < result.Columns.Length; i++)
        {
            maxLengthOfColumns[i] = result.Columns[i].Length;
        }

        foreach (var row in result.Rows)
        {
            for (var i = 0; i < result.Columns.Length; i++)
            {
                maxLengthOfColumns[i] = Math.Max(maxLengthOfColumns[i], row[i].Length);
            }
        }

        return maxLengthOfColumns;
    }

    private static string ReadSql(string scope = "CqlSharp")
    {
        Console.Write($"{scope}> ");
        var line = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(line) || !line.Contains(';'))
            line += ReadSql("       -");

        return line;
    }

    private static async ValueTask<QueryResult> RunAsync(string sql)
    {
        var parsed = CqlEngine.Parse(sql);
        return await CqlEngine.ProcessAsync(parsed);
    }
}