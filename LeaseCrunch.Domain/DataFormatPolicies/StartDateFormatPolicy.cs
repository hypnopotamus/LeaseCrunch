namespace LeaseCrunch.Domain.DataFormatPolicies;

public class StartDateFormatPolicy : DateFormatPolicy, IDataFormatPolicy
{
    public string FailureMessage => "start date must be formatted MM/dd/yyyy";
    public bool Validate(LeaseRawDataRow row) => DateFormatValid(row.StartDate);
}