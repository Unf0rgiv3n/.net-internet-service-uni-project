using System.Data;
using DotNet.Models;
using MongoDB.Driver;

namespace DotNet.Services;

public class SensorService
{
    private readonly IMongoCollection<Sensor> _sensorList;

    public SensorService()
    {
        var client = new MongoClient("mongodb://root:example@mongo:27017");
        var database = client.GetDatabase("si_180152");
        _sensorList = database.GetCollection<Sensor>("Sensors");
    }
        
    public List<Sensor> Get() =>
        _sensorList.Find(humidity => true).ToList();

    public List<Sensor> GetByName(string name, string order)
    {
        if (order.ToLower() == "desc")
        {
            
            return _sensorList.Find(x => x.Name.Contains(name)).ToList().OrderByDescending(x=>x.Measurement).ToList();
        }

        return _sensorList.Find(x => x.Name.Contains(name)).ToList().OrderBy(x => x.Measurement).ToList();
    }
    
    public List<Sensor> GetByRoom(string room, string order)
    {
        if (order.ToLower() == "desc")
        {
            return _sensorList.Find(x => x.Desc.Contains(room)).ToList().OrderByDescending(x=>x.Measurement).ToList();
        }

        return _sensorList.Find(x => x.Desc.Contains(room)).ToList().OrderBy(x => x.Measurement).ToList();
    }
    
    public List<Sensor> GetByType(string type, string order)
    {
        if (order.ToLower() == "desc")
        {
            
            return _sensorList.Find(x => x.Type.Contains(type.ToUpper())).ToList().OrderByDescending(x=>x.Measurement).ToList();
        }

        return _sensorList.Find(x => x.Type.Contains(type.ToUpper())).ToList().OrderBy(x => x.Measurement).ToList();
    }
    
    public Sensor Create(Sensor sensor)
    {
        _sensorList.InsertOne(sensor);
        return sensor;
    }
}