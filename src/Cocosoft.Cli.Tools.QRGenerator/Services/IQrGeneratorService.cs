using QRCoder;
using System.Drawing;

namespace Cocosoft.Cli.Tools.QRGenerator.Services;

internal interface IQrGeneratorService
{
    Task<ReadOnlyMemory<byte>> GenerateQrCode(
        PayloadGenerator.Payload payload,
        CustomIcon? icon,
        string? iconPath,
        Color darkColor = default,
        Color lightColor = default,
        CancellationToken cancellationToken = default);

    Task<ReadOnlyMemory<byte>> GenerateQrCode(
        string plainText,
        CustomIcon? icon,
        string? iconPath,
        Color darkColor = default,
        Color lightColor = default,
        CancellationToken cancellationToken = default);
}
