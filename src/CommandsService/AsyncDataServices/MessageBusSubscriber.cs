using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandsService.AsyncDataServices
{
	public class MessageBusSubscriber : BackgroundService
	{
		private readonly IConfiguration configuration;
		private readonly IEventProcessor eventProcessor;
		private IConnection connection;
		private IModel channel;
		private string queueName;

		public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
		{
			this.configuration = configuration;
			this.eventProcessor = eventProcessor;

			InitializeRabbitMQ();
		}

		private void InitializeRabbitMQ()
		{
			var factory = new ConnectionFactory() { HostName = configuration["RabbitMQHost"], Port = int.Parse(configuration["RabbitMQPort"])};

			connection = factory.CreateConnection();
			channel = connection.CreateModel();
			channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
			queueName = channel.QueueDeclare().QueueName;
			channel.QueueBind(queue: queueName, exchange: "trigger", routingKey: "");
			Console.WriteLine("---> Listening on the message bus.");

			connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			var consumer = new EventingBasicConsumer(channel);

			consumer.Received += (ModuleHandle, ea) =>
			{
				Console.WriteLine("---> Event received!!!");

				var body = ea.Body;
				var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

				eventProcessor.ProcessEvent(notificationMessage);
			};

			channel.BasicConsume(queueName, autoAck: true, consumer: consumer);

			return Task.CompletedTask;
		}

		private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
		{
			Console.WriteLine("---> Connection shutdown.");
		}

		public override void Dispose()
		{
			if (channel.IsOpen)
			{
				channel.Close();
				connection.Close();
			}

			base.Dispose();
		}
	}
}
