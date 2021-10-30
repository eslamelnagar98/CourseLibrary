using AutoMapper;
using CourseLibrary.API.Dtos;
using CourseLibrary.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.Profiles
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDto>().ReverseMap();
            CreateMap<CourseForCreationDto, Course>().ReverseMap();
            CreateMap<CourseForUpdateDto, Course>().ReverseMap();
        }
    }
}
