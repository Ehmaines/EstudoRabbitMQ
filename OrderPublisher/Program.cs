using RabbitMQ.Client;
using SharedModels;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "pedidos",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

string[] randomNames = { "Eduardo", "Carlos", "Fernanda", "Lucas", "Aline", "João", "Patrícia", "Mateus", "Larissa", "Rafael" };
var random = new Random();

while (true)
{
    var order = new Order
    {
        OrderId = Guid.NewGuid().ToString(),
        CustomerName = randomNames[random.Next(randomNames.Length)],
        TotalAmount = Math.Round((decimal)(random.NextDouble() * 500 + 50), 2)
    };

    var json = JsonSerializer.Serialize(order);
    var body = Encoding.UTF8.GetBytes(json);

    channel.BasicPublish(exchange: "",
                         routingKey: "pedidos",
                         basicProperties: null,
                         body: body);

    Console.WriteLine($"[x] Pedido gerado para {order.CustomerName} (${order.TotalAmount})");

    await Task.Delay(1000);
}