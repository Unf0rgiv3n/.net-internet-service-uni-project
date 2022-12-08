using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotNet.Models;

public class Sensor
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [JsonInclude]
    public int SensorId { get; set; }
    [JsonInclude]
    public string Name { get; set; }
    [JsonInclude]
    public string Desc { get; set; }
    [JsonInclude] 
    public decimal Measurement { get; set; }
    [JsonInclude]
    public string Type { get; set; }
    [JsonInclude] 
    public string Unit { get; set; }
    [JsonInclude]
    public string Date { get; set; }
}