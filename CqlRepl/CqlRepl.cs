using System.Diagnostics;
using System.Text;
using CqlSharp;

namespace CqlRepl;

public class CqlRepl
{
    public static async Task Main()
    {
        Console.WriteLine("Welcome to CqlSharp.  Commands end with ;.\n");

        do
        {
            var sql = ReadSql();

            if (sql == "quit")
                break;

            var stopwatch = Stopwatch.StartNew();
            var result = await RunAsync(sql);
            stopwatch.Stop();

            PrintTableAscii(result);

            Console.WriteLine($"{result.Rows.Count()} rows is set ({stopwatch.Elapsed.ToString("%s\\.ffff")} sec)",
                (double)stopwatch.ElapsedMilliseconds / 1000);
        } while (true);

        Console.WriteLine("Bye");
    }

    public static void PrintTableAscii(Table table)
    {
        var maxLengthOfColumns = GetMaxLengthOfColumns(table);

        var sb = new StringBuilder();
        sb.AppendSeperater(maxLengthOfColumns);

        // Column
        sb.Append("|");
        for (var i = 0; i < maxLengthOfColumns.Length; i++)
        {
            var baseLength = maxLengthOfColumns[i];
            var columnLength = table.Columns[i].Name.Length;

            var leftPadding = (baseLength - columnLength) / 2;
            var rightPadding = baseLength - columnLength - leftPadding;

            sb.Append($" {new string(' ', leftPadding)}{table.Columns[i].Name}{new string(' ', rightPadding)} |");
        }

        sb.AppendLine();

        sb.AppendSeperater(maxLengthOfColumns);

        // Rows
        var lineFormatter = GetStringFormatter(maxLengthOfColumns);
        foreach (var row in table.Rows)
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

    private static int[] GetMaxLengthOfColumns(Table table)
    {
        var maxLengthOfColumns = new int[table.Columns.Length];

        for (var i = 0; i < table.Columns.Length; i++)
        {
            maxLengthOfColumns[i] = table.Columns[i].Name.Length;
        }

        foreach (var row in table.Rows)
        {
            for (var i = 0; i < table.Columns.Length; i++)
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

    private static async ValueTask<Table> RunAsync(string sql)
    {
        var parsed = CqlEngine.Parse(sql);
        return await SelectService.ProcessAsync(parsed);
    }
}