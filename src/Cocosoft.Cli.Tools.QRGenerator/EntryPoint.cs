using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace Cocosoft.Cli.Tools.QRGenerator;

internal class EntryPoint(RootCommand rootCommand, ILogger<EntryPoint> logger)
{
    public async Task ExecuteAsync(string[] args)
    {
        try
        {
            logger.LogDebug("Started QR code generator");
            await rootCommand.Parse(args).InvokeAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while executing the command");
            Environment.Exit(1);
        }
        finally
        {
            logger.LogDebug("Completed QR code generator");
        }
    }
}
