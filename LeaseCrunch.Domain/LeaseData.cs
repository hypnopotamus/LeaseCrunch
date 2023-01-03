namespace LeaseCrunch.Domain;

public record LeaseData(string Name, DateOnly StartDate, DateOnly EndDate, double PaymentAmount, uint Payments, double InterestRate);