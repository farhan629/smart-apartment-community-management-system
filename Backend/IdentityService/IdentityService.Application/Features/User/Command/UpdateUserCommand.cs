using AutoMapper;
using IdentityService.Application.Features.Users.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Users.Commands
{
    /// <summary>
    /// Command to update user profile details.
    /// </summary>
    public class UpdateUserCommand : IRequest<UserDto>
    {
        /// <summary>Gets or sets the unique identifier of the user to update.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the update user request payload.</summary>
        public UpdateUserRequestDto Request { get; set; } = null!;
    }

    /// <summary>
    /// Handler for processing the <see cref="UpdateUserCommand"/>.
    /// </summary>
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateUserCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUserCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public UpdateUserCommandHandler(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UpdateUserCommandHandler> logger
        )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the execution of the update user command.
        /// </summary>
        /// <param name="request">The command details.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A UserDto representing the updated user.</returns>
        public async Task<UserDto> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            var user =
                await _userRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException(ExceptionMessages.NotFound);

            var dto = request.Request;
            if (!string.IsNullOrWhiteSpace(dto.UserName))
                user.Name = dto.UserName;
            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.PhoneNo = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.PhotoUrl))
                user.PhotoUrl = dto.PhotoUrl;

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Updated user {UserId}", request.Id);
            return _mapper.Map<UserDto>(user);
        }
    }
}
