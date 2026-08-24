using AutoMapper;
using IdentityService.Application.Features.Approvals.DTOs;
using IdentityService.Application.Features.Auth.DTOs;
using IdentityService.Application.Features.Users.DTOs;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNo))
                .ForMember(
                    dest => dest.Role,
                    opt => opt.MapFrom(src => src.Role != null ? src.Role.Code : null)
                )
                .ForMember(dest => dest.FlatId, opt => opt.Ignore());

            CreateMap<User, AuthResponseDto>();

            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.PhoneNo, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.RoleId, opt => opt.Ignore());

            CreateMap<RegisterManagementRequestDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.PhoneNo, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.PhotoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.RoleId, opt => opt.Ignore());

            CreateMap<FlatOccupancy, ApprovalDetailDto>()
                .ForMember(
                    dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Name : null)
                )
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : null)
                )
                .ForMember(
                    dest => dest.FlatNumber,
                    opt => opt.MapFrom(src => src.Flat != null ? src.Flat.Number : null)
                )
                .ForMember(
                    dest => dest.Block,
                    opt => opt.MapFrom(src => src.Flat != null ? src.Flat.Block : null)
                )
                .ForMember(
                    dest => dest.ResidentType,
                    opt =>
                        opt.MapFrom(src =>
                            src.ResidentType != null ? src.ResidentType.DisplayName : null
                        )
                )
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.IsApproved ? "approved" : "pending")
                )
                .ForMember(dest => dest.Remarks, opt => opt.Ignore());
        }
    }
}
