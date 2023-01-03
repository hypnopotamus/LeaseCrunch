namespace LeaseCrunch.Domain.DataFormatPolicies;

public class PaymentAmountIsNumberPolicy : IDataFormatPolicy
{
    public string FailureMessage => "payment amount must be a number";
    public bool Validate(LeaseRawDataRow row) => double.TryParse(row.PaymentAmount, out _);
}