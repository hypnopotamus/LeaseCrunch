namespace LeaseCrunch.Domain.DataFormatPolicies;

public class PaymentCountIsNumberPolicy : IDataFormatPolicy
{
    public string FailureMessage => "payment count must be a number";
    public bool Validate(LeaseRawDataRow row) => uint.TryParse(row.Payments, out _);
}