using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.AutomapperProfiles
{
	public class CommandProfiles : Profile
	{
		public CommandProfiles()
		{
			CreateMap<Platform, PlatformReadDto>();
			CreateMap<CommandCreateDto, Command>();
			CreateMap<Command, CommandReadDto>();
			CreateMap<PlatformPublishedDto, Platform>()
				.ForMember(destination => destination.ExternalId, options => options.MapFrom(src => src.Id));
			CreateMap<GrpcPlatformModel, Platform>()
				.ForMember(destination => destination.ExternalId, options => options.MapFrom(src => src.PlatformId))
				.ForMember(destination => destination.Commands, options => options.Ignore());
		}
	}
}
