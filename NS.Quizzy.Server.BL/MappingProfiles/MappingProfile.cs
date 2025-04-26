using AutoMapper;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.BL.DTOs;

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
                .ForMember(dst => dst.GradeIds, opt => opt.MapFrom(src => src.GradeExams.Where(x => x.IsDeleted == false && x.IsImprovement == false).Select(x => x.GradeId).ToList()))
                .ForMember(dst => dst.ImprovementGradeIds, opt => opt.MapFrom(src => src.GradeExams.Where(x => x.IsDeleted == false && x.IsImprovement == true).Select(x => x.GradeId).ToList()))
                .ForMember(dst => dst.ClassIds, opt => opt.MapFrom(src => src.ClassExams.Where(x => x.IsDeleted == false && x.IsImprovement == false).Select(x => x.ClassId).ToList()))
                .ForMember(dst => dst.ImprovementClassIds, opt => opt.MapFrom(src => src.ClassExams.Where(x => x.IsDeleted == false && x.IsImprovement == true).Select(x => x.ClassId).ToList()));
            CreateMap<ExamType, ExamTypeDto>();
            CreateMap<Moed, MoedDto>();
            CreateMap<Grade, GradeDto>();
            CreateMap<GradeExam, GradeExamDto>();
            CreateMap<Questionnaire, QuestionnaireDto>();
            CreateMap<Subject, SubjectDto>();
            CreateMap<User, UserDto>();
            CreateMap<Notification, NotificationDto>()
                .ForMember(dst => dst.TotalUsers, opt => opt.MapFrom(src => src.UserNotifications == null ? default : src.UserNotifications.Count()))
                .ForMember(dst => dst.TotalRead, opt => opt.MapFrom(src => src.UserNotifications == null ? default : src.UserNotifications.Where(x => x.SeenAt.HasValue).Count()))
                .ForMember(dst => dst.ReadPercentage, opt => opt.MapFrom(src => src.UserNotifications == null ? default : GetPercentage(src.UserNotifications.Where(x => x.SeenAt.HasValue).Count(), src.UserNotifications.Count())))
                .ForMember(dst => dst.NumberOfPushNotificationsReceived, opt => opt.MapFrom(src => src.UserNotifications == null ? default : src.UserNotifications.Where(x => x.PushNotificationsSendingTime.HasValue).Count()))
                .ForMember(dst => dst.PushNotificationReceivedPercentage, opt => opt.MapFrom(src => src.UserNotifications == null ? default : GetPercentage(src.UserNotifications.Where(x => x.PushNotificationsSendingTime.HasValue).Count(), src.UserNotifications.Count())));
        }

        private static double? GetPercentage(int? value, int? total)
        {
            if (value == null || total == null)
            {
                return null;
            }

            if (value == 0 || total == 0)
            {
                return 0;
            }

            var val = ((double)value) * 100.0 / (double)total;
            return Math.Round(val);
        }
    }
}
