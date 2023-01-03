namespace LeaseCrunch.Domain.Tests.DataFormatPolicies;

public abstract class DateFormatPolicyTest
{
    protected static IEnumerable<object> InvalidDateStrings => new[]
    {
        new[] { "1/02/2022" },
        new[] { "01/2/2022" },
        new[] { "01/02/22" }
    };

    protected string ValidDateString => DateOnly.FromDateTime(DateTime.Now).ToString("MM/dd/yyyy");
}