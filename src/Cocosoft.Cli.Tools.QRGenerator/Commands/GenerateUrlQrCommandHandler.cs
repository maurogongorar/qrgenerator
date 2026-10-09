using Microsoft.Extensions.Logging;
using QRCoder;
using Cocosoft.Cli.Tools.QRGenerator.Commands.DependencyInjection;
using Cocosoft.Cli.Tools.QRGenerator.Services;
using System.Drawing;

namespace Cocosoft.Cli.Tools.QRGenerator.Commands;

internal class GenerateUrlQrCommandHandler(
    IQrGeneratorService qrGeneratorService,
    ILogger<GenerateUrlQrCommandHandler> logger)
    : BaseCommandHandler
{
    public Color DarkColor { get; set; }

    public CustomIcon? Icon { get; set; }

    public string? IconPath { get; set; }

    public Color LightColor { get; set; }

    public string? OutputFile { get; set; }

    public string? Url { get; set; }

    public override async Task<int> InvokeAsync(InvocationContext context)
    {
        logger.LogInformation("Starting to generate URL QR.");
        logger.LogDebug("URL: {URL}, OutputFile: {OutputFile}", Url, OutputFile);
        var outputPath = OutputFile ?? "qrcode.png";

        try
        {
            var payload = new PayloadGenerator.Url(Url ?? string.Empty);
            var qrPng = await qrGeneratorService.GenerateQrCode(
                payload,
                Icon,
                IconPath,
                DarkColor,
                LightColor,
                context.GetCancellationToken());
            await File.WriteAllBytesAsync(outputPath, qrPng, context.GetCancellationToken());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate URL QR");
            return 1;
        }

        logger.LogInformation("Successfully generated URL QR at {OutputPath}.", outputPath);
        return 0;
    }
}
