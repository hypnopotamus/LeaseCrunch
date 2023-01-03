using System.Globalization;
using FluentAssertions;
using LeaseCrunch.Domain;
using LeaseCrunch.Domain.DataFormatPolicies;
using LeaseCrunch.Domain.Errors;
using LeaseCrunch.Storage;

namespace LeaseCrunch.Actions.Tests;

[TestClass]
public class FileImportMediatorTest
{
    private readonly IFileImport _import;

    private const string TestFilePath = nameof(TestFilePath);

    private readonly IReadOnlyList<IDataFormatPolicy> _formatRules
        = new List<IDataFormatPolicy>
        {
            new StartDateFormatPolicy(),
            new EndDateFormatPolicy(),
            new InterestRateIsNumberPolicy(),
            new PaymentAmountIsNumberPolicy(),
            new PaymentCountIsNumberPolicy()
        };

    private readonly FakeFileReader _fileReader = new();

    private readonly FakeLeaseFactory _leaseFactory = new();

    private readonly FakeLeaseRepository _leaseRepository = new();

    public FileImportMediatorTest()
    {
        _import = new FileImportMediator
        (
            _formatRules.ToArray(),
            _leaseFactory,
            path => path == TestFilePath ? _fileReader : new FakeFileReader(),
            _leaseRepository
        );
    }

    [TestMethod]
    public async Task Import_NoFormatErrors_AllRowsAreSaved()
    {
        var rows = RowFactory();
        _fileReader.FileRows[TestFilePath] = rows;

        await _import.Import(TestFilePath);

        var savedRows = (await _leaseRepository.GetLeases()).OrderBy(l => l.Name);
        var leases = rows.Select(_leaseFactory.Create).OrderBy(l => l.Name);
        savedRows.Should().BeEquivalentTo(leases);
    }

    [TestMethod]
    public async Task Import_AnyRowHasInvalidStartDate_NoRowsAreSaved()
    {
        var invalidRow = SingleRowFactory() with { StartDate = Guid.NewGuid().ToString() };
        var rows = RowFactory(invalidRow);
        _fileReader.FileRows[TestFilePath] = rows;

        var formatException = await Assert.ThrowsExceptionAsync<InvalidRowFormatException>(() => _import.Import(TestFilePath));

        Assert.AreEqual(invalidRow, formatException.Row);
        Assert.AreEqual(1, formatException.FailedPolicies.Count());
        Assert.AreEqual(0, (await _leaseRepository.GetLeases()).Count());
    }

    [TestMethod]
    public async Task Import_AnyRowHasInvalidEndDate_NoRowsAreSaved()
    {
        var invalidRow = SingleRowFactory() with { EndDate = Guid.NewGuid().ToString() };
        var rows = RowFactory(invalidRow);
        _fileReader.FileRows[TestFilePath] = rows;

        var formatException = await Assert.ThrowsExceptionAsync<InvalidRowFormatException>(() => _import.Import(TestFilePath));

        Assert.AreEqual(invalidRow, formatException.Row);
        Assert.AreEqual(1, formatException.FailedPolicies.Count());
        Assert.AreEqual(0, (await _leaseRepository.GetLeases()).Count());
    }

    [TestMethod]
    public async Task Import_AnyRowHasInvalidInterestRate_NoRowsAreSaved()
    {
        var invalidRow = SingleRowFactory() with { InterestRate = Guid.NewGuid().ToString() };
        var rows = RowFactory(invalidRow);
        _fileReader.FileRows[TestFilePath] = rows;

        var formatException = await Assert.ThrowsExceptionAsync<InvalidRowFormatException>(() => _import.Import(TestFilePath));

        Assert.AreEqual(invalidRow, formatException.Row);
        Assert.AreEqual(1, formatException.FailedPolicies.Count());
        Assert.AreEqual(0, (await _leaseRepository.GetLeases()).Count());
    }

    [TestMethod]
    public async Task Import_AnyRowHasInvalidPaymentAmount_NoRowsAreSaved()
    {
        var invalidRow = SingleRowFactory() with { PaymentAmount = Guid.NewGuid().ToString() };
        var rows = RowFactory(invalidRow);
        _fileReader.FileRows[TestFilePath] = rows;

        var formatException = await Assert.ThrowsExceptionAsync<InvalidRowFormatException>(() => _import.Import(TestFilePath));

        Assert.AreEqual(invalidRow, formatException.Row);
        Assert.AreEqual(1, formatException.FailedPolicies.Count());
        Assert.AreEqual(0, (await _leaseRepository.GetLeases()).Count());
    }

    [TestMethod]
    public async Task Import_AnyRowHasInvalidPaymentCount_NoRowsAreSaved()
    {
        var invalidRow = SingleRowFactory() with { Payments = Guid.NewGuid().ToString() };
        var rows = RowFactory(invalidRow);
        _fileReader.FileRows[TestFilePath] = rows;

        var formatException = await Assert.ThrowsExceptionAsync<InvalidRowFormatException>(() => _import.Import(TestFilePath));

        Assert.AreEqual(invalidRow, formatException.Row);
        Assert.AreEqual(1, formatException.FailedPolicies.Count());
        Assert.AreEqual(0, (await _leaseRepository.GetLeases()).Count());
    }

    [TestMethod]
    public async Task Import_AnyThrowsInvalidLeaseException_NoRowsAreSaved()
    {
        var rows = RowFactory();
        var invalidRow = rows[_numberSequence.Next(0, rows.Length)];
        _leaseFactory.InvalidLeaseNames.Add(invalidRow.Name);
        _fileReader.FileRows[TestFilePath] = rows;

        var leaseException = await Assert.ThrowsExceptionAsync<InvalidLeaseException>(() => _import.Import(TestFilePath));
        
        Assert.AreEqual(0, (await _leaseRepository.GetLeases()).Count());
    }

    private readonly Random _numberSequence = new(DateTime.Now.Millisecond);

    private LeaseRawDataRow[] RowFactory(params LeaseRawDataRow[] rows) => Enumerable.Repeat(SingleRowFactory, 5)
        .Select(f => f())
        .Concat(rows)
        .ToArray();
    private LeaseRawDataRow SingleRowFactory() => new
    (
        Guid.NewGuid().ToString(),
        DateTime.Now.ToString("MM/dd/yyyy"),
        DateTime.Now.ToString("MM/dd/yyyy"),
        _numberSequence.NextDouble().ToString(CultureInfo.InvariantCulture),
        _numberSequence.Next(0, int.MaxValue).ToString(),
        _numberSequence.NextDouble().ToString(CultureInfo.InvariantCulture)
    );

    private class FakeLeaseFactory : ILeaseFactory
    {
        public ICollection<string> InvalidLeaseNames { get; } = new HashSet<string>();

        public ILease Create(LeaseData data)
        {
            var lease = new FakeLease(data.Name)
            {
                Start = data.StartDate,
                End = data.EndDate,
                InterestRate = data.InterestRate,
                PaymentAmount = data.PaymentAmount,
                PaymentCount = data.Payments
            };

            return !InvalidLeaseNames.Contains(lease.Name)
                ? lease
                : throw new InvalidLeaseException(lease, nameof(ILease.Name), lease.Name);
        }

        public ILease Create(LeaseRawDataRow row)
            => Create
            (
                new LeaseData
                (
                    row.Name,
                    DateOnly.Parse(row.StartDate),
                    DateOnly.Parse(row.EndDate),
                    double.Parse(row.PaymentAmount),
                    uint.Parse(row.Payments),
                    double.Parse(row.InterestRate)
                )
            );

        private record FakeLease(string Name) : ILease
        {
            public DateOnly Start { get; set; }
            public DateOnly End { get; set; }
            public double PaymentAmount { get; set; }
            public uint PaymentCount { get; set; }
            public double InterestRate { get; set; }
        }
    }

    private class FakeFileReader : IFileReader
    {
        public bool IsDisposed { get; private set; } = false;

        public IDictionary<string, IEnumerable<LeaseRawDataRow>> FileRows { get; }
            = new Dictionary<string, IEnumerable<LeaseRawDataRow>>();

        public IEnumerable<LeaseRawDataRow> ReadFile(string path)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(FakeFileReader) + GetHashCode());

            return FileRows.TryGetValue(path, out var rows) ? rows : Enumerable.Empty<LeaseRawDataRow>();
        }

        public void Dispose()
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(FakeFileReader) + GetHashCode());

            IsDisposed = true;
        }
    }

    private class FakeLeaseRepository : ILeaseRepository
    {
        private readonly List<ILease> _pendingLeases = new();
        private readonly List<ILease> _savedLeased = new();

        public Task AddRange(IEnumerable<ILease> leases)
        {
            _pendingLeases.AddRange(leases);

            return Task.CompletedTask;
        }

        public Task Commit()
        {
            _savedLeased.AddRange(_pendingLeases);
            _pendingLeases.Clear();

            return Task.CompletedTask;
        }

        public Task<IEnumerable<ILease>> GetLeases(Sort? sort = null)
            => Task.FromResult<IEnumerable<ILease>>(_savedLeased);
    }
}