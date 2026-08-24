using AmenityBookingService.Application.Features.Amenities.Commands;
using AmenityBookingService.Application.Features.Amenities.DTO;
using AmenityBookingService.Domain.Entities;
using AutoMapper;

namespace AmenityBookingService.Application.Mappings;

/// <summary>
/// AutoMapper profile for Amenity feature mappings.
/// </summary>
public class AmenityMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmenityMappingProfile"/> class.
    /// </summary>
    public AmenityMappingProfile()
    {
        CreateMap<CreateAmenityRequestDto, CreateAmenityCommand>();

        CreateMap<UpdateAmenityRequestDto, UpdateAmenityCommand>();

        CreateMap<CreateAmenityCommand, Amenity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.SlotType, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.AmenitySlots, opt => opt.Ignore())
            .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules ?? string.Empty))
            .ForMember(
                dest => dest.ImageUrl,
                opt => opt.MapFrom(src => src.ImageUrl ?? string.Empty)
            );

        CreateMap<Amenity, AmenityResponseDto>()
            .ForMember(
                dest => dest.SlotType,
                opt => opt.MapFrom(src => src.SlotType != null ? src.SlotType.Code : string.Empty)
            )
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => src.Status != null ? src.Status.Code : string.Empty)
            );
    }
}
