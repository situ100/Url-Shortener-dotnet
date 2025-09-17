namespace Application.Services;

public interface IQrCodeService
{
    byte[] GeneratePng(string content, int pixelsPerModule = 10);
}