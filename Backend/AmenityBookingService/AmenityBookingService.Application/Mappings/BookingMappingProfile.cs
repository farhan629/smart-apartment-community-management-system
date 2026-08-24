using AmenityBookingService.Application.Features.Bookings.Commands;
using AmenityBookingService.Application.Features.Bookings.DTO;
using AmenityBookingService.Domain.Entities;
using AutoMapper;

namespace AmenityBookingService.Application.Mappings;

/// <summary>
/// Defines AutoMapper mappings for booking-related commands, entities, and DTOs.
/// </summary>
public class BookingMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookingMappingProfile"/> class.
    /// </summary>
    public BookingMappingProfile()
    {
        CreateMap<CreateBookingRequestDto, CreateBookingCommand>();

        CreateMap<CreateBookingCommand, AmenityBooking>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AmenitySlotId, opt => opt.MapFrom(src => src.SlotId))
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.BookingStatusId, opt => opt.Ignore())
            .ForMember(dest => dest.CancelledAt, opt => opt.Ignore())
            .ForMember(dest => dest.CancellationReason, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.AmenitySlot, opt => opt.Ignore())
            .ForMember(dest => dest.BookingStatus, opt => opt.Ignore());

        CreateMap<AmenityBooking, BookingResponseDto>()
            .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(
                dest => dest.AmenityName,
                opt =>
                    opt.MapFrom(src =>
                        src.AmenitySlot != null && src.AmenitySlot.Amenity != null
                            ? src.AmenitySlot.Amenity.Name
                            : string.Empty
                    )
            )
            .ForMember(
                dest => dest.SlotType,
                opt =>
                    opt.MapFrom(src =>
                        src.AmenitySlot != null
                        && src.AmenitySlot.Amenity != null
                        && src.AmenitySlot.Amenity.SlotType != null
                            ? src.AmenitySlot.Amenity.SlotType.Code
                            : string.Empty
                    )
            )
            .ForMember(
                dest => dest.SlotLabel,
                opt =>
                    opt.MapFrom(src =>
                        src.AmenitySlot != null ? src.AmenitySlot.SlotLabel : string.Empty
                    )
            )
            .ForMember(
                dest => dest.SlotDate,
                opt =>
                    opt.MapFrom(src =>
                        src.AmenitySlot != null ? src.AmenitySlot.SlotDate : DateTime.MinValue
                    )
            )
            .ForMember(
                dest => dest.StartTime,
                opt =>
                    opt.MapFrom(src =>
                        src.AmenitySlot != null ? src.AmenitySlot.StartTime : TimeSpan.Zero
                    )
            )
            .ForMember(
                dest => dest.EndTime,
                opt =>
                    opt.MapFrom(src =>
                        src.AmenitySlot != null ? src.AmenitySlot.EndTime : TimeSpan.Zero
                    )
            )
            .ForMember(
                dest => dest.Status,
                opt =>
                    opt.MapFrom(src =>
                        src.BookingStatus != null ? src.BookingStatus.Code : string.Empty
                    )
            )
            .ForMember(dest => dest.BookedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}
