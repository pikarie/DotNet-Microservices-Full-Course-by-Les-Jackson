using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlatformsController : ControllerBase
	{
		private readonly IPlatformRepo platformRepo;
		private readonly IMapper mapper;
		private readonly ICommandDataClient commandDataClient;
		private readonly IMessageBusClient messageBusClient;

		public PlatformsController(IPlatformRepo platformRepo, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
		{
			this.platformRepo = platformRepo;
			this.mapper = mapper;
			this.commandDataClient = commandDataClient;
			this.messageBusClient = messageBusClient;
		}

		// GET: api/Platforms
		[HttpGet]
		public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
		{
			var platforms = platformRepo.GetAllPlatforms();

			if (platforms == null)
			{
				return NotFound();
			}
			return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
		}

		// GET: api/Platforms/5
		[HttpGet("{id}", Name = nameof(GetPlatform))]
		public async Task<ActionResult<PlatformReadDto>> GetPlatform(int id)
		{
			var platform = platformRepo.GetPlatformById(id);

			if (platform == null)
			{
				return NotFound();
			}

			return Ok(mapper.Map<PlatformReadDto>(platform));
		}

		// POST: api/Platforms
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<PlatformReadDto>> PostPlatform(PlatformCreateDto platformCreateDto)
		{
			var platform = mapper.Map<Platform>(platformCreateDto);

			platformRepo.CreatePlatform(platform);
			platformRepo.SaveChanges();

			var platformReadDto = mapper.Map<PlatformReadDto>(platform);

			//Send Sync message
			try
			{
				await commandDataClient.SendPlatformToCommand(platformReadDto);
			}
			catch (Exception e)
			{
				Console.WriteLine($"--> Could not send synchronously: {e}");
			}

			//Send Async message
			try
			{
				var platformPublishedDto = mapper.Map<PlatformPublishedDto>(platformReadDto);
				platformPublishedDto.Event = "Platform_Published"; //TODO: document the events expected to be sent and received.
				messageBusClient.PublishNewPlatform(platformPublishedDto);
			}
			catch (Exception e)
			{
				Console.WriteLine($"--> Could not send asynchronously: {e}");
			}

			return CreatedAtRoute(nameof(GetPlatform), new { Id = platformReadDto.Id }, platformReadDto);
		}
	}
}
