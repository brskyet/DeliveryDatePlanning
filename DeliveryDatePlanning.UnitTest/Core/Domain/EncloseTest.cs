using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using Models;

namespace DeliveryDatePlanning.UnitTest.Core.Domain;

public class EncloseTest
{
    [Theory]
    [InlineData("123-456", 100)]
    [InlineData("123-456", 150)]
    [InlineData("123-456", 155)]
    public void Create_Success(string id, int? state)
    {
        // arrange, act
        var actual = EncloseFactory(id, state);

        // assert
        Assert.True(actual.IsSuccess);
    }
    
    [Theory]
    [InlineData(null, 100)]
    [InlineData(" ", 100)]
    public void Create_Failure(string id, int? state)
    {
        // arrange, act
        var actual = EncloseFactory(id, state);

        // assert
        Assert.True(actual.IsFailure);
    }
    
    [Fact]
    public void AddEncloseStateRecord_HistoryEmpty_Success()
    {
        // arrange
        var enclose = EncloseFactory("123-456", 100).Value;
        var timestamp = DateTime.Now.ToUniversalTime();
        var state = EncloseState.Searched;
        
        // act
        enclose.AddEncloseStateRecord(timestamp, state);
        var actual = enclose.StateHistory.First();

        // assert
        Assert.True(actual is not null);
        Assert.True(actual?.Timestamp == timestamp);
        Assert.True(actual?.State == state);
    }
    
    [Fact]
    public void AddEncloseStateRecord_HistoryNotEmpty_Success()
    {
        // arrange
        var enclose = EncloseFactory("123-456", 100, new []{ EncloseStateHistoryRecord.Create(DateTime.Now, EncloseState.Registered).Value }).Value;
        var timestamp = DateTime.Now.ToUniversalTime();
        var state = EncloseState.Searched;
        
        // act
        enclose.AddEncloseStateRecord(timestamp, state);
        var actual = enclose.StateHistory.Last();

        // assert
        Assert.True(actual is not null);
        Assert.True(actual?.Timestamp == timestamp);
        Assert.True(actual?.State == state);
    }

    private Result<Enclose> EncloseFactory(string id, int? stateValue)
        => this.EncloseFactory(id, stateValue, Array.Empty<EncloseStateHistoryRecord>());
    
    private Result<Enclose> EncloseFactory(string id, int? stateValue, EncloseStateHistoryRecord[] history)
    {
        var state = EncloseStateEnum.Create(stateValue).Value;

        var result = Enclose.Create(id, state, history);

        return result;
    }
}