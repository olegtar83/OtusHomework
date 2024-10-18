using AutoMapper;

using LegendarySocialNetwork.Database.Entities;
using LegendarySocialNetwork.DataClasses.Dtos;
using LegendarySocialNetwork.DataClasses.Requests;

namespace LegendarySocialNetwork.DataClasses.Mapper;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<RegisterReq, UserEntity>()
            .ForMember(e => e.Id, opt => opt.MapFrom(r => Guid.NewGuid().ToString()))
            ;
        CreateMap<UserEntity, UserDto>();
    }
}
