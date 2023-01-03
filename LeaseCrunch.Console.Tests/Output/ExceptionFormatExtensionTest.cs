using System.Globalization;
using System.Text.Json;
using LeaseCrunch.Console.Output;
using LeaseCrunch.Domain;
using LeaseCrunch.Domain.DataFormatPolicies;
using LeaseCrunch.Domain.Errors;

namespace LeaseCrunch.Console.Tests.Output;

[TestClass]
public class ExceptionFormatExtensionTest
{
    [TestMethod]
    public void Format_InvalidLease_ContainsLeaseName()
    {
        var exception = new InvalidLeaseException(new FakeLease(), nameof(ILease.Name), Guid.NewGuid().ToString());

        var output = exception.Format();

        StringAssert.Contains(output, exception.Lease.Name);
    }

    [TestMethod]
    public void Format_InvalidLease_ContainsProperty()
    {
        var exception = new InvalidLeaseException(new FakeLease(), nameof(ILease.Name), Guid.NewGuid().ToString());

        var output = exception.Format();

        StringAssert.Contains(output, exception.Property);
    }

    [TestMethod]
    public void Format_InvalidLease_ContainsValue()
    {
        var value = new
        {
            a = "b",
            c = new
            {
                d = 'e',
                f = 1
            }
        };
        var exception = new InvalidLeaseException(new FakeLease(), nameof(ILease.Name), value);

        var output = exception.Format();

        StringAssert.Contains(output, JsonSerializer.Serialize(value));
    }

    [TestMethod]
    public void Format_InvalidRow_ContainsAllRowData()
    {
        var exception = new InvalidRowFormatException(RowFactory(), Enumerable.Empty<IDataFormatPolicy>());

        var output = exception.Format();

        foreach (var property in typeof(LeaseRawDataRow).GetProperties())
        {
            var value = property.GetValue(exception.Row) as string;
            StringAssert.Contains(output, value);
        }
    }

    [TestMethod]
    public void Format_InvalidRow_ContainsAllFailureMessages()
    {
        var policies = Enumerable.Repeat(() => new FakeDataFormatPolicy(), 5).Select(f => f()).ToArray();
        var exception = new InvalidRowFormatException(RowFactory(), policies);

        var output = exception.Format();

        foreach (var policy in policies)
        {
            StringAssert.Contains(output, policy.FailureMessage);
        }
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

    private static LeaseRawDataRow RowFactory() => new 
    (
        Guid.NewGuid().ToString(),
        DateTime.Now.ToString(CultureInfo.InvariantCulture),
        DateTime.Now.ToString(CultureInfo.InvariantCulture),
        DateTime.Now.Millisecond.ToString(),
        DateTime.Now.Millisecond.ToString(),
        DateTime.Now.Millisecond.ToString()
    );

    private class FakeDataFormatPolicy : IDataFormatPolicy
    {
        public string FailureMessage { get; } = Guid.NewGuid().ToString();
        public bool Validate(LeaseRawDataRow row)
        {
            throw new NotImplementedException();
        }
    }
}