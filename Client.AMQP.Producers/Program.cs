using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

const string exchangeName = "exchange.class";
const string queueName = "queue.class";

var stopRequested = false;

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

	channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false,
		arguments: null);

	channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

	Console.WriteLine("Producer started. Press [Enter] to stop.");

	var inputThread = new Thread(() =>
	{
		while (true)
		{
			if (Console.ReadKey(intercept: true).Key != ConsoleKey.Enter) continue;
			stopRequested = true;
			break;
		}
	});
	
	inputThread.Start();

	while (!stopRequested)
	{
		var message = new Message
		{
			Name = GetRandomName(),
			Age = GetRandomAge()
		};

		channel.BasicPublish(
			exchange: exchangeName,
			routingKey: "",
			basicProperties: null,
			body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message))
		);

		Console.WriteLine("Message published: " + message.Id);
	}
}
catch (Exception ex)
{
	Console.WriteLine($"An error occurred: {ex.Message}");
}

static string GetRandomName()
{
	string[] names = ["John", "Alice", "Bob", "Eve", "Michael", "Emily", "David", "Sarah", "Daniel", "Olivia"];
	var random = new Random();
	return names[random.Next(names.Length)];
}

static int GetRandomAge()
{
	var random = new Random();
	return random.Next(18, 60);
}

record Message
{
	public Guid Id { get; private set; } = Guid.NewGuid();
	public required string Name { get; set; }
	public required int Age { get; set; }
	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}