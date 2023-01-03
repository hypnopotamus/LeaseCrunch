namespace LeaseCrunch.Domain;

public record LeaseRawDataRow(string Name, string StartDate, string EndDate, string PaymentAmount, string Payments, string InterestRate);