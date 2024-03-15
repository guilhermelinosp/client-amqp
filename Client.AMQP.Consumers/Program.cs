using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

const string exchangeName = "exchange.class";
const string queueName = "queue.class";

try
{
	var factory = new ConnectionFactory
	{
		HostName = "localhost",
		UserName = "guest",
		Password = "guest"
	};

	using var connection = factory.CreateConnection();
	using var channel = connection.CreateModel();

	channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
	channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
	channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");

	var consumer = new EventingBasicConsumer(channel);
	consumer.Received += (_, ea) =>
	{
		var body = ea.Body.ToArray();
		var message = Encoding.UTF8.GetString(body);
		Console.WriteLine("Received message: " + message);
	};

	channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

	Console.WriteLine("Consumer started. Press [Enter] to exit.");
	Console.ReadLine();
}
catch (Exception ex)
{
	Console.WriteLine($"An error occurred: {ex.Message}");
}