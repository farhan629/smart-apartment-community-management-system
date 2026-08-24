using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Comments.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using MediatR;

namespace ComplaintMaintenanceService.Application.Features.Comments.Queries;

public class GetCommentsQuery : IRequest<List<CommentResponseDto>>
{
    public Guid ComplaintId { get; set; }
}

public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, List<CommentResponseDto>>
{
    private readonly IComplaintCommentRepository _commentRepo;

    public GetCommentsQueryHandler(IComplaintCommentRepository commentRepo)
    {
        _commentRepo = commentRepo;
    }

    public async Task<List<CommentResponseDto>> Handle(GetCommentsQuery query, CancellationToken ct)
    {
        var comments = await _commentRepo.GetByComplaintIdAsync(query.ComplaintId, ct);

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
