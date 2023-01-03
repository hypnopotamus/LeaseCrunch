namespace LeaseCrunch.Domain;

public delegate IFileReader FileReaderFactory(string path);

public interface IFileReader : IDisposable
{
    public IEnumerable<LeaseRawDataRow> ReadFile(string path);
}