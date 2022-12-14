using DotNet.Models;
using DotNet.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Controllers;

[ApiController]
[Route("[controller]")]
public class SensorController: ControllerBase
{
    private readonly SensorService _sensorService;

    public SensorController(SensorService sensorService)
    {
        _sensorService = sensorService;
    }
    
    [HttpGet]
    public ActionResult<List<Sensor>> Get()
    {
        return _sensorService.Get();
    }

    [HttpGet("GetByName/{name}")]
    public ActionResult<List<Sensor>> GetByName(string name, string order)
    {
        return _sensorService.GetByName(name, order);
    }
    
    [HttpGet("GetByRoom/{room}")]
    public ActionResult<List<Sensor>> GetByRoom(string room, string order)
    {
        return _sensorService.GetByRoom(room, order);
    }
    
    [HttpGet("GetByType/{type}")]
    public ActionResult<List<Sensor>> GetByType(string type, string order)
    {
        return _sensorService.GetByType(type, order);
    }
}