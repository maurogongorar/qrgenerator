using Microsoft.Extensions.Logging;
using Cocosoft.Cli.Tools.QRGenerator.Commands.DependencyInjection;
using Cocosoft.Cli.Tools.QRGenerator.Services;
using System.Drawing;

namespace Cocosoft.Cli.Tools.QRGenerator.Commands;

internal class MainCommandHandler(
    IQrGeneratorService qrGeneratorService,
    ILogger<MainCommandHandler> logger)
    : BaseCommandHandler
{
    public Color DarkColor { get; set; }

    public CustomIcon? Icon { get; set; }

    public string? IconPath { get; set; }

    public string? Input { get; set; }

    public Color LightColor { get; set; }

    public string? OutputFile { get; set; }

    public override async Task<int> InvokeAsync(InvocationContext context)
    {
        logger.LogInformation("Starting to generate Plain Text QR.");
        logger.LogDebug("Input: {Input}, OutputFile: {OutputFile}", Input, OutputFile);
        var outputPath = OutputFile ?? "qrcode.png";

        try
        {
            var qrPng = await qrGeneratorService.GenerateQrCode(
                Input ?? string.Empty,
                Icon,
                IconPath,
                DarkColor,
                LightColor,
                context.GetCancellationToken());
            await File.WriteAllBytesAsync(outputPath, qrPng, context.GetCancellationToken());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate Plain Text QR");
            return 1;
        }

        logger.LogInformation("Successfully generated Plain Text QR at {OutputPath}.", outputPath);
        return 0;
    }
}
