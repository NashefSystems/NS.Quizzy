using AutoMapper;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.Models.DTOs;

namespace NS.Quizzy.Server.BL.MappingProfiles
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BaseEntity, BaseEntityDto>()
                .ForMember(des => des.Id, opt => opt.MapFrom(y => y.Id));

            CreateMap<Subject, SubjectDto>()
                .ForMember(des => des.Name, opt => opt.MapFrom(y => y.Name))
                .ForMember(des => des.ItemOrder, opt => opt.MapFrom(y => y.ItemOrder));

            CreateMap<Class, ClassDto>()
               .ForMember(des => des.Name, opt => opt.MapFrom(y => y.Name))
               .ForMember(des => des.Children, opt => opt.MapFrom(y => y.Children));
        }
    }
}
