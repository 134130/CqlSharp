using CqlSharp;

namespace CqlRepl;

public class CqlRepl
{
    public static async Task Main()
    {
        // var config = new ManualConfig()
        //     .WithOptions(ConfigOptions.DisableOptimizationsValidator)
        //     .AddValidator(JitOptimizationsValidator.DontFailOnError)
        //     .AddLogger(ConsoleLogger.Default)
        //     .AddColumnProvider(DefaultColumnProviders.Instance);
        //
        // BenchmarkRunner.Run<BenchmarkTest>(config);

        var parsed =
            CqlEngine.Parse(
                "SELECT csv.id FROM \"/Users/cooper/development/CQLSharp/csv/test1.csv\" AS csv LIMIT 5");
        //        "select csv.name from (select * from \"/Users/cooper/development/CQLSharp/csv/test1.csv\") csv;)");

        var result = await SelectService.ProcessAsync(parsed);

        Console.WriteLine(string.Join(",", result.Columns.Select(x => x.Name)));

        foreach (var row in result.Rows)
        {
            Console.WriteLine(string.Join(",", row));
        }

        // var query =
//            "SELECT A.* FROM (SELECT csv.firstname, csv.lastname FROM \"/Users/cooper/development/CQLSharp/csv/test1.csv\" AS csv) AS A";

        //      CqlEngine.Query(query);
    }
}