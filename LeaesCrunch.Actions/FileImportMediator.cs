using LeaseCrunch.Domain;
using LeaseCrunch.Domain.DataFormatPolicies;
using LeaseCrunch.Domain.Errors;
using LeaseCrunch.Storage;

namespace LeaseCrunch.Actions;

public class FileImportMediator : IFileImport
{
    private readonly IDataFormatPolicy[] _formatPolicies;
    private readonly ILeaseFactory _leaseFactory;
    private readonly FileReaderFactory _readerFactory;
    private readonly ILeaseRepository _repository;

    public FileImportMediator
    (
        IDataFormatPolicy[] formatPolicies,
        ILeaseFactory leaseFactory,
        FileReaderFactory readerFactory,
        ILeaseRepository repository
    )
    {
        _formatPolicies = formatPolicies;
        _leaseFactory = leaseFactory;
        _readerFactory = readerFactory;
        _repository = repository;
    }

    public async Task Import(string filePath)
    {
        using var reader = _readerFactory(filePath);

        await _repository.AddRange(ImportRows(reader.ReadFile(filePath)));
        await _repository.Commit();
    }

    private IEnumerable<ILease> ImportRows(IEnumerable<LeaseRawDataRow> rows)
    {
        foreach (var row in rows)
        {
            var failedFormatPolicies = _formatPolicies.Where(p => !p.Validate(row)).ToArray();
            if (failedFormatPolicies.Any()) throw new InvalidRowFormatException(row, failedFormatPolicies);

            var data = new LeaseData
            (
                row.Name,
                DateOnly.Parse(row.StartDate),
                DateOnly.Parse(row.EndDate),
                double.Parse(row.PaymentAmount),
                uint.Parse(row.Payments),
                double.Parse(row.InterestRate)
            );

            yield return _leaseFactory.Create(data);
        }
    }
}