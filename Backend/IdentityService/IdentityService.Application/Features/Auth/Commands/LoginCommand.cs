using AutoMapper;
using IdentityService.Application.Features.Auth.DTOs;
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
    /// Command used to authenticate a user and generate access and refresh tokens.
    /// </summary>
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        /// <summary>
        /// Contains the user's login credentials.
        /// </summary>
        public LoginRequestDto Request { get; set; } = null!;
    }

    /// <summary>
    /// Handles the login process including user validation,
    /// password verification, token generation, and refresh token storage.
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginCommandHandler> _logger;

        /// <summary>
        /// Number of days before the refresh token expires.
        /// </summary>
        private const int RefreshTokenExpiryDays = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginCommandHandler"/> class.
        /// </summary>
        /// <param name="userRepository">Repository used to retrieve user information.</param>
        /// <param name="refreshTokenRepository">Repository used to manage refresh tokens.</param>
        /// <param name="jwtService">Service used to generate JWT access and refresh tokens.</param>
        /// <param name="passwordService">Service used to verify password hashes.</param>
        /// <param name="mapper">AutoMapper instance used for object mapping.</param>
        /// <param name="logger">Logger for recording login events.</param>
        public LoginCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService,
            IPasswordService passwordService,
            IMapper mapper,
            ILogger<LoginCommandHandler> logger
        )
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
            _passwordService = passwordService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates the user, validates account status,
        /// generates JWT tokens, and returns authentication details.
        /// </summary>
        /// <param name="request">The login command containing user credentials.</param>
        /// <param name="cancellationToken">Token used to cancel the operation.</param>
        /// <returns>
        /// An <see cref="AuthResponseDto"/> containing user details,
        /// access token, refresh token, and token expiration information.
        /// </returns>
        public async Task<AuthResponseDto> Handle(
            LoginCommand request,
            CancellationToken cancellationToken
        )
        {
            var dto = request.Request;
            if (dto.Email == null)
            {
                throw new UnauthorizedException(ExceptionMessages.EmailNotEntered);
            }
            var user = await _userRepository.GetUserWithCredentialAsync(dto.Email!.Trim());

            if (user?.PasswordSecurity == null)
                throw new UnauthorizedException(ExceptionMessages.EmailNotRegistered);

            if (!_passwordService.VerifyPassword(dto.Password, user.PasswordSecurity.PasswordHash))
                throw new UnauthorizedException(ExceptionMessages.InvalidPassword);

            if (!user.IsActive)
                throw new UnauthorizedException(ExceptionMessages.AccountDeactivated);

            if (user.Role?.RefSetId == RefSetIds.OccupantSetId)
            {
                var isApproved =
                    user.FlatOccupancies?.Any(o => o.IsApproved && o.IsActive) ?? false;
                if (!isApproved)
                    throw new UnauthorizedException(ExceptionMessages.OccupancyNotApproved);
            }

            var response = _mapper.Map<AuthResponseDto>(user);
            response.Token = _jwtService.GenerateAccessToken(user);

            var refreshToken = _jwtService.GenerateRefreshToken(user);
            var refreshExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);

            await _refreshTokenRepository.UpsertAsync(
                user.Id,
                refreshToken,
                refreshExpiry,
                user.Id,
                cancellationToken
            );

            response.RefreshToken = refreshToken;
            response.ExpiresAt = DateTime.UtcNow.AddMinutes(
                _jwtService.GetAccessTokenExpiryMinutes()
            );

            _logger.LogInformation("Login successful for {Email}", dto.Email.Trim());

            return response;
        }
    }
}
