using PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncDataServices.Http
{
	public class CommandDataClient : ICommandDataClient
	{
		private readonly HttpClient httpClient;
		private readonly IConfiguration configuration;

		public CommandDataClient(HttpClient httpClient, IConfiguration configuration)
		{
			this.httpClient = httpClient;
			this.configuration = configuration;
		}

		public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
		{
			var httpContent = new StringContent(
				JsonSerializer.Serialize(platformReadDto),
				Encoding.UTF8,
				"application/json"
				);

			var response = await httpClient.PostAsync(configuration["CommandsService"], httpContent);

			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("--> Sync POST to commandsService was OK!");
			}
			else
			{
				Console.WriteLine("--> Sync POST to commandsService was incorrect.");
			}
		}
	}
}
