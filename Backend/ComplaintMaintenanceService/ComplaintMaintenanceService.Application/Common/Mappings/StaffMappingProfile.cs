using AutoMapper;
using ComplaintMaintenanceService.Application.Features.Staff.DTOs;
using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile mapping Staff entity to StaffResponseDto and StaffSummaryDto.
/// </summary>
public class StaffMappingProfile : Profile
{
    public StaffMappingProfile()
    {
        CreateMap<Staff, StaffResponseDto>()
            .ForMember(d => d.StaffId, opt => opt.MapFrom(s => s.Id))
            .ForMember(
                d => d.CategoryName,
                opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty)
            );

        CreateMap<Staff, StaffSummaryDto>()
            .ForMember(d => d.StaffId, opt => opt.MapFrom(s => s.Id))
            .ForMember(
                d => d.CategoryName,
                opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty)
            );
    }
}
