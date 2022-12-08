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
}