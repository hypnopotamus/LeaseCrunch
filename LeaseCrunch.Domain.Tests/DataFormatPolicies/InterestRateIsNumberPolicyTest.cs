using System.Globalization;
using LeaseCrunch.Domain.DataFormatPolicies;

namespace LeaseCrunch.Domain.Tests.DataFormatPolicies;

[TestClass]
public class InterestRateIsNumberPolicyTest
{
    private readonly IDataFormatPolicy _policy = new InterestRateIsNumberPolicy();

    [TestMethod]
    public void Validate_InvalidFormats_ReturnsFalse()
    {
        var data = LeaseRawDataRowFactory.Create() with { InterestRate = Guid.NewGuid().ToString() };

        var valid = _policy.Validate(data);

        Assert.IsFalse(valid);
    }

    [TestMethod]
    public void Validate_ValidFormat_ReturnsTrue()
    {
        var data = LeaseRawDataRowFactory.Create() with { InterestRate = new Random(DateTime.Now.Millisecond).NextDouble().ToString(CultureInfo.InvariantCulture) };

        var valid = _policy.Validate(data);

        Assert.IsTrue(valid);
    }
}