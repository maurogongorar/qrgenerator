using System.CommandLine;

namespace Cocosoft.Cli.Tools.QRGenerator.Commands;

internal class GenerateUrlQrCommand : Command
{
    public GenerateUrlQrCommand() : base("url", "Generates a QR code for a given URL")
    {
        Arguments.Add(new Argument<string>("url") { Description = "The URL to encode in the QR code." });
    }
}
