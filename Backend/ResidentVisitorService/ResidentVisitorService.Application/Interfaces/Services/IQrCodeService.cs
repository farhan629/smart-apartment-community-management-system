namespace ResidentVisitorService.Application.Interfaces.Services;

public interface IQrCodeService
{
    /// <summary>Generates a PNG QR image for the given token and returns its stored URL.</summary>
    Task<string> GenerateAndStoreAsync(string token, CancellationToken cancellationToken = default);
}