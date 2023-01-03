using LeaseCrunch.Domain.DataFormatPolicies;
using System.Globalization;

namespace LeaseCrunch.Domain.Tests.DataFormatPolicies;

[TestClass]
public class PaymentAmountIsNumberPolicyTest
{
    private readonly IDataFormatPolicy _policy = new PaymentAmountIsNumberPolicy();

    [TestMethod]
    public void Validate_InvalidFormats_ReturnsFalse()
    {
        var data = LeaseRawDataRowFactory.Create() with { PaymentAmount = Guid.NewGuid().ToString() };

        var valid = _policy.Validate(data);

        Assert.IsFalse(valid);
    }

    [TestMethod]
    public void Validate_ValidFormat_ReturnsTrue()
    {
        var data = LeaseRawDataRowFactory.Create() with { PaymentAmount = new Random(DateTime.Now.Millisecond).NextDouble().ToString(CultureInfo.InvariantCulture) };

        var valid = _policy.Validate(data);

        Assert.IsTrue(valid);
    }
}