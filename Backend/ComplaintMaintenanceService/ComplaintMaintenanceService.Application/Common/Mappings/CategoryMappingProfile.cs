using AutoMapper;
using ComplaintMaintenanceService.Domain.Entities;

namespace ComplaintMaintenanceService.Application.Common.Mappings;

/// <summary>
/// AutoMapper profile for Category entity.
/// in CmsGrpcService (Infrastructure layer) where proto types are accessible.
/// </summary>
public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile() { }
}
