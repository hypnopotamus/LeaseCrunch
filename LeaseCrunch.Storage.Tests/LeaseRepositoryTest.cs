using FluentAssertions;
using LeaseCrunch.Domain;

namespace LeaseCrunch.Storage.Tests;

[TestClass]
[TestCategory("Integration")]
public class LeaseRepositoryTest
{
    private readonly ILeaseRepository _repository;

    private readonly LeaseDbContext _db = new();
    private readonly FakeLeaseFactory _factory = new();

    public LeaseRepositoryTest()
    {
        _repository = new LeaseRepository(_db, _factory);
    }

    private static readonly SemaphoreSlim OtherTests = new(1);

    [TestInitialize]
    public void TestInitialize()
    {
        OtherTests.Wait();
        _db.Leases.RemoveRange(_db.Leases);
        _db.SaveChanges();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        OtherTests.Release();
    }

    [TestMethod]
    public async Task AddRangeThenCommitThenGetLeases_NoExceptionInEnumerableSequence_ReturnsEquivalentLeases()
    {
        var leases = Enumerable.Repeat(() => _factory.Create(), 5)
            .Select(f => f().lease)
            .ToArray();

        await _repository.AddRange(leases);
        await _repository.Commit();
        var savedLeases = await _repository.GetLeases();

        savedLeases.Should().BeEquivalentTo(leases);
    }

    [TestMethod]
    public async Task AddRange_ExceptionInEnumerableSequence_ExceptionThrown()
    {
        var enumerableException = new Exception();
        var leases = Enumerable.Repeat(() => _factory.Create(), 5)
            .Select((f, i) => i % 3 != 0 ? f().lease : throw enumerableException);

        var thrownException = await Assert.ThrowsExceptionAsync<Exception>(() => _repository.AddRange(leases));

        Assert.AreSame(enumerableException, thrownException);
    }

    [TestMethod]
    public async Task AddRangeThenGetLeases_NoCommit_DoesNotReturnLeases()
    {
        var leases = Enumerable.Repeat(() => _factory.Create(), 5).Select(f => f().lease).ToArray();

        await _repository.AddRange(leases);
        var savedLeases = await _repository.GetLeases();

        savedLeases.Should().NotIntersectWith(leases);
    }

    [TestMethod]
    public async Task GetLeases_SortedByStartAscending_ReturnsSavedLeasesInOrder()
    {
        var leases = Enumerable.Repeat(() => _factory.Create(), 5).Select(f => f().data).ToArray();
        await _db.Leases.AddRangeAsync(leases);
        var sort = new Sort { Direction = SortDirection.Ascending, Property = SortableProperties.StartDate };

        var sortedLeases = await _repository.GetLeases(sort);

        sortedLeases.Should().BeInAscendingOrder(l => l.Start);
    }

    [TestMethod]
    public async Task GetLeases_SortedByStartDescending_ReturnsSavedLeasesInOrder()
    {
        var leases = Enumerable.Repeat(() => _factory.Create(), 5).Select(f => f().data).ToArray();
        await _db.Leases.AddRangeAsync(leases);
        var sort = new Sort { Direction = SortDirection.Descending, Property = SortableProperties.EndDate };

        var sortedLeases = await _repository.GetLeases(sort);

        sortedLeases.Should().BeInDescendingOrder(l => l.Start);
    }

    [TestMethod]
    public async Task GetLeases_SortedByEndAscending_ReturnsSavedLeasesInOrder()
    {
        var leases = Enumerable.Repeat(() => _factory.Create(), 5).Select(f => f().data).ToArray();
        await _db.Leases.AddRangeAsync(leases);
        var sort = new Sort { Direction = SortDirection.Ascending, Property = SortableProperties.EndDate };

        var sortedLeases = await _repository.GetLeases(sort);

        sortedLeases.Should().BeInAscendingOrder(l => l.End);
    }

    [TestMethod]
    public async Task GetLeases_SortedByEndDescending_ReturnsSavedLeasesInOrder()
    {
        var leases = Enumerable.Repeat(() => _factory.Create(), 5).Select(f => f().data).ToArray();
        await _db.Leases.AddRangeAsync(leases);
        var sort = new Sort { Direction = SortDirection.Descending, Property = SortableProperties.EndDate };

        var sortedLeases = await _repository.GetLeases(sort);

        sortedLeases.Should().BeInDescendingOrder(l => l.End);
    }

    [TestMethod]
    public async Task GetLeases_SortedByUnknownProperty_ThrowsException()
    {
        var unknownProperty = (SortableProperties)(Enum.GetValues<SortableProperties>().GetUpperBound(0) + 1);
        var sort = new Sort { Property = unknownProperty };

        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _repository.GetLeases(sort));
    }

    [TestMethod]
    public async Task GetLeases_SortedByUnknownOrder_ThrowsException()
    {
        var unknownDirection = (SortDirection)(Enum.GetValues<SortDirection>().GetUpperBound(0) + 1);
        var sort = new Sort { Direction = unknownDirection };

        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => _repository.GetLeases(sort));
    }

    private class FakeLeaseFactory : ILeaseFactory
    {
        private readonly Random _dateOffset = new(DateTime.Now.Millisecond);

        public (ILease lease, LeaseData data) Create()
        {
            var data = new LeaseData
            (
                Guid.NewGuid().ToString(),
                DateFactory(),
                DateFactory(),
                DateTime.Now.Millisecond,
                (uint)DateTime.Now.Millisecond,
                DateTime.Now.Millisecond
            );

            return (Create(data), data);
        }

        public ILease Create(LeaseData data)
            => new FakeLease(data.Name)
            {
                Start = data.StartDate,
                End = data.EndDate,
                InterestRate = data.InterestRate,
                PaymentAmount = data.PaymentAmount,
                PaymentCount = data.Payments
            };

        private DateOnly DateFactory() => DateOnly.FromDateTime(DateTime.Now).AddDays(_dateOffset.Next(365));

        private record FakeLease(string Name) : ILease
        {
            public DateOnly Start { get; set; }
            public DateOnly End { get; set; }
            public double PaymentAmount { get; set; }
            public uint PaymentCount { get; set; }
            public double InterestRate { get; set; }
        }
    }
}