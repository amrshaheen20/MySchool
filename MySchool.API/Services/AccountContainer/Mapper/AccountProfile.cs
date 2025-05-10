using AutoMapper;
using MySchool.API.Extensions;
using MySchool.API.Models.DbSet;
using MySchool.API.Models.Dtos;

namespace MySchool.API.Services.AccountContainer.Mapper
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AccountRequestDto, User>()

                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom((src, dest) => src.Role ?? dest.Role))
                .ForMember(dest => dest.IsActive,
                    opt => opt.MapFrom((src, dest) => src.IsActive ?? dest.IsActive))
                .ForMember(dest => dest.MustChangePassword,
                    opt => opt.MapFrom((src, dest) => src.MustChangePassword ?? dest.MustChangePassword))
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.MapFrom((src, dest) => src.Password != null ? src.Password.HashPassword() : dest.PasswordHash))
                //.ForMember(dest => dest.CustomFields, opt =>
                //{
                //    opt.MapFrom((src, dest,context) => src.CustomFields.Select(x => new UserCustomFields
                //    {
                //        Key = x.Key,
                //        Value = x.Value
                //    }));

                //    opt.PreCondition((src, dest) => src.CustomFields.Any());
                //})

                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<User, AccountAdminResponseDto>();
            //.ForMember(dest => dest.CustomFields,
            //    opt => opt.MapFrom(src => src.CustomFields
            //        .Select(cf => new KeyValuePair<string, string>(cf.Key, cf.Value))))

            CreateMap<User, AccountResponseDto>();
        }
    }
}
