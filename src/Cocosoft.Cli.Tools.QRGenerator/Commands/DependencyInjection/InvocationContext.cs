namespace Cocosoft.Cli.Tools.QRGenerator.Commands.DependencyInjection;

internal class InvocationContext(CancellationToken cancellationToken)
{
    public CancellationToken GetCancellationToken() => cancellationToken;
}
