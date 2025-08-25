using MongoDB.Bson.Serialization.Attributes;
using Models;

namespace DeliveryDatePlanning.Protocol.Models;

public class EventJson
{
    private CompanyKey objectKey;
    [BsonIgnore]
    public CompanyKey ObjectKey
    {
        get => this.objectKey;
        set
        {
            this.objectKey = value;
            this.objectId = value.ToString();
        }
    }

    private string objectId;
    public string Id
    {
        get => this.objectId;
        set
        {
            this.objectId = value;
            this.objectKey = CompanyKey.Parse(value);
        }
    }
        
    public DateTime Time { get; set; }

    public int? PreviousAttemptIndex { get; set; }
}