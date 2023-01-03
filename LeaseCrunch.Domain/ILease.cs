namespace LeaseCrunch.Domain;

public interface ILease
{
    public string Name { get; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public double PaymentAmount { get; set; }
    public uint PaymentCount { get; set; }
    public double InterestRate { get; set; }
}