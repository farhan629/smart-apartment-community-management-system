using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visitors.DTOs;
using ResidentVisitorService.Application.Interfaces.Repositories;
using ResidentVisitorService.Domain.Entities;

namespace ResidentVisitorService.Application.Features.Visitors.Commands;

/// <summary>Command to create a new visitor record.</summary>
public class CreateVisitorCommand : IRequest<VisitorResponseDto>
{
    public CreateVisitorRequestDto Request { get; set; } = null!;
}

/// <summary>Handles the <see cref="CreateVisitorCommand"/> request.</summary>
public class CreateVisitorCommandHandler : IRequestHandler<CreateVisitorCommand, VisitorResponseDto>
{
    private readonly IVisitorRepository _visitorRepository;
    private readonly IRefTermRepository _refTermRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateVisitorCommandHandler> _logger;

    public CreateVisitorCommandHandler(
        IVisitorRepository visitorRepository,
        IRefTermRepository refTermRepository,
        IMapper mapper,
        ILogger<CreateVisitorCommandHandler> logger
    )
    {
        _visitorRepository = visitorRepository;
        _refTermRepository = refTermRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<VisitorResponseDto> Handle(
        CreateVisitorCommand command,
        CancellationToken cancellationToken
    )
    {
        var request = command.Request;

        var existing = await _visitorRepository.GetByPhoneNumberAsync(
            request.PhoneNumber.Trim(),
            cancellationToken
        );
        if (existing is not null)
        {
            _logger.LogInformation(
                "Visitor with phone '{PhoneNumber}' already exists — returning existing visitor {VisitorId}",
                request.PhoneNumber,
                existing.Id
            );

            return _mapper.Map<VisitorResponseDto>(existing);
        }

        var visitorType =
            await _refTermRepository.GetByIdAsync(request.VisitorTypeId, cancellationToken)
            ?? throw new KeyNotFoundException(
                string.Format(
                    ResidentVisitorConstants.Errors.VisitorTypeNotFound,
                    request.VisitorTypeId
                )
            );

        var visitor = new Visitor
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = request.Email?.Trim(),
            VisitorTypeId = request.VisitorTypeId,
        };

        var created = await _visitorRepository.AddAsync(visitor, cancellationToken);

        _logger.LogInformation(
            "Created visitor {VisitorId} — name: {Name}",
            created.Id,
            created.Name
        );

        created.VisitorType = visitorType;

        return _mapper.Map<VisitorResponseDto>(created);
    }
}
