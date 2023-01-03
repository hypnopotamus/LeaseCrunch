using FluentAssertions;
using LeaseCrunch.Domain;

namespace LeaseCrunch.CSVInput.Tests;

[TestClass]
[TestCategory("Integration")]
public class CSVReaderTest
{
    private readonly IFileReader _reader = new CSVReader();

    [TestMethod]
    public void ReadFile_NotEmpty_ReadsAllRowsInFile()
    {
        const string file = $"{nameof(CSVReaderTest)}.{nameof(ReadFile_NotEmpty_ReadsAllRowsInFile)}.csv";
        var rows = Enumerable.Repeat(RowFactory, 5).Select(f => f()).ToArray();
        WriteFile(file, rows);

        var readRows = _reader.ReadFile(file);

        readRows.Should().BeEquivalentTo(rows);
    }

    [TestMethod]
    public void ReadFile_OnlyHeader_ReadsAllRowsInFile()
    {
        const string file = $"{nameof(CSVReaderTest)}.{nameof(ReadFile_OnlyHeader_ReadsAllRowsInFile)}.csv";
        WriteFile(file, Enumerable.Empty<LeaseRawDataRow>());

        var readRows = _reader.ReadFile(file);

        readRows.Should().BeEmpty();
    }

    private static void WriteFile(string path, IEnumerable<LeaseRawDataRow> rows)
        => File.WriteAllLines
        (
            path,
            new[] { "Name,Start Date,End Date,Payment Amount,Number of Payments,Interest Rate" }
                .Concat
                (
                    rows.Select(r => $"{r.Name},{r.StartDate},{r.EndDate},{r.PaymentAmount},{r.Payments},{r.InterestRate}")
                )
        );

    private static LeaseRawDataRow RowFactory() => new
    (
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString()
    );
}