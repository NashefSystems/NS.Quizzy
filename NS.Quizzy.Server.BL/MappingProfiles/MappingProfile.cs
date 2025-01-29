using AutoMapper;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.Models.DTOs;

namespace NS.Quizzy.Server.BL.MappingProfiles
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BaseEntity, IBaseEntityDto>();
            CreateMap<Subject, SubjectDto>();
            CreateMap<ExamType, ExamTypeDto>();
            CreateMap<Class, ClassDto>();
            CreateMap<Grade, GradeDto>();
        }
    }
}
