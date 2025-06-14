﻿using AutoMapper;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.ClassContainer.Mapper
{
    public class ClassRoomProfile : Profile
    {

        public ClassRoomProfile()
        {
            //Request
            CreateMap<ClassRequestDto, ClassRoom>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            //Response
            CreateMap<ClassRoom, ClassResponseDto>()
                .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Enrollments.Count()))
                .ForMember(dest => dest.TotalTimetables, opt => opt.MapFrom(src => src.Timetables.Count()));
        }



    }
}
