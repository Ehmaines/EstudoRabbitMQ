using RabbitMQ.Client;
using RabbitMQ.Client.Events;
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

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var json = Encoding.UTF8.GetString(body);
    var order = JsonSerializer.Deserialize<Order>(json);

    Console.WriteLine($"Processando pedido #{order.OrderId} de {order.CustomerName} - Valor: R${order.TotalAmount}");

    // Simula processamento
    Thread.Sleep(1000);
    Console.WriteLine($"Pedido {order.OrderId} finalizado\n");
};

channel.BasicConsume(queue: "pedidos", autoAck: true, consumer: consumer);

Console.WriteLine("Aguardando pedidos...");
Console.ReadLine();
