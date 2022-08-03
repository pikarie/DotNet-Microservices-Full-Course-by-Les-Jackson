using CommandsService.Models;

namespace CommandsService.Data
{
	public class CommandRepo : ICommandRepo
	{
		private readonly AppDbContext context;

		public CommandRepo(AppDbContext context)
		{
			this.context = context;
		}

		public bool SaveChanges()
		{
			return (context.SaveChanges() >= 0);
		}

		public IEnumerable<Platform> GetAllPlatforms()
		{
			return context.Platforms.ToList();
		}

		public void CreatePlatform(Platform platform)
		{
			if (platform == null)
			{
				throw new ArgumentNullException(nameof(platform));
			}

			context.Platforms.Add(platform);
		}

		public bool PlatformExists(int platformId)
		{
			return context.Platforms.Any(x => x.Id == platformId);
		}

		public bool ExternalPlatformExists(int externalPlatformId)
		{
			return context.Platforms.Any(x => x.ExternalId == externalPlatformId);
		}

		#region Commands
		public IEnumerable<Command> GetCommandsForPlatform(int platformId)
		{
			return context.Commands
				.Where(x => x.PlatformId == platformId)
				.OrderBy(x => x.Platform.Name);
		}

		public void CreateCommand(int platformId, Command command)
		{
			if (command == null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			command.PlatformId = platformId;
			context.Commands.Add(command);
		}

		public Command GetCommand(int platformId, int commandId)
		{
			return context.Commands
				.Where(x => x.PlatformId == platformId && x.Id == commandId).FirstOrDefault();
		}
		#endregion
	}
}
