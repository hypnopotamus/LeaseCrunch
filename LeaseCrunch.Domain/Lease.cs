using LeaseCrunch.Domain.Errors;

namespace LeaseCrunch.Domain;

public class Lease : ILease
{
    public string Name { get; }

    public DateOnly Start
    {
        get => _start;
        set
        {
            if (value > End) throw new InvalidLeaseException(this, nameof(Start), value);

            _start = value;
        }
    }

    public DateOnly End
    {
        get => _end;
        set
        {
            if (value < Start) throw new InvalidLeaseException(this, nameof(End), value);

            _end = value;
        }
    }

    public double PaymentAmount
    {
        get => _paymentAmount;
        set
        {
            if (value is > 1000000000 or < -1000000000) throw new InvalidLeaseException(this, nameof(PaymentAmount), value);
            
            _paymentAmount = double.Round(value, MidpointRounding.ToZero);
        }
    }
    public uint PaymentCount
    {
        get => _paymentCount;
        set
        {
            if(value <= 0 || value > TermTotalMonths) throw new InvalidLeaseException(this, nameof(PaymentCount), value);

            _paymentCount = value;
        }
    }
    public double InterestRate
    {
        get => _interestRate;
        set
        {
            if(value is <= 0 or > 9.9999) throw new InvalidLeaseException(this, nameof(InterestRate), value);

            _interestRate = value;
        }
    }

    private int TermTotalMonths => ((End.Year - Start.Year) * 12) + (End.Month - Start.Month);

    private DateOnly _start;
    private DateOnly _end;
    private double _paymentAmount;
    private uint _paymentCount;
    private double _interestRate;

    public Lease
        (string name, DateOnly start, DateOnly end, double paymentAmount, uint paymentCount, double interestRate)
    {
        Name = name;
        _start = start;
        _end = end;
        Start = start;
        End = end;
        PaymentAmount = paymentAmount;
        PaymentCount = paymentCount;
        InterestRate = interestRate;
    }
}