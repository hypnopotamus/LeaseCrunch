using LeaseCrunch.Domain.Errors;

namespace LeaseCrunch.Domain.Tests;

[TestClass]
public class LeaseTest
{
    private readonly ILease _lease = new Lease
    (
        LeaseFactoryTest.ValidData.Name,
        LeaseFactoryTest.ValidData.StartDate,
        LeaseFactoryTest.ValidData.EndDate,
        LeaseFactoryTest.ValidData.PaymentAmount,
        LeaseFactoryTest.ValidData.Payments,
        LeaseFactoryTest.ValidData.InterestRate
    );

    [TestMethod]
    public void SetStart_ValueAfterEnd_ThrowsException()
    {
        var start = _lease.End.AddMonths(1);

        var exception = Assert.ThrowsException<InvalidLeaseException>(() => _lease.Start = start);

        Assert.AreSame(_lease, exception.Lease);
        Assert.AreEqual(start, exception.Value);
        Assert.AreEqual(nameof(ILease.Start), exception.Property);
    }

    [TestMethod]
    public void SetStart_ValueBeforeEnd_SetsValue()
    {
        var start = _lease.End.AddMonths(-1);

        _lease.Start = start;

        Assert.AreEqual(start, _lease.Start);
    }

    [TestMethod]
    public void SetStart_ValueEqualToEnd_SetsValue()
    {
        var start = _lease.End;

        _lease.Start = start;

        Assert.AreEqual(start, _lease.Start);
    }

    [TestMethod]
    public void SetEnd_ValueAfterStart_SetsValue()
    {
        var end = _lease.Start.AddMonths(1);

        _lease.End = end;

        Assert.AreEqual(end, _lease.End);
    }

    [TestMethod]
    public void SetEnd_ValueBeforeStart_ThrowsException()
    {
        var end = _lease.Start.AddMonths(-1);

        var exception = Assert.ThrowsException<InvalidLeaseException>(() => _lease.End = end);

        Assert.AreSame(_lease, exception.Lease);
        Assert.AreEqual(end, exception.Value);
        Assert.AreEqual(nameof(ILease.End), exception.Property);
    }

    [TestMethod]
    public void SetEnd_ValueEqualToStart_SetsValue()
    {
        var end = _lease.Start;

        _lease.End = end;

        Assert.AreEqual(end, _lease.End);
    }

    [TestMethod]
    [DataRow(1000000001)]
    [DataRow(-1000000001)]
    public void SetPaymentAmount_ValueOutOfRange_ThrowsException(double paymentAmount)
    {
        var exception = Assert.ThrowsException<InvalidLeaseException>(() => _lease.PaymentAmount = paymentAmount);

        Assert.AreSame(_lease, exception.Lease);
        Assert.AreEqual(paymentAmount, exception.Value);
        Assert.AreEqual(nameof(ILease.PaymentAmount), exception.Property);
    }

    [TestMethod]
    [DataRow(1000000000)]
    [DataRow(-1000000000)]
    public void SetPaymentAmount_ValueInRange_SetsValue(int paymentAmount)
    {
        _lease.PaymentAmount = paymentAmount;

        Assert.AreEqual(paymentAmount, _lease.PaymentAmount);
    }

    [TestMethod]
    public void SetPaymentCount_ValueIsZero_ThrowsException()
    {
        const uint paymentCount = 0;

        var exception = Assert.ThrowsException<InvalidLeaseException>(() => _lease.PaymentCount = paymentCount);

        Assert.AreSame(_lease, exception.Lease);
        Assert.AreEqual(paymentCount, exception.Value);
        Assert.AreEqual(nameof(ILease.PaymentCount), exception.Property);
    }

    [TestMethod]
    public void SetPaymentCount_ValueLessThanTermMonths_ThrowsException()
    {
        var termMonths = DateTime.Now.Millisecond;
        _lease.End = _lease.Start.AddMonths(termMonths);
        var paymentCount = (uint)termMonths + 1;

        var exception = Assert.ThrowsException<InvalidLeaseException>(() => _lease.PaymentCount = paymentCount);

        Assert.AreSame(_lease, exception.Lease);
        Assert.AreEqual(paymentCount, exception.Value);
        Assert.AreEqual(nameof(ILease.PaymentCount), exception.Property);
    }

    [TestMethod]
    public void SetPaymentCount_ValueBetweenZeroAndTermMonths_SetsValue()
    {
        var termMonths = DateTime.Now.Millisecond;
        _lease.End = _lease.Start.AddMonths(termMonths);
        var paymentCount = (uint)termMonths;

        _lease.PaymentCount = paymentCount;

        Assert.AreEqual(paymentCount, _lease.PaymentCount);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(10)]
    public void SetInterestRate_ValueOutOfRange_ThrowsException(double interestRate)
    {
        var exception = Assert.ThrowsException<InvalidLeaseException>(() => _lease.InterestRate = interestRate);

        Assert.AreSame(_lease, exception.Lease);
        Assert.AreEqual(interestRate, exception.Value);
        Assert.AreEqual(nameof(ILease.InterestRate), exception.Property);
    }

    [TestMethod]
    [DataRow(0.0001)]
    [DataRow(9.9999)]
    public void SetInterestRate_ValueBetweenZeroAndTenPercent_SetsValue(double interestRate)
    {
        _lease.InterestRate = interestRate;

        Assert.AreEqual(interestRate, _lease.InterestRate);
    }
}