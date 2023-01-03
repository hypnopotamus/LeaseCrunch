using System.Globalization;
using System.Text.RegularExpressions;
using LeaseCrunch.Domain;
using static System.Text.RegularExpressions.Regex;

namespace LeaseCrunch.Console.Tests;

[TestClass]
[TestCategory("EndToEnd")]
public class ProgramTest
{
    private readonly Random _numberSequence = new(DateTime.Now.Millisecond);

    [TestMethod]
    public async Task Console_DefaultArgsAndValidFile_CanReadImportAndPrintLeases()
    {
        var error = new StringWriter();
        var output = new StringWriter();
        System.Console.SetError(error);
        System.Console.SetOut(output);
        const string file = $"{nameof(ProgramTest)}.{nameof(Console_DefaultArgsAndValidFile_CanReadImportAndPrintLeases)}.csv";
        var rows = Enumerable.Repeat(RowFactory, 5).Select(f => f()).ToArray();
        WriteFile(file, rows);

        await Run(file);

        var leaseOutputRows = Matches(output.ToString().ReplaceLineEndings("\n"), "^.*: Term \\d{2}/\\d{2}/\\d{4}-\\d{2}/\\d{2}/\\d{4}, Payments: \\d* of -?(\\d{1,3},?)+\\.\\d{2}, Interest: \\d*\\.\\d{2}%$", RegexOptions.Multiline);
        Assert.IsTrue(string.IsNullOrEmpty(error.ToString()), error.ToString());
        Assert.AreEqual(rows.Length, leaseOutputRows.Count, output.ToString());
    }

    private static async Task Run(string file)
    {
        var entry = typeof(Program).Assembly.EntryPoint!;
        var run = entry.Invoke
        (
            null,
            new object[] { new[] { "--file", file } }
        );

        if (run is Task asyncRun) await asyncRun;
    }

    private static void WriteFile(string path, IEnumerable<LeaseRawDataRow> rows)
        => File.WriteAllLines
        (
            path,
            new[] { "Name,Start Date,End Date,Payment Amount,Number of Payments,Interest Rate" }
                .Concat
                (
                    rows.Select
                        (r => $"{r.Name},{r.StartDate},{r.EndDate},{r.PaymentAmount},{r.Payments},{r.InterestRate}")
                )
        );

    private LeaseRawDataRow RowFactory()
    {
        var start = DateTime.Now;
        var end = DateTime.Now.AddMonths(_numberSequence.Next(1, 12));
        int TotalMonths() => ((end.Year - start.Year) * 12) + (end.Month - start.Month);

        double RandomDouble(double lower, double upper) => Math.Clamp
        (
            _numberSequence.Next
            (
                (int)Math.Round(lower, MidpointRounding.AwayFromZero),
                (int)Math.Round(upper, MidpointRounding.AwayFromZero)
            ),
            lower,
            upper
        );
        
        return new LeaseRawDataRow
        (
            Guid.NewGuid().ToString(),
            start.ToString("MM/dd/yyyy"),
            end.ToString("MM/dd/yyyy"),
            RandomDouble(-1000000000, 1000000000).ToString(CultureInfo.InvariantCulture),
            _numberSequence.Next(1, TotalMonths()).ToString(),
            RandomDouble(0.0001, 9.9999).ToString(CultureInfo.InvariantCulture)
        );
    }
}