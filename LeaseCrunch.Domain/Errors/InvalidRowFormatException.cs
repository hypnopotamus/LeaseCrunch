using LeaseCrunch.Domain.DataFormatPolicies;

namespace LeaseCrunch.Domain.Errors;

public class InvalidRowFormatException : Exception
{
    public LeaseRawDataRow Row { get; }
    public IEnumerable<IDataFormatPolicy> FailedPolicies { get; }

    public InvalidRowFormatException(LeaseRawDataRow row, IEnumerable<IDataFormatPolicy> failedPolicies)
    {
        Row = row;
        FailedPolicies = failedPolicies;
    }
}