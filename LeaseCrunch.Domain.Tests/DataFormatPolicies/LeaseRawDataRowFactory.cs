namespace LeaseCrunch.Domain.Tests.DataFormatPolicies;

public static class LeaseRawDataRowFactory
{
    public static LeaseRawDataRow Create() => new
    (
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString()
    );
}