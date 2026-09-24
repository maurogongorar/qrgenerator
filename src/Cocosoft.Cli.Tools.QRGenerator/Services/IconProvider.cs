using QRCoder;
using System.Globalization;

namespace Cocosoft.Cli.Tools.QRGenerator.Services;

internal class IconProvider : IIconProvider
{
    public async Task<SvgQRCode.SvgLogo> GetIcon(
        CustomIcon icon,
        string? path = null,
        CancellationToken cancellationToken = default)
    {
        var svg = icon switch
        {
            CustomIcon.Custom => await File.ReadAllTextAsync(
                path ?? throw new ArgumentNullException(nameof(path), "Path cannot be null or empty for Custom icon."),
                cancellationToken),
            _ => await File.ReadAllTextAsync(GetIconPath(icon), cancellationToken)
        };

        return new SvgQRCode.SvgLogo(
            iconVectorized: svg,
            iconSizePercent: 20,
            fillLogoBackground: true,
            iconEmbedded: true);
    }

    private static string GetIconPath(CustomIcon icon)
    {
        return Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            icon.ToString().ToLower(CultureInfo.InvariantCulture) + ".svg");
    }
}
