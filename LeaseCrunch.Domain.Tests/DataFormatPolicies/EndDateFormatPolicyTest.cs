using LeaseCrunch.Domain.DataFormatPolicies;

namespace LeaseCrunch.Domain.Tests.DataFormatPolicies;

[TestClass]
public class EndDateFormatPolicyTest : DateFormatPolicyTest
{
    private readonly IDataFormatPolicy _policy = new EndDateFormatPolicy();

    [TestMethod]
    [DynamicData(nameof(InvalidDateStrings), typeof(DateFormatPolicyTest))]
    public void Validate_InvalidFormats_ReturnsFalse(string endDate)
    {
        var data = LeaseRawDataRowFactory.Create() with { EndDate = endDate };

        var valid = _policy.Validate(data);

        Assert.IsFalse(valid);
    }

    [TestMethod]
    public void Validate_ValidFormat_ReturnsTrue()
    {
        var data = LeaseRawDataRowFactory.Create() with { EndDate = ValidDateString };

        var valid = _policy.Validate(data);

        Assert.IsTrue(valid);
    }
}