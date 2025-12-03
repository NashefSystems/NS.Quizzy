using AutoMapper;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.BL.DTOs;
using NS.Quizzy.Server.BL.Utils;
using NS.Quizzy.Server.DAL.Models;
using NS.Quizzy.Server.BL.Extensions;

namespace NS.Quizzy.Server.BL.MappingProfiles
{
    internal class MappingGlobalProfile : Profile
    {
        public MappingGlobalProfile()
        {
            CreateMap<BaseEntity, IBaseEntityDto>();
            //CreateMap<AppSetting, AppSettingDto>();
            CreateMap<Class, ClassDto>();
            CreateMap<ClassExam, ClassExamDto>();
            CreateMap<Exam, ExamDto>()
                .ForMember(dst => dst.StartTimeStr, opt => opt.MapFrom(src => src.StartTime.ToExamStringTime()))
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
            CreateMap<NotificationTemplate, NotificationTemplateDto>();
            CreateMap<NotificationGroup, NotificationGroupDto>();
            CreateMap<Notification, NotificationDto>()
                .ForMember(dst => dst.TotalUsers, opt => opt.MapFrom(src => src.UserNotifications == null ? default : src.UserNotifications.Count()))
                .ForMember(dst => dst.TotalRead, opt => opt.MapFrom(src => src.UserNotifications == null ? default : src.UserNotifications.Where(x => x.SeenAt.HasValue).Count()))
                .ForMember(dst => dst.ReadPercentage, opt => opt.MapFrom(src => src.UserNotifications == null ? default : NumberUtils.GetPercentage(src.UserNotifications.Where(x => x.SeenAt.HasValue).Count(), src.UserNotifications.Count())))
                .ForMember(dst => dst.NumberOfPushNotificationsReceived, opt => opt.MapFrom(src => src.UserNotifications == null ? default : src.UserNotifications.Where(x => x.PushNotificationsSendingTime.HasValue).Count()))
                .ForMember(dst => dst.PushNotificationReceivedPercentage, opt => opt.MapFrom(src => src.UserNotifications == null ? default : NumberUtils.GetPercentage(src.UserNotifications.Where(x => x.PushNotificationsSendingTime.HasValue).Count(), src.UserNotifications.Count())));
            CreateMap<NotificationTarget, NotificationTargetDto>();
            CreateMap<Device, DeviceDto>();
            CreateMap<UserLoginStatus, UserLoginStatusDto>();
        }
    }
}
