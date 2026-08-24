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
    /// Command to soft-delete a user by their unique identifier.
    /// </summary>
    public class DeleteUserCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }

    /// <summary>
    /// Handler for processing the <see cref="DeleteUserCommand"/>.
    /// </summary>
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteUserCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="logger">The logger instance.</param>
        public DeleteUserCommandHandler(
            IUserRepository userRepository,
            ILogger<DeleteUserCommandHandler> logger
        )
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the execution of the delete user command.
        /// </summary>
        /// <param name="request">The command details.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A Unit value representing completion.</returns>
        public async Task<Unit> Handle(
            DeleteUserCommand request,
            CancellationToken cancellationToken
        )
        {
            var exists =
                await _userRepository.GetByIdAsync(request.Id)
                ?? throw new NotFoundException(ExceptionMessages.NotFound);

            await _userRepository.DeleteAsync(request.Id);

            _logger.LogInformation("Deleted user {UserId}", request.Id);
            return Unit.Value;
        }
    }
}
