using ComplaintMaintenanceService.Application.Features.Lookups.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;

namespace ComplaintMaintenanceService.Application.Features.Lookups.Queries;

public class GetCategoriesQuery : IRequest<List<CategoryLookupDto>> { }

public class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, List<CategoryLookupDto>>
{
    private readonly ICategoryRepository _categoryRepo;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    public async Task<List<CategoryLookupDto>> Handle(
        GetCategoriesQuery query,
        CancellationToken ct
    )
    {
        var categories = await _categoryRepo.GetAllAsync();

        return categories
            .Select(c => new CategoryLookupDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Img = c.Img,
            })
            .ToList();
    }
}