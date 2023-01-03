using LeaseCrunch.Domain;
using Microsoft.EntityFrameworkCore;

namespace LeaseCrunch.Storage;

public class LeaseRepository : ILeaseRepository
{
    private readonly LeaseDbContext _dbContext;
    private readonly ILeaseFactory _factory;

    public LeaseRepository(LeaseDbContext dbContext, ILeaseFactory factory)
    {
        _dbContext = dbContext;
        _factory = factory;
    }

    public Task AddRange(IEnumerable<ILease> leases)
    {
        var leaseRecords = leases.Select(l => new LeaseData(l.Name, l.Start, l.End, l.PaymentAmount, l.PaymentCount, l.InterestRate));

        return _dbContext.Leases.AddRangeAsync(leaseRecords);
    }

    public Task Commit()
    {
        return _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<ILease>> GetLeases(Sort? sort = null)
    {
        var records = (sort ?? new Sort()) switch
        {
            { Property: SortableProperties.StartDate, Direction: SortDirection.Descending } => _dbContext.Leases.OrderByDescending(l => l.StartDate),
            { Property: SortableProperties.StartDate, Direction: SortDirection.Ascending } => _dbContext.Leases.OrderBy(l => l.StartDate),
            { Property: SortableProperties.EndDate, Direction: SortDirection.Descending } => _dbContext.Leases.OrderByDescending(l => l.EndDate),
            { Property: SortableProperties.EndDate, Direction: SortDirection.Ascending } => _dbContext.Leases.OrderBy(l => l.EndDate),
            _ => throw new ArgumentOutOfRangeException()
        };

        return (await records.ToArrayAsync()).Select(_factory.Create);
    }
}