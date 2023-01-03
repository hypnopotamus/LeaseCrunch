using LeaseCrunch.Domain.DataFormatPolicies;

namespace LeaseCrunch.Domain.Tests.DataFormatPolicies;

[TestClass]
public class StartDateFormatPolicyTest : DateFormatPolicyTest
{
    private readonly IDataFormatPolicy _policy = new StartDateFormatPolicy();

    [TestMethod]
    [DynamicData(nameof(InvalidDateStrings), typeof(DateFormatPolicyTest))]
    public void Validate_InvalidFormats_ReturnsFalse(string startDate)
    {
        var data = LeaseRawDataRowFactory.Create() with { StartDate = startDate };

        var valid = _policy.Validate(data);

        Assert.IsFalse(valid);
    }

    [TestMethod]
    public void Validate_ValidFormat_ReturnsTrue()
    {
        var data = LeaseRawDataRowFactory.Create() with { StartDate = ValidDateString };

        var valid = _policy.Validate(data);

        Assert.IsTrue(valid);
    }
}