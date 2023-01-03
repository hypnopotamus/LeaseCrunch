using System.Globalization;

namespace LeaseCrunch.Domain.DataFormatPolicies;

public abstract class DateFormatPolicy
{
    protected bool DateFormatValid(string date) => DateOnly.TryParseExact(date, "MM/dd/yyyy", null, DateTimeStyles.None, out _);
}