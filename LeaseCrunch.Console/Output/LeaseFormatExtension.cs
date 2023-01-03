using LeaseCrunch.Domain;

namespace LeaseCrunch.Console.Output;

public static class LeaseFormatExtension
{
    private const string DateFormat = "MM/dd/yyyy";

    public static string Format(this ILease lease) => $"{lease.Name}: Term {lease.Start.ToString(DateFormat)}-{lease.End.ToString(DateFormat)}, Payments: {lease.PaymentCount} of {lease.PaymentAmount:N2}, Interest: {lease.InterestRate:P2}";
}