using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
	//[Route("api/[controller]")]
	[Route("api/commands/platforms/{platformId}/[controller]")]
	[ApiController]
	public class CommandsController : ControllerBase
	{
		private readonly ICommandRepo commandRepo;
		private readonly IMapper mapper;

		public CommandsController(ICommandRepo commandRepo, IMapper mapper)
		{
			this.commandRepo = commandRepo;
			this.mapper = mapper;
		}

		[HttpGet]
		public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
		{
			if (!commandRepo.PlatformExists(platformId))
			{
				return NotFound();
			}

			var commands = commandRepo.GetCommandsForPlatform(platformId);

			return Ok(mapper.Map<IEnumerable<CommandCreateDto>>(commands));
		}

		[HttpGet("{commandId}", Name = nameof(GetCommandForPlatform))]
		public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
		{
			if (!commandRepo.PlatformExists(platformId))
			{
				return NotFound();
			}

			var command = commandRepo.GetCommand(platformId, commandId);

			if (command is null)
			{
				return NotFound();
			}

			return Ok(mapper.Map<CommandReadDto>(command));
		}

		[HttpPost]
		public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandCreateDto)
		{
			if (!commandRepo.PlatformExists(platformId))
			{
				return NotFound();
			}

			var command = mapper.Map<Command>(commandCreateDto);

			commandRepo.CreateCommand(platformId, command);
			commandRepo.SaveChanges();

			var commandReadDto = mapper.Map<CommandReadDto>(command);

			return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
		}

	}
}
