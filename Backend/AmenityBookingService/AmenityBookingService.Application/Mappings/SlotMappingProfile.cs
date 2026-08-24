using AmenityBookingService.Application.Features.Slots.Commands;
using AmenityBookingService.Application.Features.Slots.DTO;
using AmenityBookingService.Domain.Entities;
using AutoMapper;

namespace AmenityBookingService.Application.Mappings;

/// <summary>
/// AutoMapper profile for Slot feature mappings.
/// </summary>
public class SlotMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SlotMappingProfile"/> class.
    /// </summary>
    public SlotMappingProfile()
    {
        CreateMap<UpdateSlotRequestDto, UpdateSlotCommand>();
        CreateMap<CreateSlotsBulkRequestDto, CreateSlotsBulkCommand>();

        CreateMap<CreateSlotRequestDto, AmenitySlot>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AmenityId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentBookingCount, opt => opt.Ignore())
            .ForMember(dest => dest.Amenity, opt => opt.Ignore())
            .ForMember(dest => dest.AmenityBookings, opt => opt.Ignore());

        CreateMap<AmenitySlot, SlotResponseDto>()
            .ForMember(
                dest => dest.CurrentBookings,
                opt => opt.MapFrom(src => src.CurrentBookingCount)
            );

        CreateMap<AmenitySlot, AvailableSlotResponseDto>()
            .ForMember(dest => dest.SlotId, opt => opt.MapFrom(src => src.Id))
            .ForMember(
                dest => dest.CurrentBookings,
                opt => opt.MapFrom(src => src.CurrentBookingCount)
            )
            .ForMember(dest => dest.AvailableSpots, opt => opt.Ignore());

        CreateMap<Amenity, AvailableSlotsResponseDto>()
            .ForMember(dest => dest.AmenityId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AmenityName, opt => opt.MapFrom(src => src.Name))
            .ForMember(
                dest => dest.SlotType,
                opt => opt.MapFrom(src => src.SlotType != null ? src.SlotType.Code : string.Empty)
            )
            .ForMember(dest => dest.Slots, opt => opt.Ignore())
            .ForMember(dest => dest.Pagination, opt => opt.Ignore());
    }
}
