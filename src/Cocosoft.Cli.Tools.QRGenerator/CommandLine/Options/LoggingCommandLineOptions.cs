using Cocosoft.Cli.Tools.QRGenerator.Commands.DependencyInjection.Extensions;
using System.CommandLine;
using System.Reflection;

namespace Cocosoft.Cli.Tools.QRGenerator.CommandLine.Options;

internal static class LoggingCommandLineOptions
{
    public static IEnumerable<(Option Option, string OptionName)> All
    {
        get
        {
            foreach (var option in typeof(LoggingCommandLineOptions)
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(p => p.PropertyType.IsAssignableTo(typeof(Option))))
            {
                var cliOptionAttr = option.GetCustomAttribute<CliOptionAttribute>();
                if (cliOptionAttr != null)
                {
                    yield return ((Option)option.GetValue(null)!, cliOptionAttr.Name);
                }
            }
        }
    }

    [CliOption(nameof(LoggingOptions.Verbose))]
    public static Option<bool> VerboseOption { get; } = new("--verbose", "-v")
    {
        Description = "Enable verbose logging output.",
        Recursive = true
    };

    public static IDictionary<string, string?> BuildConfiguration(string[] args)
    {
        var parsingCommand = new RootCommand
        {
            TreatUnmatchedTokensAsErrors = false,
        };
        
        var allOptions = All.ToList();

        foreach (var option in allOptions)
        {
            parsingCommand.Options.Add(option.Option);
        }

        var parseResult = parsingCommand.Parse(args);
        var configuration = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var option in allOptions)
        {
            configuration.AddIfNotEmpty(option.OptionName, parseResult.GetParsedValue(option.Option)?.ToString());
        }

        return configuration;
    }

    private static void AddIfNotEmpty(this IDictionary<string, string?> configuration, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            configuration[$"{LoggingOptions.SectionName}:{key}"] = value;
        }
    }
}
