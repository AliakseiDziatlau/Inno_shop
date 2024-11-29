using AutoMapper;
using UserControl.Application.DTOs;
using UserControl.Core.Entities;

namespace UserControl.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<RegisterRequestDto, User>();
        CreateMap<UpdateUserRequestDto, User>();
        CreateMap<UpdateUserRequestDto, User>();
    }
}