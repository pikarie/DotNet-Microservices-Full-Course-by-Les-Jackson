using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
	public static class SeedDb
	{
		public static void PreparationPopulation(IApplicationBuilder app, bool isProd)
		{
			using var serviceScope = app.ApplicationServices.CreateScope();
			SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
		}

		private static void SeedData(AppDbContext context, bool isProd)
		{
			if (isProd)
			{
				Console.WriteLine($"---> Attempting to apply migrations.");
				try
				{
					context.Database.Migrate();
				}
				catch (Exception e)
				{
					Console.WriteLine($"---> Could not run migrations: {e.Message}");
				}
			}

			if (!context.Platforms.Any())
			{
				Console.WriteLine($"Seeding {context.Platforms} data...");
				context.Platforms.AddRange(
					new Platform()
					{
						Name = ".Net",
						Publisher = "Microsoft",
						Cost = "Free",
					},
					new Platform()
					{
						Name = "SQL Server Express",
						Publisher = "Microsoft",
						Cost = "Free",
					},
					new Platform()
					{
						Name = "Kubernetes",
						Publisher = "Cloud Native Computing Foundation",
						Cost = "Free or Paid",
					}
				);

				context.SaveChanges();
			}
			else
			{
				Console.WriteLine($"We already have data in {context.Platforms}");
			}
		}
	}
}
