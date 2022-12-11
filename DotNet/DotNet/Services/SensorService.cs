using DotNet.Models;
using MongoDB.Driver;

namespace DotNet.Services;

public class SensorService
{
    private readonly IMongoCollection<Sensor> _sensorList;

    public SensorService()
    {
        //var client = new MongoClient("mongodb://root:example@mongo:27017");
        var client = new MongoClient("mongodb://root:student@actina15.maas:27017");
        var database = client.GetDatabase("si_180152");
        _sensorList = database.GetCollection<Sensor>("Sensors");
    }
        
    public List<Sensor> Get() =>
        _sensorList.Find(humidity => true).ToList();
    
    public Sensor Create(Sensor sensor)
    {
        _sensorList.InsertOne(sensor);
        return sensor;
    }
}