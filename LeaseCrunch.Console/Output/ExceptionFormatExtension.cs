using System.Text.Json;
using LeaseCrunch.Domain.Errors;

namespace LeaseCrunch.Console.Output;

public static class ExceptionFormatExtension
{
    //todo more nicely format the data validation exceptions for a user to read
    public static string Format(this InvalidLeaseException exception) => $"error in Lease {exception.Lease.Name}:{exception.Property}, {JsonSerializer.Serialize(exception.Value)}";
    public static string Format(this InvalidRowFormatException exception) => @$"error importing row {exception.Row.Name}, {exception.Row.StartDate}, {exception.Row.EndDate}, {exception.Row.PaymentAmount}, {exception.Row.Payments}, {exception.Row.InterestRate}
Failures: {string.Join(',', exception.FailedPolicies.Select(p => p.FailureMessage))}";
}