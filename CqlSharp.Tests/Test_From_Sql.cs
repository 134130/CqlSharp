using System.Diagnostics;
using Antlr4.Runtime;
using CqlSharp.Expressions;
using CqlSharp.Expressions.Literals;
using CqlSharp.Expressions.Predicate;
using CqlSharp.Query;

namespace CqlSharp.Tests;

public class Test_From_Sql
{
    private static List<string> _queries = new List<string>
    {
        "select id,firstname,lastname,email,profession from \"/Users/cooper/development/CQLSharp/csv/test1.csv\";",
        "select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\";",
        "(select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\");",
        "((select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\"));",
        "select csv.* from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" as csv;",
        "select COUNT(*) from \"/Users/cooper/development/CQLSharp/csv/test1.csv\";",
        "select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" limit 10;",
        "select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" limit 10 offset 10;",
        "select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" where firstname = 'evan';",
        "select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" where firstname like '%an';",
        "select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\" where firstname = 'evan' and lastname = 'choi';",
        "select 'hello';",
        "select 'hello, ' + 'world';",
        "select csv.name from (select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\") csv;",
    };

    [TestCaseSource(nameof(_queries))]
    public async Task Test(string sql)
    {
        var query = CqlEngine.Parse(sql);
        var result = await SelectService.ProcessAsync(query);

        Console.WriteLine($"SQL: {sql}");
        Console.WriteLine($"Columns: {string.Join(",", result.Columns.Select(x => x.Name))}\n");
        foreach (var row in result.Rows)
        {
            Console.WriteLine(string.Join(",", row));
        }
    }
}