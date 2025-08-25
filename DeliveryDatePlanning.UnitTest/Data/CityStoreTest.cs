using DeliveryDatePlanning.Data.Model.Apt;
using DeliveryDatePlanning.Data.Specification.InvoiceDb;

namespace DeliveryDatePlanning.UnitTest.Data;

public class CityStoreTest
{
    [Theory]
    [MemberData(nameof(BeforeMidnight))]
    public void IsMidnight_Before(int timezone, int hour)
    {
        // arrange
        var spec = new IsMidnightCitySpecification(hour);
        var entity = new City { Region = new Region { TimeZone = timezone } };
        
        // act
        var actual = spec.IsSatisfiedBy(entity);

        // assert
        Assert.False(actual);
    }
    
    [Theory]
    [MemberData(nameof(NowMidnight))]
    public void IsMidnight_Now(int timezone, int hour)
    {
        // arrange
        var spec = new IsMidnightCitySpecification(hour);
        var entity = new City { Region = new Region { TimeZone = timezone } };
        
        // act
        var actual = spec.IsSatisfiedBy(entity);

        // assert
        Assert.True(actual);
    }

    [Theory]
    [MemberData(nameof(AfterMidnight))]
    public void IsMidnight_After(int timezone, int hour)
    {
        // arrange
        var spec = new IsMidnightCitySpecification(hour);
        var entity = new City { Region = new Region { TimeZone = timezone } };
        
        // act
        var actual = spec.IsSatisfiedBy(entity);

        // assert
        Assert.False(actual);
    }

    public static IEnumerable<object[]> BeforeMidnight => new List<object[]>
    {
        new object[] {-9, 8},
        new object[] {-8, 7},
        new object[] {-7, 6},
        new object[] {-6, 5},
        new object[] {-5, 4},
        new object[] {-4, 3},
        new object[] {-3, 2},
        new object[] {-2, 1},
        new object[] {-1, 0},
        new object[] {0, 23},
        new object[] {1, 22},
        new object[] {2, 21},
        new object[] {3, 20},
        new object[] {4, 19},
        new object[] {5, 18},
        new object[] {6, 17},
        new object[] {7, 16},
        new object[] {8, 15},
        new object[] {9, 14},
        new object[] {10, 13},
        new object[] {11, 12},
        new object[] {12, 11},
        new object[] {13, 10},
        new object[] {14, 9},
    };

    public static IEnumerable<object[]> NowMidnight => new List<object[]>
    {
        new object[] {-9, 9},
        new object[] {-8, 8},
        new object[] {-7, 7},
        new object[] {-6, 6},
        new object[] {-5, 5},
        new object[] {-4, 4},
        new object[] {-3, 3},
        new object[] {-2, 2},
        new object[] {-1, 1},
        new object[] {0, 0},
        new object[] {1, 23},
        new object[] {2, 22},
        new object[] {3, 21},
        new object[] {4, 20},
        new object[] {5, 19},
        new object[] {6, 18},
        new object[] {7, 17},
        new object[] {8, 16},
        new object[] {9, 15},
        new object[] {10, 14},
        new object[] {11, 13},
        new object[] {12, 12},
        new object[] {13, 11},
        new object[] {14, 10},
    };

    public static IEnumerable<object[]> AfterMidnight => new List<object[]>
    {
        new object[] {-9, 7},
        new object[] {-8, 6},
        new object[] {-7, 5},
        new object[] {-6, 4},
        new object[] {-5, 3},
        new object[] {-4, 2},
        new object[] {-3, 1},
        new object[] {-2, 0},
        new object[] {-1, 23},
        new object[] {0, 22},
        new object[] {1, 21},
        new object[] {2, 20},
        new object[] {3, 19},
        new object[] {4, 18},
        new object[] {5, 17},
        new object[] {6, 16},
        new object[] {7, 15},
        new object[] {8, 14},
        new object[] {9, 13},
        new object[] {10, 12},
        new object[] {11, 11},
        new object[] {12, 10},
        new object[] {13, 9},
        new object[] {14, 8},
    };
}
