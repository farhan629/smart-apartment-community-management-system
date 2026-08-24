using AutoMapper;
using IdentityService.Application.Features.Approvals.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Approvals.Commands
{
    /// <summary>
    /// Admin approves or rejects a pending flat-occupancy request.
    /// Mirrors PUT /api/approval/{id}.
    /// </summary>
    public class UpdateApprovalCommand : IRequest<UpdateApprovalResponseDto>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the flat occupancy request to update.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the update request payload containing status and remarks.
        /// </summary>
        public UpdateApprovalRequestDto Request { get; set; } = null!;
    }

    /// <summary>
    /// Handler for processing the <see cref="UpdateApprovalCommand"/>.
    /// </summary>
    public class UpdateApprovalCommandHandler
        : IRequestHandler<UpdateApprovalCommand, UpdateApprovalResponseDto>
    {
        private readonly IFlatOccupancyRepository _flatOccupancyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateApprovalCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateApprovalCommandHandler"/> class.
        /// </summary>
        /// <param name="flatOccupancyRepository">The repository for flat occupancy requests.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public UpdateApprovalCommandHandler(
            IFlatOccupancyRepository flatOccupancyRepository,
            IMapper mapper,
            ILogger<UpdateApprovalCommandHandler> logger
        )
        {
            _flatOccupancyRepository = flatOccupancyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the execution of the update approval command.
        /// </summary>
        /// <param name="request">The command containing update parameters.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response DTO detailing the result of the update operation.</returns>
        public async Task<UpdateApprovalResponseDto> Handle(
            UpdateApprovalCommand request,
            CancellationToken cancellationToken
        )
        {
            FlatOccupancy occupancy =
                await _flatOccupancyRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException(
                    string.Format(
                        ExceptionMessages.EntityNotFound,
                        ExceptionMessages.ApprovalRecordEntityName
                    )
                );
            occupancy.IsApproved = request.Request.IsApproved;

            FlatOccupancy flatOccupancy = new FlatOccupancy
            {
                Id = occupancy.Id,
                UserId = occupancy.UserId,
                FlatId = occupancy.FlatId,
                ResidentTypeId = occupancy.ResidentTypeId,
                IsApproved = occupancy.IsApproved
                
            };
            await _flatOccupancyRepository.UpdateAsync(flatOccupancy);

            var message = request.Request.IsApproved
                ? ApprovalMessages.ApprovedSuccessfully
                : ApprovalMessages.RejectedSuccessfully;

            _logger.LogInformation(
                "Approval {ApprovalId} for user {UserId} set to {IsApproved}",
                occupancy.Id,
                occupancy.UserId,
                request.Request.IsApproved
            );

            var dto = _mapper.Map<ApprovalDetailDto>(occupancy);
            dto.Remarks = request.Request.Remarks;

            return new UpdateApprovalResponseDto { Message = message, Approval = dto };
        }
    }
}
