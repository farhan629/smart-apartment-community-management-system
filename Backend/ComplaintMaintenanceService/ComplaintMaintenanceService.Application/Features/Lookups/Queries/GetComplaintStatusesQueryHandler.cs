using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Lookups.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;

namespace ComplaintMaintenanceService.Application.Features.Lookups.Queries;

public class GetComplaintStatusesQuery : IRequest<List<RefTermLookupDto>> { }

public class GetComplaintStatusesQueryHandler
    : IRequestHandler<GetComplaintStatusesQuery, List<RefTermLookupDto>>
{
    private readonly IRefTermRepository _refTermRepo;

    public GetComplaintStatusesQueryHandler(IRefTermRepository refTermRepo)
    {
        _refTermRepo = refTermRepo;
    }

    public async Task<List<RefTermLookupDto>> Handle(
        GetComplaintStatusesQuery query,
        CancellationToken ct
    )
    {
        var terms = await _refTermRepo.GetByRefSetIdAsync(
            ComplaintConstants.RefSetIds.ComplaintStatus
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