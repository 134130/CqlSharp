using System.Globalization;
using System.Runtime.CompilerServices;
using CqlSharp.Expressions;
using CsvHelper;

namespace CqlSharp;

internal class CsvFile : ITable, IDisposable
{
    public string? Alias { get; }
    public QualifiedIdentifier[] Columns { get; }

    private readonly StreamReader _streamReader;
    private readonly CsvReader _csvReader;

    public CsvFile(string filePath, string alias = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException(filePath);

        _streamReader = new StreamReader(filePath);
        _csvReader = new CsvReader(_streamReader, CultureInfo.InvariantCulture);

        if (!_csvReader.Read() || !_csvReader.ReadHeader())
            throw new InvalidDataException();

        if (_csvReader.HeaderRecord is { } headerRecord)
            Columns = headerRecord.Select(x => new QualifiedIdentifier(x)).ToArray();

        Alias = alias;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<bool> ReadAsync()
    {
        return _csvReader.ReadAsync();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string[] FetchRow()
    {
        return FetchRow(Enumerable.Range(0, Columns.Length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string[] FetchRow(IEnumerable<int> columnIndexes)
    {
        return columnIndexes.Select(x =>
        {
            if (!_csvReader.TryGetField<string>(x, out var data))
                throw new InvalidDataException();

            if (data is null)
                throw new InvalidDataException();

            return data;
        }).ToArray();
    }

    public void Dispose()
    {
        _csvReader.Dispose();
        _streamReader.Dispose();
    }
}