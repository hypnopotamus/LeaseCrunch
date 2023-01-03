using LeaseCrunch.Domain;
using Microsoft.VisualBasic.FileIO;

namespace LeaseCrunch.CSVInput;

public class CSVReader : IFileReader
{
    private readonly IList<IDisposable> _openParsers = new List<IDisposable>();

    public IEnumerable<LeaseRawDataRow> ReadFile(string path)
    {
        var parser = new TextFieldParser(path)
        {
            TextFieldType = FieldType.Delimited,
            Delimiters = new []{ "," }
        };
        _openParsers.Add(parser);

        if (parser.EndOfData) yield break;
        parser.ReadFields();

        while (!parser.EndOfData) 
        {
            var fields = parser.ReadFields();
            if (fields?.Length != 6) throw new InvalidDataException();

            yield return new LeaseRawDataRow(fields[0], fields[1], fields[2], fields[3], fields[4], fields[5]);
        }
    }

    public void Dispose()
    {
        foreach (var parser in _openParsers)
        {
            parser.Dispose();
        }
    }
}