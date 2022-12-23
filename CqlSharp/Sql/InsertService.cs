using System.Collections.Immutable;
using System.Globalization;
using CqlSharp.Exceptions;
using CqlSharp.Extension;
using CqlSharp.Sql.Queries;
using CqlSharp.Sql.Tables;
using CsvHelper;
using CsvHelper.Configuration;

namespace CqlSharp.Sql;

internal static class InsertService
{
    public static async ValueTask<int> ProcessAsync(Insert insertQuery)
    {
        var referenceTable = await GetReferenceTable(insertQuery);

        var isAppending = File.Exists(insertQuery.DestinationFilePath);

        if (!isAppending)
            await File.Create(insertQuery.DestinationFilePath).DisposeAsync();

        switch (isAppending)
        {
            case false:
            {
                await using var fileStream = File.Open(insertQuery.DestinationFilePath, FileMode.Append);
                await using var writeStream = new StreamWriter(fileStream);
                await using var csvWriter = new CsvWriter(writeStream,
                    new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true
                    });

                foreach (var column in referenceTable.Columns)
                {
                    csvWriter.WriteField(column.Name);
                }

                await csvWriter.NextRecordAsync();

                foreach (var row in referenceTable.Rows)
                {
                    foreach (var column in row)
                    {
                        csvWriter.WriteField(column);
                    }

                    await csvWriter.NextRecordAsync();
                }

                break;
            }
            case true:
            {
                CsvFile csv;
                try
                {
                    csv = new CsvFile(insertQuery.DestinationFilePath);
                }
                catch (Exception e)
                {
                    await Console.Error.WriteLineAsync(e.Message);
                    throw new CqlSharpException($"Failed to open csv file: {insertQuery.DestinationFilePath}");
                }

                var csvHeader = csv.Columns;
                csv.Dispose();

                await using var fileStream = File.Open(insertQuery.DestinationFilePath, FileMode.Append);
                await using var writeStream = new StreamWriter(fileStream);
                await using var csvWriter = new CsvWriter(writeStream,
                    new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true
                    });

                var csvNotContainingColumn =
                    referenceTable.Columns.FirstOrDefault(x => csvHeader.All(csvColumn => x != csvColumn));

                if (csvNotContainingColumn is not null)
                    throw new ColumnNotfoundException(csvNotContainingColumn);

                var indexedColumns = csvHeader.Select(x => (x, referenceTable.Columns.IndexOf(x))).ToImmutableArray();

                foreach (var row in referenceTable.Rows)
                {
                    foreach (var indexedColumn in indexedColumns)
                    {
                        if (indexedColumn.Item2 < 0)
                        {
                            csvWriter.WriteField("");
                            continue;
                        }

                        csvWriter.WriteField(row[indexedColumn.Item2]);
                    }
                }

                break;
            }
        }

        return referenceTable.RowSize;
    }

    private static async ValueTask<Table> GetReferenceTable(Insert insertQuery)
    {
        if (insertQuery.SubQueryTableReference is { } subQueryRef)
            return await SelectService.ProcessAsync(subQueryRef.SubQuery);

        if (insertQuery.ReferenceTable is { })
            return insertQuery.ReferenceTable;

        throw new InvalidOperationException();
    }
}