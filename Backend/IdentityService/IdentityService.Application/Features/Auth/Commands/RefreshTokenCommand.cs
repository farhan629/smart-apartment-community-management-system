using AutoMapper;
using IdentityService.Application.Features.Auth.DTOs;
using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.Application.Features.Auth.Commands
{
    /// <summary>
    /// Command used to generate a new access token and refresh token
    /// using a valid refresh token.
    /// </summary>
    public class RefreshTokenCommand : IRequest<AuthResponseDto>
    {
        /// <summary>
        /// /// The refresh token provided by the client.
        /// </summary>
        public string RefreshToken { get; set; } = null!;
    }

    /// <summary>
    /// Handles refresh token validation, token rotation,
    /// and generation of new authentication tokens.
    /// </summary>
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">
        /// Repository used to retrieve user information.
        /// </param>
        /// <param name="refreshTokenRepository">
        /// Repository used to manage refresh tokens.
        /// </param>
        /// <param name="jwtService">
        /// Service used to generate access and refresh tokens.
        /// </param>
        /// <param name="mapper">
        /// AutoMapper instance used for object mapping.
        /// </param>
        /// <param name="logger">
        /// Logger used to record refresh token operations.
        /// </param>
        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService,
            IMapper mapper,
            ILogger<RefreshTokenCommandHandler> logger
        )
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Validates the provided refresh token, generates a new access token,
        /// rotates the refresh token, and returns updated authentication details.
        /// </summary>
        /// <param name="request">
        /// The refresh token command containing the refresh token.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel the operation.
        /// </param>
        /// <returns>
        /// An <see cref="AuthResponseDto"/> containing the new access token,
        /// refresh token, expiration information, and user details.
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when the refresh token is invalid, expired,
        /// or the associated user account is inactive.
        /// </exception>
        public async Task<AuthResponseDto> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken
        )
        {
            var stored = await _refreshTokenRepository.GetByTokenKeyAsync(
                request.RefreshToken,
                cancellationToken
            );

            if (stored is null || !stored.IsActive)
                throw new UnauthorizedException(ExceptionMessages.InvalidRefreshToken);

            if (stored.ExpiryAt < DateTime.UtcNow)
                throw new UnauthorizedException(ExceptionMessages.RefreshTokenExpired);

            var user = await _userRepository.GetByIdAsync(stored.UserId);

            if (user is null || !user.IsActive)
                throw new UnauthorizedException(ExceptionMessages.AccountDeactivated);

            var response = _mapper.Map<AuthResponseDto>(user);
            response.UserId = user.Id;

            response.Token = _jwtService.GenerateAccessToken(user);
            response.ExpiresAt = DateTime.UtcNow.AddMinutes(
                _jwtService.GetAccessTokenExpiryMinutes()
            );

            var newRefreshToken = _jwtService.GenerateRefreshToken(user);
            var newExpiry = DateTime.UtcNow.AddDays(_jwtService.GetRefreshTokenExpiryDays());

            await _refreshTokenRepository.UpsertAsync(
                user.Id,
                newRefreshToken,
                newExpiry,
                user.Id,
                cancellationToken
            );

            response.RefreshToken = newRefreshToken;

            _logger.LogInformation("Refresh token rotated for user {UserId}", user.Id);

            return response;
        }
    }
}
