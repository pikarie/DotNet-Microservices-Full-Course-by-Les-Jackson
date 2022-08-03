using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data
{
	public static class PrepareDb
	{
		public static void PreparePopulation(IApplicationBuilder applicationBuilder)
		{
			using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
			{
				var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
				var platforms = grpcClient.ReturnAllPlatforms();

				SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
			}
		}

		private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
		{
			Console.WriteLine("---> Seeding new platforms");

			if (platforms != null)
			{
				foreach (var platform in platforms)
				{
					if (!repo.ExternalPlatformExists(platform.ExternalId))
					{
						repo.CreatePlatform(platform);
						repo.SaveChanges();
					}
				}
			}
		}
	}
}
