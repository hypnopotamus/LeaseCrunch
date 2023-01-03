namespace LeaseCrunch.Domain.Errors;

public class InvalidLeaseException : Exception
{
    public ILease Lease { get; }
    public string Property { get; }
    public object Value { get; }

    public InvalidLeaseException(ILease lease, string property, object value)
    {
        Lease = lease;
        Property = property;
        Value = value;
    }
}