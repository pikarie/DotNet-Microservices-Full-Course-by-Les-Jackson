using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using System.Text.Json;

namespace CommandsService.EventProcessing
{
	public class EventProcessor : IEventProcessor
	{
		private readonly IServiceScopeFactory serviceScopeFactory;
		private readonly IMapper mapper;

		public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
		{
			this.serviceScopeFactory = serviceScopeFactory;
			this.mapper = mapper;
		}

		public void ProcessEvent(string message)
		{
			var eventType = DetermineEvent(message);

			switch (eventType)
			{
				case EventType.PlatformPublished:
					AddPlatform(message);
					break;
				default:
					break;
			}
		}

		private EventType DetermineEvent(string notificationMessage)
		{
			Console.WriteLine("---> Determining Event");

			var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

			switch (eventType.Event)
			{
				case "Platform_Published":
					Console.WriteLine("---> Platform Published Event Detected");
					return EventType.PlatformPublished;
				default:
					Console.WriteLine("---> Could not determine the event type.");
					return EventType.Undetermined;
			}
		}

		private void AddPlatform(string platformPublishedMessage)
		{
			using (var scope = serviceScopeFactory.CreateScope())
			{
				var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

				var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

				try
				{
					var platform = mapper.Map<Platform>(platformPublishedDto);
					if (!repo.ExternalPlatformExists(platform.ExternalId))
					{
						repo.CreatePlatform(platform);
						repo.SaveChanges();
						Console.WriteLine($"---> Platform added!");
					}
					else
					{
						Console.WriteLine($"---> Platform already exists.");
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"---> Could not add Platform to database. {e.Message}");
				}
			}
		}
	}

	enum EventType
	{
		PlatformPublished,
		Undetermined
	}
}
