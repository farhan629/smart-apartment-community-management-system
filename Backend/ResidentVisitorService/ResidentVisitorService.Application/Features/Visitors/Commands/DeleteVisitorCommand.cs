using MediatR;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Interfaces.Repositories;

namespace ResidentVisitorService.Application.Features.Visitors.Commands;

/// <summary>Command to soft-delete (deactivate) a visitor.</summary>
public class DeleteVisitorCommand : IRequest
{
    public Guid Id { get; set; }
}

/// <summary>Handles the <see cref="DeleteVisitorCommand"/> request.</summary>
public class DeleteVisitorCommandHandler : IRequestHandler<DeleteVisitorCommand>
{
    private readonly IVisitorRepository _visitorRepository;
    private readonly ILogger<DeleteVisitorCommandHandler> _logger;

    public DeleteVisitorCommandHandler(
        IVisitorRepository visitorRepository,
        ILogger<DeleteVisitorCommandHandler> logger
    )
    {
        _visitorRepository = visitorRepository;
        _logger = logger;
    }

    public async Task Handle(DeleteVisitorCommand command, CancellationToken cancellationToken)
    {
        var deleted = await _visitorRepository.SoftDeleteAsync(command.Id, cancellationToken);
        if (!deleted)
        {
            throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitorNotFound, command.Id)
            );
        }

        _logger.LogInformation("Soft-deleted visitor {VisitorId}", command.Id);
    }
}
