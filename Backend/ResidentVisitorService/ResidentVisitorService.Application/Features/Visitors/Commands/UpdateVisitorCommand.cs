using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visitors.DTOs;
using ResidentVisitorService.Application.Interfaces.Repositories;

namespace ResidentVisitorService.Application.Features.Visitors.Commands;

/// <summary>Command to update an existing visitor record.</summary>
public class UpdateVisitorCommand : IRequest<VisitorResponseDto>
{
    public Guid Id { get; set; }
    public UpdateVisitorRequestDto Request { get; set; } = null!;
}

/// <summary>Handles the <see cref="UpdateVisitorCommand"/> request.</summary>
public class UpdateVisitorCommandHandler : IRequestHandler<UpdateVisitorCommand, VisitorResponseDto>
{
    private readonly IVisitorRepository _visitorRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateVisitorCommandHandler> _logger;

    public UpdateVisitorCommandHandler(
        IVisitorRepository visitorRepository,
        IRefTermRepository refTermRepository,
        IMapper mapper,
        ILogger<UpdateVisitorCommandHandler> logger
    )
    {
        _visitorRepository = visitorRepository;
        _refTermRepository = refTermRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<VisitorResponseDto> Handle(
        UpdateVisitorCommand command,
        CancellationToken cancellationToken
    )
    {
        var visitor =
            await _visitorRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(ResidentVisitorConstants.Errors.VisitorNotFound, command.Id)
            );

        var request = command.Request;

        if (
            !string.IsNullOrWhiteSpace(request.PhoneNumber)
            && request.PhoneNumber != visitor.PhoneNumber
            && await _visitorRepository.PhoneNumberExistsAsync(
                request.PhoneNumber,
                command.Id,
                cancellationToken
            )
        )
        {
            throw new InvalidOperationException(
                string.Format(
                    ResidentVisitorConstants.Errors.PhoneNumberAlreadyExists,
                    request.PhoneNumber
                )
            );
        }

        if (request.VisitorTypeId.HasValue && request.VisitorTypeId.Value != visitor.VisitorTypeId)
        {
            var visitorType =
                await _refTermRepository.GetByIdAsync(
                    request.VisitorTypeId.Value,
                    cancellationToken
                )
                ?? throw new KeyNotFoundException(
                    string.Format(
                        ResidentVisitorConstants.Errors.VisitorTypeNotFound,
                        request.VisitorTypeId
                    )
                );
            visitor.VisitorTypeId = request.VisitorTypeId.Value;
            visitor.VisitorType = visitorType;
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
            visitor.Name = request.Name.Trim();
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            visitor.PhoneNumber = request.PhoneNumber.Trim();
        if (request.Email is not null)
            visitor.Email = request.Email.Trim();

        await _visitorRepository.UpdateAsync(visitor, cancellationToken);

        _logger.LogInformation("Updated visitor {VisitorId}", visitor.Id);

        return _mapper.Map<VisitorResponseDto>(visitor);
    }
}
