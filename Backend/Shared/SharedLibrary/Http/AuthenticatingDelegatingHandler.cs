using Microsoft.AspNetCore.Http;

namespace Shared.SharedLibrary.Http;

/// <summary>
/// Forwards the incoming HTTP request's Authorization header to outgoing HttpClient calls.
/// Ensures the internal service-to-service call carries the original bearer token.
/// </summary>
public class AuthenticatingDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticatingDelegatingHandler"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor to read the incoming request headers.</param>
    public AuthenticatingDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Sends the HTTP request, stamping the original Authorization header if available.
    /// </summary>
    /// <param name="request">The outgoing HTTP request message.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The HTTP response message.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            var authHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
            if (authHeader is not null)
            {
                request.Headers.TryAddWithoutValidation("Authorization", authHeader);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
