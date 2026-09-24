namespace Cocosoft.Cli.Tools.QRGenerator.Commands.DependencyInjection.Extensions;

internal static class NamingExtensions
{
    public static string ToPascalCaseFromKebabCase(this string kebabCase)
    {
        if (string.IsNullOrEmpty(kebabCase))
        {
            return kebabCase;
        }

        return string.Concat(
            kebabCase.Split('-', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpper(word[0]) + word[1..].ToLower()));
    }
}
