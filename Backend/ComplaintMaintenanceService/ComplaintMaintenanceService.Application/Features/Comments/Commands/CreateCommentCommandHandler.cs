using ComplaintMaintenanceService.Application.Common.Constants;
using ComplaintMaintenanceService.Application.Features.Comments.DTOs;
using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using ComplaintMaintenanceService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shared.SharedLibrary.Exceptions;

namespace ComplaintMaintenanceService.Application.Features.Comments.Commands;

public class CreateCommentCommand : IRequest<CommentResponseDto>
{
    public Guid ComplaintId { get; set; }
    public Guid CommentedBy { get; set; }
    public CreateCommentRequestDto Request { get; set; } = default!;
}

public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, CommentResponseDto>
{
    private readonly IComplaintRepository _complaintRepo;
    private readonly IComplaintCommentRepository _commentRepo;
    private readonly IComplaintAssignmentRepository _assignmentRepo;
    private readonly IServiceScopeFactory _scopeFactory;

    public CreateCommentCommandHandler(
        IComplaintRepository complaintRepo,
        IComplaintCommentRepository commentRepo,
        IComplaintAssignmentRepository assignmentRepo,
        IServiceScopeFactory scopeFactory
    )
    {
        _complaintRepo = complaintRepo;
        _commentRepo = commentRepo;
        _assignmentRepo = assignmentRepo;
        _scopeFactory = scopeFactory;
    }

    public async Task<CommentResponseDto> Handle(CreateCommentCommand cmd, CancellationToken ct)
    {
        var complaint =
            await _complaintRepo.GetByIdAsync(cmd.ComplaintId, ct)
            ?? throw new NotFoundException(ComplaintConstants.Messages.ComplaintNotFound);

        if (cmd.Request.StaffRating.HasValue)
        {
            var existing = await _commentRepo.GetRatingByComplaintIdAsync(
                cmd.ComplaintId,
                cmd.CommentedBy,
                ct
            );
            if (existing is not null)
                throw new ConflictException(ComplaintConstants.RatingMessages.AlreadyRated);
        }

        var now = DateTime.UtcNow;
        var comment = new ComplaintComment
        {
            Id = Guid.NewGuid(),
            ComplaintId = cmd.ComplaintId,
            CommentedBy = cmd.CommentedBy,
            CommentText = cmd.Request.CommentText,
            StaffRating = cmd.Request.StaffRating,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = cmd.CommentedBy,
            UpdatedBy = cmd.CommentedBy,
        };

        await _commentRepo.AddAsync(comment, ct);

        var complaintId = cmd.ComplaintId;
        var commentedBy = cmd.CommentedBy;
        var commentText = cmd.Request.CommentText;
        var staffRating = cmd.Request.StaffRating;

        _ = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var identityClient = scope.ServiceProvider.GetRequiredService<IIdentityGrpcClient>();
            var notificationClient =
                scope.ServiceProvider.GetRequiredService<INotificationGrpcClient>();
            var assignmentRepo =
                scope.ServiceProvider.GetRequiredService<IComplaintAssignmentRepository>();

            try
            {
                var activeAssignment = await assignmentRepo.GetActiveByComplaintIdAsync(
                    complaintId,
                    CancellationToken.None
                );
                var residentUser = await identityClient.GetUserByIdAsync(
                    commentedBy,
                    CancellationToken.None
                );
                var admins = await identityClient.GetUsersByRoleAsync(
                    ComplaintConstants.RoleCodes.Admin,
                    CancellationToken.None
                );

                string staffName = string.Empty;
                string residentName = residentUser?.Name ?? string.Empty;

                if (activeAssignment is not null)
                {
                    var staffUser = await identityClient.GetUserByIdAsync(
                        activeAssignment.Staff!.UserId,
                        CancellationToken.None
                    );
                    staffName = staffUser?.Name ?? string.Empty;
                }

                if (staffRating.HasValue)
                {
                    foreach (var admin in admins)
                    {
                        await notificationClient.PushNotificationAsync(
                            admin.UserId,
                            ComplaintConstants.NotificationTypes.ComplaintRatingDone,
                            ComplaintConstants.NotificationTitles.ComplaintRatingDone,
                            string.Format(
                                ComplaintConstants.NotificationMessages.ComplaintRatingDone,
                                staffName,
                                complaintId,
                                staffRating.Value
                            ),
                            complaintId,
                            admin.Email,
                            admin.Name,
                            CancellationToken.None
                        );
                    }
                }
                else
                {
                    foreach (var admin in admins)
                    {
                        await notificationClient.PushNotificationAsync(
                            admin.UserId,
                            ComplaintConstants.NotificationTypes.ComplaintRaised,
                            "New Comment on Complaint",
                            $"Resident {residentName} commented on complaint #{complaintId}: \"{commentText}\"",
                            complaintId,
                            admin.Email,
                            admin.Name,
                            CancellationToken.None
                        );
                    }
                }
            }
            catch { }
        });

        return new CommentResponseDto
        {
            CommentId = comment.Id,
            ComplaintId = comment.ComplaintId,
            CommentedBy = comment.CommentedBy,
            CommentText = comment.CommentText,
            StaffRating = comment.StaffRating,
            CreatedAt = comment.CreatedAt,
        };
    }
}
