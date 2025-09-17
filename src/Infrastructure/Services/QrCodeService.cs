using Application.Services;
using QRCoder;

namespace Infrastructure.Services;

public class QrCodeService : IQrCodeService
{
    // Metoda do generowania kodu QR w formacie PNG
    public byte[] GeneratePng(string content, int pixelsPerModule = 10)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        var png = new PngByteQRCode(data);
        return png.GetGraphic(pixelsPerModule);
    }
}