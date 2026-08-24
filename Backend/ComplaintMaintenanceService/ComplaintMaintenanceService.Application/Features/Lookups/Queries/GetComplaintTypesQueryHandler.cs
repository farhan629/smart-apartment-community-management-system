using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Lookups.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;

namespace ComplaintMaintenanceService.Application.Features.Lookups.Queries;

public class GetComplaintTypesQuery : IRequest<List<RefTermLookupDto>> { }

public class GetComplaintTypesQueryHandler
    : IRequestHandler<GetComplaintTypesQuery, List<RefTermLookupDto>>
{
    private readonly IRefTermRepository _refTermRepo;

    public GetComplaintTypesQueryHandler(IRefTermRepository refTermRepo)
    {
        _refTermRepo = refTermRepo;
    }

    public async Task<List<RefTermLookupDto>> Handle(
        GetComplaintTypesQuery query,
        CancellationToken ct
    )
    {
        var terms = await _refTermRepo.GetByRefSetIdAsync(
            ComplaintConstants.RefSetIds.ComplaintType
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
