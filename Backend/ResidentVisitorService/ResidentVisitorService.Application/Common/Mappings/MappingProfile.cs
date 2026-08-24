using AutoMapper;
using ResidentVisitorService.Application.Features.Visitors.DTOs;
using ResidentVisitorService.Application.Features.VisitQrToken.DTOs;
using ResidentVisitorService.Application.Features.Visits.DTOs;
using ResidentVisitorService.Domain.Entities;

namespace ResidentVisitorService.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile that defines all entity-to-DTO and DTO-to-entity mappings
/// for the ResidentVisitor service.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Visitor, VisitorResponseDto>()
            .ForMember(
                dest => dest.VisitorType,
                opt =>
                    opt.MapFrom(src =>
                        src.VisitorType != null ? src.VisitorType.DisplayName : string.Empty
                    )
            );

        CreateMap<CreateVisitorRequestDto, Visitor>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Trim()))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Trim()))
            .ForMember(
                dest => dest.Email,
                opt => opt.MapFrom(src => src.Email != null ? src.Email.Trim() : null)
            );

        CreateMap<Visit, VisitResponseDto>()
            .ForMember(
                dest => dest.VisitorName,
                opt => opt.MapFrom(src => src.Visitor != null ? src.Visitor.Name : string.Empty)
            )
            .ForMember(
                dest => dest.VisitorPhoneNumber,
                opt =>
                    opt.MapFrom(src => src.Visitor != null ? src.Visitor.PhoneNumber : string.Empty)
            )
            .ForMember(
                dest => dest.VisitorEmail,
                opt => opt.MapFrom(src => src.Visitor != null ? src.Visitor.Email : null)
            )
            .ForMember(
                dest => dest.VisitorType,
                opt =>
                    opt.MapFrom(src =>
                        src.Visitor != null && src.Visitor.VisitorType != null
                            ? src.Visitor.VisitorType.DisplayName
                            : string.Empty
                    )
            )
            .ForMember(
                dest => dest.Purpose,
                opt =>
                    opt.MapFrom(src =>
                        src.PurposeType != null ? src.PurposeType.DisplayName : string.Empty
                    )
            )
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => src.Status != null ? src.Status.Code : string.Empty)
            )
            .ForMember(
                dest => dest.StartDate,
                opt => opt.MapFrom(src => DateOnly.FromDateTime(src.StartDate))
            )
            .ForMember(
                dest => dest.EndDate,
                opt => opt.MapFrom(src => DateOnly.FromDateTime(src.EndDate))
            )
            .ForMember(dest => dest.QrToken, opt => opt.MapFrom(src => src.VisitQrToken));

        CreateMap<Domain.Entities.VisitQrToken, VisitQrTokenEmbedDto>();

        CreateMap<RefTerm, RefTermDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName));

        CreateMap<Domain.Entities.VisitQrToken, VisitQrTokenResponseDto>();
    }
}
