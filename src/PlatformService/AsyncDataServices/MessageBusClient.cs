using PlatformService.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataServices
{
	public class MessageBusClient : IMessageBusClient
	{
		private readonly IConfiguration configuration;
		private readonly IConnection connection;
		private readonly IModel channel;

		public MessageBusClient(IConfiguration configuration)
		{
			this.configuration = configuration;
			var factory = new ConnectionFactory() { HostName = configuration["RabbitMQHost"], Port = int.Parse(configuration["RabbitMQPort"]) };

			try
			{
				connection = factory.CreateConnection();
				channel = connection.CreateModel();

				channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
				connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

				Console.WriteLine($"---> Connected to message bus.");
			}
			catch (Exception e)
			{
				Console.WriteLine($"---> Could not connect to the message bus. {e.Message}");
			}
		}

		public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
		{
			var message = JsonSerializer.Serialize(platformPublishedDto);

			if (connection.IsOpen)
			{
				Console.WriteLine($"---> RabbitMQ connection is open. Sending message {message}");
				SendMessage(message);
			}
			else
			{
				Console.WriteLine($"---> RabbitMQ connection is close. message {message}");
			}
		}

		private void SendMessage(string message)
		{
			var body = Encoding.UTF8.GetBytes(message);

			channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);

			Console.WriteLine($"---> We have sent this message = {message}");
		}

		private void Dispose()
		{
			Console.WriteLine($"--->Message bus disposed");

			if (connection.IsOpen)
			{
				channel.Close();
				connection.Close();
			}
		}

		private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
		{
			Console.WriteLine($"---> RabbitMQ connection shutdown.");
		}
	}
}
