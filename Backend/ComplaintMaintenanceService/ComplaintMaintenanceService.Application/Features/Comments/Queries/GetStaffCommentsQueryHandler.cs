using ComplaintMaintenanceService.Application.Features.Comments.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;

namespace ComplaintMaintenanceService.Application.Features.Comments.Queries;

public class GetStaffCommentsQuery : IRequest<List<CommentResponseDto>>
{
    public Guid StaffId { get; set; }
}

public class GetStaffCommentsQueryHandler
    : IRequestHandler<GetStaffCommentsQuery, List<CommentResponseDto>>
{
    private readonly IComplaintCommentRepository _commentRepo;

    public GetStaffCommentsQueryHandler(IComplaintCommentRepository commentRepo)
    {
        _commentRepo = commentRepo;
    }

    public async Task<List<CommentResponseDto>> Handle(
        GetStaffCommentsQuery query,
        CancellationToken ct
    )
    {
        var comments = await _commentRepo.GetByStaffIdAsync(query.StaffId, ct);

        return comments
            .Select(c => new CommentResponseDto
            {
                CommentId = c.Id,
                ComplaintId = c.ComplaintId,
                CommentedBy = c.CommentedBy,
                CommentText = c.CommentText,
                StaffRating = c.StaffRating,
                CreatedAt = c.CreatedAt,
            })
            .ToList();
    }
}
