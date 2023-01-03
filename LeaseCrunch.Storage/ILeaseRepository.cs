using LeaseCrunch.Domain;

namespace LeaseCrunch.Storage;

public interface ILeaseRepository
{
    Task AddRange(IEnumerable<ILease> leases);
    Task Commit();
    Task<IEnumerable<ILease>> GetLeases(Sort? sort = null);
}