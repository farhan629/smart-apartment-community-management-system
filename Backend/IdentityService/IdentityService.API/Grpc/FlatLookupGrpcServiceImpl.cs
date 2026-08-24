using Grpc.Core;
using IdentityService.Application.Interfaces.Repositories;
using Shared.Grpc;

namespace IdentityService.API.Grpc;

/// <summary>
/// gRPC server that lets other services resolve flat and resident information
/// by block/flat number (for Security walk-ins) or by user ID (for Residents).
/// </summary>
public class FlatLookupGrpcServiceImpl : FlatLookupGrpcService.FlatLookupGrpcServiceBase
{
    private readonly IFlatRepository _flatRepository;
    private readonly IFlatOccupancyRepository _occupancyRepository;
    private readonly ILogger<FlatLookupGrpcServiceImpl> _logger;

    public FlatLookupGrpcServiceImpl(
        IFlatRepository flatRepository,
        IFlatOccupancyRepository occupancyRepository,
        ILogger<FlatLookupGrpcServiceImpl> logger
    )
    {
        _flatRepository = flatRepository;
        _occupancyRepository = occupancyRepository;
        _logger = logger;
    }

    /// <summary>
    /// Resolves the flat and approved resident from a block number and flat number.
    /// Called by ResidentVisitorService when Security registers a walk-in visit.
    /// </summary>
    public override async Task<FlatLookupResponse> GetFlatByBlockAndNumber(
        FlatLookupRequest request,
        ServerCallContext context
    )
    {
        _logger.LogInformation(
            "gRPC FlatLookup by block/flat: Block={Block}, FlatNumber={FlatNumber}",
            request.Block,
            request.FlatNumber
        );

        var flat =
            await _flatRepository.GetByNumberAndBlockAsync(request.FlatNumber, request.Block)
            ?? throw new RpcException(
                new Status(
                    StatusCode.NotFound,
                    $"No flat found for Block '{request.Block}', Flat '{request.FlatNumber}'."
                )
            );

        var (_, occupancies) = await _occupancyRepository.GetAllAsync(
            page: 1,
            limit: 10,
            status: "approved",
            userId: null
        );

        var occupancy =
            occupancies.FirstOrDefault(o => o.FlatId == flat.Id)
            ?? throw new RpcException(
                new Status(
                    StatusCode.NotFound,
                    $"No approved resident found in Block '{request.Block}', Flat '{request.FlatNumber}'."
                )
            );

        return new FlatLookupResponse
        {
            FlatId = flat.Id.ToString(),
            HostUserId = occupancy.UserId.ToString(),
            ResidentName = occupancy.User?.Name ?? string.Empty,
            ResidentEmail = occupancy.User?.Email ?? string.Empty,
            Block = flat.Block,
            FlatNumber = flat.Number,
        };
    }

    /// <summary>
    /// Resolves the flat that a resident occupies from their user ID.
    /// Called by ResidentVisitorService when a Resident registers a planned visit.
    /// </summary>
    public override async Task<FlatLookupResponse> GetFlatByUserId(
        FlatByUserIdRequest request,
        ServerCallContext context
    )
    {
        _logger.LogInformation("gRPC FlatLookup by userId: UserId={UserId}", request.UserId);

        var userId = Guid.Parse(request.UserId);

        var (_, occupancies) = await _occupancyRepository.GetAllAsync(
            page: 1,
            limit: 1,
            status: "approved",
            userId: userId
        );

        var occupancy =
            occupancies.FirstOrDefault()
            ?? throw new RpcException(
                new Status(
                    StatusCode.NotFound,
                    $"No approved flat occupancy found for user '{request.UserId}'."
                )
            );

        return new FlatLookupResponse
        {
            FlatId = occupancy.FlatId.ToString(),
            HostUserId = userId.ToString(),
            ResidentName = occupancy.User?.Name ?? string.Empty,
            ResidentEmail = occupancy.User?.Email ?? string.Empty,
            Block = occupancy.Flat?.Block ?? string.Empty,
            FlatNumber = occupancy.Flat?.Number ?? string.Empty,
        };
    }
}
