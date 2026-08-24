using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Lookups.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;

namespace ComplaintMaintenanceService.Application.Features.Lookups.Queries;

public class GetComplaintPrioritiesQuery : IRequest<List<RefTermLookupDto>> { }

public class GetComplaintPrioritiesQueryHandler
    : IRequestHandler<GetComplaintPrioritiesQuery, List<RefTermLookupDto>>
{
    private readonly IRefTermRepository _refTermRepo;

    public GetComplaintPrioritiesQueryHandler(IRefTermRepository refTermRepo)
    {
        _refTermRepo = refTermRepo;
    }

    public async Task<List<RefTermLookupDto>> Handle(
        GetComplaintPrioritiesQuery query,
        CancellationToken ct
    )
    {
        var terms = await _refTermRepo.GetByRefSetIdAsync(
            ComplaintConstants.RefSetIds.ComplaintPriority
        );

        return terms
            .Select(t => new RefTermLookupDto
            {
                Id = t.Id,
                Code = t.Code,
                DisplayName = t.DisplayName,
            })
            .ToList();
    }
}