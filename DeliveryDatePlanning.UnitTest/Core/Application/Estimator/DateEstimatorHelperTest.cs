using DeliveryDatePlanning.Application.Core.Estimator.CalculationFormulaDefiner;
using DeliveryDatePlanning.UnitTest.Common;

namespace DeliveryDatePlanning.UnitTest.Core.Application.Estimator;

public class DateEstimatorHelperTest
{
    [Fact]
    public void AddDaysUntilFirstServiceDay_ShouldReturnStartDate()
    {
        // arrange
        var point = PointFactory.CreateDefaultPoint();
        point.ServiceDateExceptions= new List<DateTime> {new(2022, 12, 11)};
        point.ServiceDatePositiveExceptions= new List<DateTime> {new(2022, 12, 1)};
        var startDate = new DateOnly(2022, 12, 1);
        
        // act
        var result = startDate.AddDaysUntilFirstServiceDay(point);
        
        // assert
        Assert.Equal(result, startDate);
    }
    
    [Fact]
    public void AddDaysUntilFirstServiceDay_ShouldReturnStartDatePlusOneDay()
    {
        // arrange
        var point = PointFactory.CreateDefaultPoint();
        point.ServiceDateExceptions= new List<DateTime> {new(2022, 12, 1)};
        var startDate = new DateOnly(2022, 12, 1);
        var expectedDate = new DateOnly(2022, 12, 2);

        // act
        var result = startDate.AddDaysUntilFirstServiceDay(point);
        
        // assert
        Assert.Equal(result, expectedDate);
    }
    
    [Fact]
    public void AddDaysUntilFirstServiceDay_ShouldReturnMonday()
    {
        // arrange
        var point = PointFactory.CreateDefaultPoint();
        point.ServiceDateExceptions= new List<DateTime> {new(2022, 12, 1), new(2022, 12, 2), new(2022, 12, 3)};
        var startDate = new DateOnly(2022, 12, 1);
        var expectedDate = new DateOnly(2022, 12, 5);

        // act
        var result = startDate.AddDaysUntilFirstServiceDay(point);
        
        // assert
        Assert.Equal(result, expectedDate);
    }
    
    [Fact]
    public void AddDaysUntilFirstServiceDay_ShouldReturnSunday()
    {
        // arrange
        var point = PointFactory.CreateDefaultPoint();
        point.ServiceDateExceptions= new List<DateTime> {new(2022, 12, 1), new(2022, 12, 2), new(2022, 12, 3)};
        point.ServiceDatePositiveExceptions= new List<DateTime> {new(2022, 12, 4)};
        var startDate = new DateOnly(2022, 12, 1);
        var expectedDate = new DateOnly(2022, 12, 4);

        // act
        var result = startDate.AddDaysUntilFirstServiceDay(point);
        
        // assert
        Assert.Equal(result, expectedDate);
    }
}