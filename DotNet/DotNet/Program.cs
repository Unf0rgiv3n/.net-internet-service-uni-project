using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotNet v1"));
}

var factory = new ConnectionFactory() { HostName = "si_180152_rabbit" };
var connection = factory.CreateConnection();
var channel = connection.CreateModel();

channel.QueueDeclare(queue: "sensor-HUMIDITY",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

channel.QueueDeclare(queue: "sensor-INSOLATION",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

channel.QueueDeclare(queue: "sensor-TEMPERATURE",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

channel.QueueDeclare(queue: "sensor-NOISE",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [x] Received {0}", message);
};
channel.BasicConsume(queue: "sensor-HUMIDITY",
    autoAck: true,
    consumer: consumer);

channel.BasicConsume(queue: "sensor-INSOLATION",
    autoAck: true,
    consumer: consumer);

channel.BasicConsume(queue: "sensor-TEMPERATURE",
    autoAck: true,
    consumer: consumer);

channel.BasicConsume(queue: "sensor-NOISE",
    autoAck: true,
    consumer: consumer);


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();