using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visitors.Services;

namespace ResidentVisitorService.Infrastructure.Services;

/// <summary>
/// Saves visitor photo files to the local wwwroot folder and returns a servable relative URL.
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly string _baseUrl;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(
        IWebHostEnvironment environment,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<FileStorageService> logger
    )
    {
        _environment = environment;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;

        var subPath =
            configuration[ResidentVisitorConstants.FileStorage.PhotoPathConfigKey]
            ?? throw new InvalidOperationException(
                ResidentVisitorConstants.Errors.FileStoragePathMissing
            );

        _basePath = Path.Combine(environment.WebRootPath, subPath);
        _baseUrl = $"/{subPath.Replace('\\', '/')}";

        Directory.CreateDirectory(_basePath);
    }

    /// <inheritdoc/>
    public async Task<string> SaveVisitorPhotoAsync(
        Guid visitorId,
        Stream fileStream,
        string extension,
        CancellationToken cancellationToken = default
    )
    {
        var fileName = $"{visitorId}{extension}";
        var filePath = Path.Combine(_basePath, fileName);

        await using var outputStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None
        );
        await fileStream.CopyToAsync(outputStream, cancellationToken);

        _logger.LogInformation(
            "Saved photo for visitor {VisitorId} at {FilePath}",
            visitorId,
            filePath
        );

        return $"{_baseUrl}/{fileName}";
    }

    public async Task<string> SaveAsync(
        Stream stream,
        string folder,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        var folderPath = Path.Combine(_environment.WebRootPath, folder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        await using var outputStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None
        );
        await stream.CopyToAsync(outputStream, cancellationToken);

        _logger.LogInformation("Saved file to {FilePath}", filePath);

        var request = _httpContextAccessor.HttpContext?.Request;
        if (request != null)
        {
            var scheme = request.Scheme;
            var host = request.Host.Value;
            return $"{scheme}://{host}/{folder.Replace('\\', '/')}/{fileName}";
        }

        var configBaseUrl = _configuration["FileStorage:BaseUrl"] ?? "http://localhost:5064";
        return $"{configBaseUrl.TrimEnd('/')}/{folder.Replace('\\', '/')}/{fileName}";
    }
}
