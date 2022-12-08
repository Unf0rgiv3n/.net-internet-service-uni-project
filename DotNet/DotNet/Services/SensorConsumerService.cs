using System.Text;
using System.Text.Json;
using DotNet.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace DotNet.Services;

public class SensorConsumerService: BackgroundService
{
    private readonly SensorService _service;
    private readonly IModel _channel;

    public SensorConsumerService(SensorService service)
    {
        _service = service;
        var factory = new ConnectionFactory() { HostName = "si_180152_rabbit" };
        IConnection connection;
        while (true)
        {
            try
            {
                Console.WriteLine("Try To connect");
                connection = factory.CreateConnection();
                Console.WriteLine("Connection Success");
                break;
            }
            catch (BrokerUnreachableException exception)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Connection Fail");
            }
        }
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "sensor",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Received sensor {0}", message);
            var newObject = JsonSerializer.Deserialize<Sensor>(message);
            _service.Create(newObject);
        };
        _channel.BasicConsume(queue: "sensor",
            autoAck: true,
            consumer: consumer);
        return Task.CompletedTask;
    }
}