namespace LeaseCrunch.Domain.DataFormatPolicies;

public class EndDateFormatPolicy : DateFormatPolicy, IDataFormatPolicy
{
    public string FailureMessage => "end date must be formatted MM/dd/yyyy";
    public bool Validate(LeaseRawDataRow row) => DateFormatValid(row.EndDate);
}