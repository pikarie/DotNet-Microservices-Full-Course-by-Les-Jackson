using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.AutoMapperProfiles
{
	public class PlatformProfiles : Profile
	{
		public PlatformProfiles()
		{
			CreateMap<Platform, PlatformReadDto>();//.ReverseMap();
			CreateMap<PlatformCreateDto, Platform>();//.ReverseMap();
			CreateMap<PlatformReadDto, PlatformPublishedDto>();
			CreateMap<Platform, GrpcPlatformModel>()
				.ForMember(dest => dest.PlatformId, opt => opt.MapFrom(src => src.Id));
		}
	}
}
