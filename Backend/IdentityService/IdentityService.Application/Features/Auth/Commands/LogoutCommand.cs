using IdentityService.Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;
using Shared.SharedLibrary.Services;

namespace IdentityService.Application.Features.Auth.Commands
{
    /// <summary>
    /// Command used to log out the currently authenticated user.
    /// </summary>
    public class LogoutCommand : IRequest<Unit> { }

    /// <summary>
    /// Handles user logout by deactivating the user's refresh token.
    /// </summary>
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<LogoutCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutCommandHandler"/> class.
        /// </summary>
        /// <param name="refreshTokenRepository">
        /// Repository used to manage refresh tokens.
        /// </param>
        /// <param name="currentUserService">
        /// Service used to retrieve information about the currently authenticated user.
        /// </param>
        /// <param name="logger">
        /// Logger used to record logout events.
        /// </param>
        public LogoutCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ICurrentUserService currentUserService,
            ILogger<LogoutCommandHandler> logger
        )
        {
            _refreshTokenRepository = refreshTokenRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        /// <summary>
        /// Logs out the current user by deactivating their refresh token.
        /// </summary>
        /// <param name="request">
        /// The logout command.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel the operation.
        /// </param>
        /// <returns>
        /// <see cref="Unit.Value"/> when the logout operation completes successfully.
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when the current user is not authenticated or has an invalid user identifier.
        /// </exception>
        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            if (userId == Guid.Empty)
                throw new UnauthorizedException(ExceptionMessages.InvalidRefreshToken);
            await _refreshTokenRepository.DeactivateAsync(userId, userId, cancellationToken);

            _logger.LogInformation("User {UserId} logged out, refresh token deactivated", userId);

            return Unit.Value;
        }
    }
}
