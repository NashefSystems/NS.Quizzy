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
            //CreateMap<AppSetting, AppSettingDto>();
            CreateMap<Class, ClassDto>();
            CreateMap<ClassExam, ClassExamDto>();
            CreateMap<Exam, ExamDto>()
                .ForMember(dst => dst.GradeIds, opt => opt.MapFrom(src => src.GradeExams.Where(x => x.IsDeleted == false).Select(x => x.GradeId).ToList()))
                .ForMember(dst => dst.ClassIds, opt => opt.MapFrom(src => src.ClassExams.Where(x => x.IsDeleted == false).Select(x => x.ClassId).ToList()));
            CreateMap<ExamType, ExamTypeDto>();
            CreateMap<Moed, MoedDto>();
            CreateMap<Grade, GradeDto>();
            CreateMap<GradeExam, GradeExamDto>();
            CreateMap<Questionnaire, QuestionnaireDto>();
            CreateMap<Subject, SubjectDto>();
            CreateMap<User, UserDto>();
        }
    }
}
