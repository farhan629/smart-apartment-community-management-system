using QRCoder;
using ResidentVisitorService.Application.Features.Visitors.Services;
using ResidentVisitorService.Application.Interfaces.Services;

namespace ResidentVisitorService.Infrastructure.Services;

public class QrCodeService : IQrCodeService
{
    private readonly IFileStorageService _fileStorageService;
    private const string FolderName = "qrcodes";

    public QrCodeService(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    public async Task<string> GenerateAndStoreAsync(
        string token,
        CancellationToken cancellationToken = default
    )
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(token, QRCodeGenerator.ECCLevel.Q);
        var pngQr = new PngByteQRCode(data);
        var bytes = pngQr.GetGraphic(20);

        var fileName = $"{token}.png";
        using var stream = new MemoryStream(bytes);

        var url = await _fileStorageService.SaveAsync(
            stream,
            FolderName,
            fileName,
            cancellationToken
        );
        return url;
    }
}
