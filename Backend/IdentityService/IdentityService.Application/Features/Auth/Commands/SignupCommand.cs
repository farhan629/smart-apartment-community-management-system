using AutoMapper;
using FluentValidation;
using IdentityService.Application.Features.Auth.DTOs;
using IdentityService.Application.Features.Auth.Validators;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Auth.Commands
{
    /// <summary>
    /// Command for registration/signup of an occupant (Owner / Tenant).
    /// </summary>
    public class SignupCommand : IRequest<SuccessResponseDto>
    {
        /// <summary>
        /// Gets or sets the registration payload for the occupant.
        /// </summary>
        public RegisterRequestDto Request { get; set; } = null!;
    }

    /// <summary>
    /// Handler for processing the <see cref="SignupCommand"/>.
    /// </summary>
    public class SignupCommandHandler : IRequestHandler<SignupCommand, SuccessResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFlatRepository _flatRepository;
        private readonly IFlatOccupancyRepository _flatOccupancyRepository;
        private readonly IRefTermRepository _refTermRepository;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        private readonly ILogger<SignupCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignupCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="flatRepository">The flat repository.</param>
        /// <param name="flatOccupancyRepository">The flat occupancy repository.</param>
        /// <param name="refTermRepository">The reference term repository.</param>
        /// <param name="passwordService">The password security service.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public SignupCommandHandler(
            IUserRepository userRepository,
            IFlatRepository flatRepository,
            IFlatOccupancyRepository flatOccupancyRepository,
            IRefTermRepository refTermRepository,
            IPasswordService passwordService,
            IMapper mapper,
            ILogger<SignupCommandHandler> logger
        )
        {
            _userRepository = userRepository;
            _flatRepository = flatRepository;
            _flatOccupancyRepository = flatOccupancyRepository;
            _refTermRepository = refTermRepository;
            _passwordService = passwordService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the execution of the occupant signup command.
        /// </summary>
        /// <param name="request">The command containing signup details.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A response DTO detailing success and pending status.</returns>
        public async Task<SuccessResponseDto> Handle(
            SignupCommand request,
            CancellationToken cancellationToken
        )
        {
            var dto = request.Request;

            if (await _userRepository.EmailExistsAsync(dto.Email!.Trim()))
                throw new BadRequestException(ExceptionMessages.EmailAlreadyExists);

            if (dto.Role_id is null)
                throw new BadRequestException(ExceptionMessages.InvalidRole);

            var role =
                await _refTermRepository.GetByIdAsync(dto.Role_id.Value)
                ?? throw new BadRequestException(ExceptionMessages.InvalidRole);

            if (role.RefSetId != RefSetIds.OccupantSetId)
                throw new BadRequestException(ExceptionMessages.InvalidRole);

            if (dto.Flat_id is null)
                throw new BadRequestException(ExceptionMessages.FlatRequired);

            var flat =
                await _flatRepository.GetByIdAsync(dto.Flat_id.Value)
                ?? throw new NotFoundException(ExceptionMessages.NotFound);

            var existingOccupancy =
                await _flatOccupancyRepository.GetActiveOccupancyByFlatAndRoleAsync(
                    flat.Id,
                    role.Id
                );
            if (existingOccupancy != null)
            {
                if (!existingOccupancy.IsApproved)
                {
                    throw new BadRequestException(ExceptionMessages.FlatOccupancyPendingExists);
                }
                else
                {
                    throw new BadRequestException(ExceptionMessages.AlreadyFlatOccupied);
                }
            }

            var userId = Guid.NewGuid();

            var user = _mapper.Map<User>(dto);

            user.Id = userId;
            user.Email = dto.Email.Trim();
            user.RoleId = role.Id;

            var credential = new UserPasswordSecurity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PasswordHash = _passwordService.HashPassword(dto.Password!),
            };

            await _userRepository.AddUserWithCredentialAsync(user, credential);

            var occupancy = new FlatOccupancy
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FlatId = flat.Id,
                ResidentTypeId = role.Id,
                IsApproved = false,
            };

            await _flatOccupancyRepository.AddAsync(occupancy);

            _logger.LogInformation(
                "Occupant signup successful for {Email} — FlatOccupancy {OccupancyId} pending approval",
                dto.Email.Trim(),
                occupancy.Id
            );

            return new SuccessResponseDto
            {
                Message = SuccessMessages.FlatRegistrationPendingApproval,
            };
        }
    }
}
