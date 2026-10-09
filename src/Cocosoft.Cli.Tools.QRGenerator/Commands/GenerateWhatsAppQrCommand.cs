using System.CommandLine;

namespace Cocosoft.Cli.Tools.QRGenerator.Commands;

internal class GenerateWhatsAppQrCommand : Command
{
    public GenerateWhatsAppQrCommand() : base("whatsapp", "Generates a QR code for a WhatsApp message")
    {
        Options.Add(new Option<string>("--phone-number", "-p")
        {
            Description = "The phone number to send the message to.",
            Required = true
        });
        Arguments.Add(new Argument<string>("message") { Description = "The message to send." });
    }
}
