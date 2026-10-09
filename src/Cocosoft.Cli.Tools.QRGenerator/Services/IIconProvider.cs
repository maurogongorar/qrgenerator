using QRCoder;

namespace Cocosoft.Cli.Tools.QRGenerator.Services;

internal interface IIconProvider
{
    Task<SvgQRCode.SvgLogo> GetIcon(
        CustomIcon icon,
        string? path = null,
        CancellationToken cancellationToken = default);
}
