namespace LeaseCrunch.Domain;

public class LeaseFactory : ILeaseFactory
{
    public ILease Create(LeaseData data)
    {
        return new Lease
        (
            data.Name,
            data.StartDate,
            data.EndDate,
            data.PaymentAmount,
            data.Payments,
            data.InterestRate
        );
    }
}