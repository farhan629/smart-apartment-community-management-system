using AutoMapper;
using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Staff.DTOs;
using ComplaintMaintenanceService.Application.Features.StaffAvailability.DTOs;
using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile mapping StaffAvailability entity to response DTOs.
/// </summary>
public class StaffAvailabilityMappingProfile : Profile
{
    public StaffAvailabilityMappingProfile()
    {
        CreateMap<StaffAvailability, AvailabilitySlotResponseDto>()
            .ForMember(d => d.SlotId, opt => opt.MapFrom(s => s.Id))
            .ForMember(
                d => d.Date,
                opt =>
                    opt.MapFrom(s =>
                        s.AvailableDate.ToString(ComplaintConstants.DateFormats.OutputDate)
                    )
            )
            .ForMember(
                d => d.StartTime,
                opt =>
                    opt.MapFrom(s =>
                        s.SlotStartTime.ToString(ComplaintConstants.DateFormats.OutputTime)
                    )
            )
            .ForMember(
                d => d.EndTime,
                opt =>
                    opt.MapFrom(s =>
                        s.SlotEndTime.ToString(ComplaintConstants.DateFormats.OutputTime)
                    )
            )
            .ForMember(
                d => d.StaffName,
                opt => opt.MapFrom(s => s.Staff != null ? s.Staff.Description : string.Empty)
            )
            .ForMember(
                d => d.Category,
                opt =>
                    opt.MapFrom(s =>
                        s.Staff != null && s.Staff.Category != null
                            ? s.Staff.Category.Name
                            : string.Empty
                    )
            )
            .ForMember(
                d => d.CategoryId,
                opt => opt.MapFrom(s => s.Staff != null ? s.Staff.CategoryId : Guid.Empty)
            );

        CreateMap<StaffAvailability, StaffAvailabilityResponseDto>()
            .ForMember(d => d.SlotId, opt => opt.MapFrom(s => s.Id))
            .ForMember(
                d => d.AvailableDate,
                opt =>
                    opt.MapFrom(s =>
                        s.AvailableDate.ToString(ComplaintConstants.DateFormats.OutputDate)
                    )
            )
            .ForMember(
                d => d.SlotStartTime,
                opt =>
                    opt.MapFrom(s =>
                        s.SlotStartTime.ToString(ComplaintConstants.DateFormats.OutputTime)
                    )
            )
            .ForMember(
                d => d.SlotEndTime,
                opt =>
                    opt.MapFrom(s =>
                        s.SlotEndTime.ToString(ComplaintConstants.DateFormats.OutputTime)
                    )
            );
    }
}
