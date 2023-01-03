using LeaseCrunch.Console.Output;
using LeaseCrunch.Domain;

namespace LeaseCrunch.Console.Tests.Output;

[TestClass]
public class LeaseFormatExtensionTest
{
    [TestMethod]
    public void Format_AllCases_ContainsName()
    {
        var lease = new FakeLease();

        var output = lease.Format();

        StringAssert.Contains(output, lease.Name);
    }

    [TestMethod]
    public void Format_AllCases_ContainsStart()
    {
        var lease = new FakeLease();

        var output = lease.Format();

        StringAssert.Contains(output, lease.Start.ToString("MM/dd/yyyy"));
    }

    [TestMethod]
    public void Format_AllCases_ContainsEnd()
    {
        var lease = new FakeLease();

        var output = lease.Format();

        StringAssert.Contains(output, lease.End.ToString("MM/dd/yyyy"));
    }

    [TestMethod]
    public void Format_AllCases_ContainsPaymentAmount()
    {
        var lease = new FakeLease();

        var output = lease.Format();

        StringAssert.Contains(output, lease.PaymentAmount.ToString("N2"));
    }

    [TestMethod]
    public void Format_AllCases_ContainsPaymentCount()
    {
        var lease = new FakeLease();

        var output = lease.Format();

        StringAssert.Contains(output, lease.PaymentCount.ToString());
    }

    [TestMethod]
    public void Format_AllCases_ContainsInterestRate()
    {
        var lease = new FakeLease();

        var output = lease.Format();

        StringAssert.Contains(output, lease.InterestRate.ToString("P2"));
    }

    private class FakeLease : ILease
    {
        public string Name { get; } = Guid.NewGuid().ToString();
        public DateOnly Start { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly End { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));
        public double PaymentAmount { get; set; } = DateTime.Now.Millisecond;
        public uint PaymentCount { get; set; } = (uint)DateTime.Now.Millisecond + 1;
        public double InterestRate { get; set; } = DateTime.Now.Millisecond + 2;
    }
}