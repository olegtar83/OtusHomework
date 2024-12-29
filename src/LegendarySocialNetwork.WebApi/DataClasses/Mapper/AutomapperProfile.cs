using AutoMapper;
using LegendarySocialNetwork.Application.Features.Auth.Register;
using LegendarySocialNetwork.DataClasses.Dtos;
using LegendarySocialNetwork.Domain.Entities;
using LegendarySocialNetwork.Domain.Messages;
using LegendarySocialNetwork.WebApi.DataClasses.Dtos;

namespace LegendarySocialNetwork.DataClasses.Mapper;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<RegisterUserRequest, UserEntity>()
           .ForMember(src => src.Id, opt => opt.MapFrom(dest => Guid.NewGuid().ToString()));
        CreateMap<UserEntity, UserDto>();
        CreateMap<PostMessage, PostDto>().ReverseMap();
        CreateMap<PostEntity, PostDto>()
            .ForMember(src => src.UserId, opt => opt.MapFrom(dest => dest.User_id))
            .ForMember(src => src.Created, opt => opt.MapFrom(dest => dest.Updated ?? dest.Created));
    }
}
