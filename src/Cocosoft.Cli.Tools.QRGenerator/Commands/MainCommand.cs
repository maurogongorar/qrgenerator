using Cocosoft.Cli.Tools.QRGenerator.CommandLine.Options;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Drawing;

namespace Cocosoft.Cli.Tools.QRGenerator.Commands;

internal class MainCommand : RootCommand
{
    public MainCommand(IEnumerable<Command> subCommands) : base("Generate QR. A tool for generating QR codes.")
    {
        Arguments.Add(new Argument<string>("input") { Description = "The input text to encode in the QR code." });
        Options.Add(new Option<string?>("--output-file", "-o")
        {
            Description = "The output file path for the generated QR code image.",
            Recursive = true
        });
        Options.Add(new Option<CustomIcon?>("--icon", "-i")
        {
            Description = "The icon file path to include in the generated QR code image.",
            Recursive = true
        });
        Options.Add(new Option<string>("--icon-path")
        {
            Description = "The icon file path to include in the generated QR code image.",
            Recursive = true
        });
        Options.Add(new Option<Color>("--dark-color", "-d")
        {
            Description = "The dark color (Name like 'Black' or Hex like '#FF5733') of the QR code.",
            Recursive = true,
            CustomParser = ParseColor
        });
        Options.Add(new Option<Color>("--light-color", "-l")
        {
            Description = "The light color (Name like 'White' or Hex like '#FFFFFF') of the QR code.",
            Recursive = true,
            CustomParser = ParseColor
        });

        // Global logging options (bound to LoggingOptions through IConfiguration)
        foreach (var loggingOption in LoggingCommandLineOptions.All)
        {
            Options.Add(loggingOption.Option);
        }

        // Adds subcommands from the service provider
        AddSubcommands(subCommands);
    }

    private void AddSubcommands(IEnumerable<Command> subCommands)
    {
        foreach (var command in subCommands)
        {
            Subcommands.Add(command);
        }
    }

    private static Color ParseColor(ArgumentResult result)
    {
        var value = result.Tokens.SingleOrDefault()?.Value;

        if (string.IsNullOrWhiteSpace(value))
        {
            // If no value is provided, return Color.Empty to indicate that the default color should be used.
            return Color.Empty;
        }

        try
        {
            // Try to parse as a known color name
            var knownColor = Color.FromName(value);

            if (knownColor.IsKnownColor)
            {
                return knownColor;
            }

            // Try to parse as a hex color code
            if (value.StartsWith('#') && (value.Length == 7 || value.Length == 9))
            {
                return ColorTranslator.FromHtml(value);
            }

            result.AddError($"Invalid color value: {value}");
        }
        catch (Exception ex)
        {
            result.AddError($"Failed to parse color value: {value}. Error: {ex.Message}");
        }

        return Color.Empty;
    }
}
