using AutoMapper;
using CourseLibrary.API.Dtos;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.Profiles
{
    public class AuthorsProfile : Profile
    {
        public AuthorsProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember
                (dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember
                (dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge()))
                .ForMember(dest => dest.MainCategory, opt => opt.MapFrom(src => src.MainCategory))
                .ReverseMap();

            CreateMap<AuthorForCreationDto, Author>();


        }
    }
}
