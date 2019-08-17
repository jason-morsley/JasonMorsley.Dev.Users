using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Users.API.Entities;
using Users.API.Models;

namespace Users.API.Profile
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(destination => destination.Name, options => options.MapFrom(src =>
                    $"{src.FirstName} {src.LastName}"));

            CreateMap<UserForCreationDto, User>();
        }
    }
}
