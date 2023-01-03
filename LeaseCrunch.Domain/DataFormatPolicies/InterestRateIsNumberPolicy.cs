namespace LeaseCrunch.Domain.DataFormatPolicies;

public class InterestRateIsNumberPolicy : IDataFormatPolicy
{
    public string FailureMessage => "interest rate must be a decimal";
    public bool Validate(LeaseRawDataRow row) => double.TryParse(row.InterestRate, out _);
}