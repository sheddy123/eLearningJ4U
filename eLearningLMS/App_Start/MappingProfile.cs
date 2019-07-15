using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using eLearningLMS.Dtos;
using eLearningLMS.Models;

namespace eLearningLMS.App_Start
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to Dto
            Mapper.CreateMap<Programme, ProgrammeDto>();  
            Mapper.CreateMap<Course, CourseDto>();
            Mapper.CreateMap<Enrollment, EnrollmentDto>();
            Mapper.CreateMap<Student, StudentDto>();
            Mapper.CreateMap<Instructor, InstructorDto>();
            Mapper.CreateMap<Department, DepartmentDto>();

            //Dto to Domain
            Mapper.CreateMap<ProgrammeDto, Programme>()
                .ForMember(p => p.Id, opt => opt.Ignore());
            //Mapper.CreateMap<DepartmentDto, Department>()
            // .ForMember(p => p.DepartmentID, opt => opt.Ignore());
            //Mapper.CreateMap<EnrollmentDto, Enrollment>()
            // .ForMember(p => p.EnrollmentID, opt => opt.Ignore());
            //Mapper.CreateMap<StudentDto, Student>()
            // .ForMember(p => p.Id, opt => opt.Ignore());
            //Mapper.CreateMap<InstructorDto, Instructor>()
            // .ForMember(p => p.id, opt => opt.Ignore());
            //Mapper.CreateMap<CourseDto, Course>()
            //    .ForMember(p => p.Id, opt => opt.Ignore());
        }
    }
}