namespace LeaseCrunch.Domain;

public interface ILeaseFactory
{
    ILease Create(LeaseData data);
}