using AmenityBookingService.Domain.Entities;
using AutoMapper;

namespace AmenityBookingService.Application.Mappings;

/// <summary>
/// Defines AutoMapper projection mappings for booking-related domain entities.
/// </summary>
public class BookingProjectionProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BookingProjectionProfile"/> class.
    /// </summary>
    public BookingProjectionProfile()
    {
        CreateMap<AmenityBooking, AmenityBooking>();

        CreateMap<AmenitySlot, AmenitySlot>()
            .ForMember(dest => dest.AmenityBookings, opt => opt.Ignore());

        CreateMap<Amenity, Amenity>().ForMember(dest => dest.AmenitySlots, opt => opt.Ignore());

        CreateMap<RefTerm, RefTerm>()
            .ForMember(dest => dest.Amenities, opt => opt.Ignore())
            .ForMember(dest => dest.AmenityBookings, opt => opt.Ignore());
    }
}
