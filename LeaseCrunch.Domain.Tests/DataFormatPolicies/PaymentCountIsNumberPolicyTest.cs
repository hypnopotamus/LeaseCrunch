using LeaseCrunch.Domain.DataFormatPolicies;
using System.Globalization;

namespace LeaseCrunch.Domain.Tests.DataFormatPolicies;

[TestClass]
public class PaymentCountIsNumberPolicyTest
{
    private readonly IDataFormatPolicy _policy = new PaymentCountIsNumberPolicy();

    [TestMethod]
    public void Validate_InvalidFormats_ReturnsFalse()
    {
        var data = LeaseRawDataRowFactory.Create() with { Payments = Guid.NewGuid().ToString() };

        var valid = _policy.Validate(data);

        Assert.IsFalse(valid);
    }

    [TestMethod]
    public void Validate_ValidFormat_ReturnsTrue()
    {
        var data = LeaseRawDataRowFactory.Create() with { Payments = new Random(DateTime.Now.Millisecond).Next(0, int.MaxValue).ToString(CultureInfo.InvariantCulture) };

        var valid = _policy.Validate(data);

        Assert.IsTrue(valid);
    }
}