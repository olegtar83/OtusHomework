using AutoMapper;
using LegendarySocialNetwork.Domain.Entities;
using LegendarySocialNetwork.Domain.Messages;

namespace LegendarySocialNetwork.DataClasses.Mapper;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<PostEntity, PostMessage>()
         .ForMember(src => src.UserId, opt => opt.MapFrom(dest => dest.User_id))
         .ForMember(src => src.Created, opt => opt.MapFrom(dest => dest.Updated ?? dest.Created))
         .ReverseMap();
    }
}
