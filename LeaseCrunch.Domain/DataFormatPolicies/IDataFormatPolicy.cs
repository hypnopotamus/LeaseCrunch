namespace LeaseCrunch.Domain.DataFormatPolicies;

public interface IDataFormatPolicy
{
    public string FailureMessage { get; }

    public bool Validate(LeaseRawDataRow row);
}