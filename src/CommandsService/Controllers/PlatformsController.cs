using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
	[Route("api/commands/[controller]")]
	[ApiController]
	public class PlatformsController : ControllerBase
	{
		private readonly ICommandRepo commandRepo;
		private readonly IMapper mapper;

		public PlatformsController(ICommandRepo commandRepo, IMapper mapper)
		{
			this.commandRepo = commandRepo;
			this.mapper = mapper;
		}

		[HttpGet]
		public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
		{
			Console.WriteLine("---> Calling GetPlatforms from Commands service.");

			var platforms = commandRepo.GetAllPlatforms();

			return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
		}

		[HttpPost]
		public ActionResult TestInboundConnection()
		{
			Console.WriteLine("---> Inbound POST in commands service.");

			return Ok($"Inbound test from {nameof(PlatformsController)} ");
		}
	}
}
