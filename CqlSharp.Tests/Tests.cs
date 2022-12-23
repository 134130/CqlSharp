using System.Text;
using CqlRepl;
using CqlSharp.Sql.Tables;
using FluentAssertions;

namespace CqlSharp.Tests;

public class Test_From_Sql
{
    [TestCase(
        "select id,firstname,lastname,email,profession from \"/Users/cooper/development/CQLSharp/csv/test1.csv\";")]
    [TestCase("select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\";")]
    [TestCase("(select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\");")]
    [TestCase("((select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\"));")]
    [TestCase("select csv.* from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" as csv;")]
    [TestCase("select csv.firstname from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" as csv;")]
    [TestCase("select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" limit 10;")]
    [TestCase("select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" limit 10 offset 10;")]
    [TestCase("select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" where lastname = 'choi';")]
    [TestCase(
        "select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" where firstname = 'evan' and lastname = 'choi';")]
    [TestCase("select csv.firstname from (select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\") csv;")]
    [TestCase(
        "select csv.name from (select firstname as name from \"/Users/cooper/development/CQLSharp/csv/test1.csv\") csv;")]
    [TestCase(
        "select csv.name as name from (select firstname as name from \"/Users/cooper/development/CQLSharp/csv/test1.csv\") csv;")]
    [TestCase("select 'hello';")]
    [TestCase("select 'hello, ' + 'world';")]
    [TestCase("select COUNT(*) from \"/Users/cooper/development/CQLSharp/csv/test1.csv\";")]
    [TestCase("select *, true from \"/Users/cooper/development/CQLSharp/csv/test1.csv\";")]
    [TestCase("select *, true and true as a from \"/Users/cooper/development/CQLSharp/csv/test1.csv\";")]
    public async Task Test(string sql)
    {
        var query = CqlEngine.Parse(sql);
        var result = await CqlEngine.ProcessAsync(query);

        await Verify(AsciiTable(sql, result)).UseDirectory("verified");
    }

    [TestCase("select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" where firstname like '%an';")]
    [TestCase(
        "select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" where firstname in ('evan', 'tony', 'wein');")]
    [TestCase(
        "select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" where id between '500' and '600';")]
    [TestCase("select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" where firstname regexp '^[aA].+';")]
    public async Task Test_Predicate_Operations(string sql)
    {
        var query = CqlEngine.Parse(sql);
        var result = await CqlEngine.ProcessAsync(query);

        await Verify(AsciiTable(sql, result)).UseDirectory("verified");
    }

    private static string AsciiTable(string sql, Table table)
    {
        var maxLengthOfColumns = GetMaxLengthOfColumns(table);

        var sb = new StringBuilder();
        sb.AppendLine(sql);
        sb.AppendLine();

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

        return sb.ToString();
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
}