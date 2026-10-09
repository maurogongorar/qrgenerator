using Microsoft.Extensions.Logging;
using QRCoder;
using SkiaSharp;
using Svg.Skia;
using System.Drawing;

namespace Cocosoft.Cli.Tools.QRGenerator.Services;

internal class QrGeneratorService(
    QRCodeGenerator qrCodeGenerator,
    IIconProvider iconProvider,
    ILogger<QrGeneratorService> logger)
    : IQrGeneratorService
{
    public Task<ReadOnlyMemory<byte>> GenerateQrCode(
        PayloadGenerator.Payload payload,
        CustomIcon? icon,
        string? iconPath,
        Color darkColor = default,
        Color lightColor = default,
        CancellationToken cancellationToken = default) =>
        GetQrBytes(
            qrCodeGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.H), icon, iconPath, darkColor, lightColor, cancellationToken);

    public Task<ReadOnlyMemory<byte>> GenerateQrCode(
        string plainText,
        CustomIcon? icon,
        string? iconPath,
        Color darkColor = default,
        Color lightColor = default,
        CancellationToken cancellationToken = default) =>
        GetQrBytes(
            qrCodeGenerator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.H), icon, iconPath, darkColor, lightColor, cancellationToken);

    private async Task<ReadOnlyMemory<byte>> GetQrBytes(
        QRCodeData qrData,
        CustomIcon? icon,
        string? iconPath,
        Color darkColor,
        Color lightColor,
        CancellationToken cancellationToken)
    {
        darkColor = darkColor.IsEmpty ? Color.Black : darkColor;
        lightColor = lightColor.IsEmpty ? Color.White : lightColor;

        if (icon.HasValue)
        {
            logger.LogDebug("Generating QR code with icon {Icon} at path {IconPath}", icon.Value, iconPath);
            var logo = await iconProvider.GetIcon(icon.Value, iconPath, cancellationToken);
            var svg = new SvgQRCode(qrData)
                .GetGraphic(
                    pixelsPerModule: 18,
                    darkColor: darkColor,
                    lightColor: lightColor,
                    drawQuietZones: true,
                    logo: logo,
                    sizingMode: SvgQRCode.SizingMode.WidthHeightAttribute);
            return GetPng(svg);
        }
        
        logger.LogDebug("Generating QR code without icon");
        return new PngByteQRCode(qrData)
            .GetGraphic(
                pixelsPerModule: 20,
                darkColor: darkColor,
                lightColor: lightColor,
                drawQuietZones: true);
    }

    private ReadOnlyMemory<byte> GetPng(string svg)
    {
        logger.LogDebug("Converting SVG to PNG");
        using var skSvg = new SKSvg();
        skSvg.FromSvg(svg);

        if (skSvg.Picture == null)
        {
            throw new InvalidOperationException("Failed to load SVG.");
        }

        var bounds = skSvg.Picture.CullRect;
        var width = (int)bounds.Width;
        var height = (int)bounds.Height;

        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawPicture(skSvg.Picture);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
