using Microsoft.Extensions.Logging;
using QRCoder;
using Cocosoft.Cli.Tools.QRGenerator.Commands.DependencyInjection;
using Cocosoft.Cli.Tools.QRGenerator.Services;
using System.Drawing;

namespace Cocosoft.Cli.Tools.QRGenerator.Commands;

internal class GenerateWhatsAppQrCommandHandler(
    IQrGeneratorService qrGeneratorService,
    ILogger<GenerateWhatsAppQrCommandHandler> logger)
    : BaseCommandHandler
{
    public Color DarkColor { get; set; }

    [Option(IgnoreNull = true)]
    public CustomIcon Icon { get; set; } = CustomIcon.Whatsapp;

    public string? IconPath { get; set; }

    public Color LightColor { get; set; }

    public string? Message { get; set; }

    public string? OutputFile { get; set; }

    public string? PhoneNumber { get; set; }

    public override async Task<int> InvokeAsync(InvocationContext context)
    {
        logger.LogInformation("Starting to generate WhatsApp message QR.");
        logger.LogDebug(
            "PhoneNumber: {PhoneNumber}, Message: {Message}, OutputFile: {OutputFile}",
            PhoneNumber,
            Message,
            OutputFile);
        var outputPath = OutputFile ?? "qrcode.png";

        try
        {
            var payload = new PayloadGenerator.WhatsAppMessage(PhoneNumber ?? string.Empty, Message ?? string.Empty);
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
            logger.LogError(ex, "Failed to generate WhatsApp message QR");
            return 1;
        }

        logger.LogInformation("Successfully generated WhatsApp message QR at {OutputPath}.", outputPath);
        return 0;
    }
}
