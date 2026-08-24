using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.VisitQrToken.DTOs;
using ResidentVisitorService.Application.Interfaces.Repositories;

namespace ResidentVisitorService.Application.Features.VisitorTypes.Queries;

/// <summary>Query to retrieve all active visitor types.</summary>
public class GetVisitorTypesQuery : IRequest<IEnumerable<RefTermDto>> { }

/// <summary>Handles the <see cref="GetVisitorTypesQuery"/> request.</summary>
public class GetVisitorTypesQueryHandler
    : IRequestHandler<GetVisitorTypesQuery, IEnumerable<RefTermDto>>
{
    private readonly IRefTermRepository _refTermRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetVisitorTypesQueryHandler> _logger;

    public GetVisitorTypesQueryHandler(
        IRefTermRepository refTermRepository,
        IMapper mapper,
        ILogger<GetVisitorTypesQueryHandler> logger
    )
    {
        _refTermRepository = refTermRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<RefTermDto>> Handle(
        GetVisitorTypesQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Fetching all visitor types from ref set {RefSet}",
            ResidentVisitorConstants.RefSetCodes.VISITOR_TYPE
        );

        var terms = await _refTermRepository.GetByRefSetCodeAsync(
            ResidentVisitorConstants.RefSetCodes.VISITOR_TYPE,
            cancellationToken
        );

        return _mapper.Map<IEnumerable<RefTermDto>>(terms);
    }
}

/// <summary>Query to retrieve all active purpose types.</summary>
public class GetPurposeTypesQuery : IRequest<IEnumerable<RefTermDto>> { }

/// <summary>Handles the <see cref="GetPurposeTypesQuery"/> request.</summary>
public class GetPurposeTypesQueryHandler
    : IRequestHandler<GetPurposeTypesQuery, IEnumerable<RefTermDto>>
{
    private readonly IRefTermRepository _refTermRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPurposeTypesQueryHandler> _logger;

    public GetPurposeTypesQueryHandler(
        IRefTermRepository refTermRepository,
        IMapper mapper,
        ILogger<GetPurposeTypesQueryHandler> logger
    )
    {
        _refTermRepository = refTermRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<RefTermDto>> Handle(
        GetPurposeTypesQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Fetching all purpose types from ref set {RefSet}",
            ResidentVisitorConstants.RefSetCodes.PURPOSE_TYPE
        );

        var terms = await _refTermRepository.GetByRefSetCodeAsync(
            ResidentVisitorConstants.RefSetCodes.PURPOSE_TYPE,
            cancellationToken
        );

        return _mapper.Map<IEnumerable<RefTermDto>>(terms);
    }
}
