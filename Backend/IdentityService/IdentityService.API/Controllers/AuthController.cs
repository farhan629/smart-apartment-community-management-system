using IdentityService.Application.Features.Auth.Commands;
using IdentityService.Application.Features.Auth.DTOs;
using IdentityService.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Attributes;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Exceptions;

namespace IdentityService.API.Controllers
{
    /// <summary>
    /// Handles authentication operations including user registration and login.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;
        private readonly IJwtService _jwtService;

        public AuthController(
            IMediator mediator,
            ILogger<AuthController> logger,
            IJwtService jwtService
        )
        {
            _mediator = mediator;
            _logger = logger;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Registers a new Occupant (Owner / Tenant) in the system.
        /// Role must be resolved from GET /role/occupant.
        /// </summary>
        [HttpPost("signup")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SuccessResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Signup([FromForm] RegisterRequestDto request)
        {
            var result = await _mediator.Send(new SignupCommand { Request = request });
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// Registers a new Management user (Admin / Staff). Admin access only.
        /// Role must be resolved from GET /role/managment.
        /// </summary>
        [HttpPost("register")]
        [PermissionAuthorize(PermissionConst.USER_MANAGE)]
        [ProducesResponseType(typeof(SuccessResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Register([FromForm] RegisterManagementRequestDto request)
        {
            var result = await _mediator.Send(new RegisterCommand { Request = request });
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT access token in the body.
        /// Refresh token and user ID are set as HttpOnly, Secure, SameSite=Lax cookies
        /// for automatic submission on refresh-token requests.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _mediator.Send(new LoginCommand { Request = request });

            SetAuthCookies(result.RefreshToken, result.UserId);

            return Ok(new { result.Token, result.ExpiresAt });
        }

        /// <summary>
        /// Refreshes an expired access token using the refresh token stored in the
        /// HttpOnly cookie. The refresh token is rotated on each successful refresh.
        /// If the token is expired or invalid, the cookies are cleared and a 401 is returned.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var userIdRaw = Request.Cookies["userId"];

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(userIdRaw))
                throw new UnauthorizedException(ErrorMessages.NoRefreshTokenProvided);

            try
            {
                var result = await _mediator.Send(
                    new RefreshTokenCommand { RefreshToken = refreshToken }
                );

                SetAuthCookies(result.RefreshToken, result.UserId);

                return Ok(new { result.Token, result.ExpiresAt });
            }
            catch (UnauthorizedException)
            {
                ClearAuthCookies();
                throw new UnauthorizedException(ErrorMessages.RefreshTokenExpiredOrInvalid);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _mediator.Send(new LogoutCommand());
            ClearAuthCookies();
            return NoContent();
        }

        /// <summary>
        /// Changes the password for the currently authenticated user.
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var result = await _mediator.Send(new ChangePasswordCommand { Request = request });
            return Ok(result);
        }

        /// <summary>
        /// Initiates forgot password flow by sending OTP to registered phone and email.
        /// </summary>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SuccessResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var result = await _mediator.Send(new ForgotPasswordCommand { Request = request });
            return Ok(result);
        }

        /// <summary>
        /// Verifies the OTP and returns a reset token for password reset.
        /// </summary>
        [HttpPost("verify-otp")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(VerifyOtpResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request)
        {
            var result = await _mediator.Send(new VerifyOtpCommand { Request = request });
            return Ok(result);
        }

        /// <summary>
        /// Resets the password using a valid reset token obtained from OTP verification.
        /// </summary>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SuccessResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            var result = await _mediator.Send(new ResetPasswordCommand { Request = request });
            return Ok(result);
        }

        /// <summary>
        /// Sets HttpOnly cookies for refresh token and user ID.
        /// Expiry matches the configured RefreshExpiryDays from appsettings.
        /// </summary>
        private void SetAuthCookies(string refreshToken, Guid userId)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(_jwtService.GetRefreshTokenExpiryDays()),
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
            Response.Cookies.Append("userId", userId.ToString(), cookieOptions);
        }

        /// <summary>
        /// Clears the auth cookies by setting them to expire in the past.
        /// </summary>
        private void ClearAuthCookies()
        {
            var clearOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(-1),
            };

            Response.Cookies.Append("refreshToken", "", clearOptions);
            Response.Cookies.Append("userId", "", clearOptions);
        }
    }
}
