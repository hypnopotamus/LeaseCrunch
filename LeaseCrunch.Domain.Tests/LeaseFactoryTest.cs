using LeaseCrunch.Domain.Errors;

namespace LeaseCrunch.Domain.Tests;

[TestClass]
public class LeaseFactoryTest
{
    private readonly ILeaseFactory _factory = new LeaseFactory();

    public static readonly LeaseData ValidData = new
    (
        Guid.NewGuid().ToString(),
        DateOnly.FromDateTime(DateTime.Now),
        DateOnly.FromDateTime(DateTime.Now).AddMonths(1),
        1,
        1,
        .1
    );

    [TestMethod]
    public void Create_ValidData_ReturnsLease()
    {
        var lease = _factory.Create(ValidData);

        Assert.IsInstanceOfType(lease, typeof(Lease));
    }

    [TestMethod]
    public void Create_EndDateBeforeStartDate_ThrowsException()
    {
        var data = ValidData with { EndDate = ValidData.StartDate.AddMonths(-1) };

        Assert.ThrowsException<InvalidLeaseException>(() => _factory.Create(data));
    }

    [TestMethod]
    [DataRow(1000000001)]
    [DataRow(-1000000001)]
    public void Create_PaymentAmountOutOfRange_ThrowsException(int paymentAmount)
    {
        var data = ValidData with { PaymentAmount = paymentAmount };

        Assert.ThrowsException<InvalidLeaseException>(() => _factory.Create(data));
    }

    [TestMethod]
    public void Create_PaymentsOutOfRange_ThrowsException()
    {
        var data = ValidData with { Payments = 0 };

        Assert.ThrowsException<InvalidLeaseException>(() => _factory.Create(data));
    }

    [TestMethod]
    public void Create_MorePaymentsThanMonths_ThrowsException()
    {
        var termMonths = DateTime.Now.Millisecond;
        var data = ValidData with
        {
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now).AddMonths(termMonths),
            Payments = (uint)termMonths + 1
        };

        Assert.ThrowsException<InvalidLeaseException>(() => _factory.Create(data));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public void Create_InterestRateOutOfRange_ThrowsException(double interestRate)
    {
        var data = ValidData with { InterestRate = interestRate };

        Assert.ThrowsException<InvalidLeaseException>(() => _factory.Create(data));
    }
}