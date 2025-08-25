using System.Text.RegularExpressions;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

namespace DeliveryDatePlanning.Domain.Core.Entity;

using CSharpFunctionalExtensions;

public class City : Entity<string>
{
    public CityTimeZone TimeZone { get; }

    private City(string id, CityTimeZone timeZone) : base(id)
    {
        this.TimeZone = timeZone;
    }

    public static Result<City> Create(string id, CityTimeZone timeZone)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Result.Failure<City>($"{nameof(id)} argument is null or whitespace");
        }

        if (Regex.IsMatch(id, @"(^[0-9]+)[-]([0-9]+$)") == false)
        {
            return Result.Failure<City>($"{nameof(id)} format is invalid");
        }

        var entity = new City(id, timeZone);

        return entity;
    }

    public override string ToString()
    {
        return $"CityId={Id}, TimeZone={TimeZone}";
    }
}
