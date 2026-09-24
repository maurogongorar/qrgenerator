using Cocosoft.Cli.Tools.QRGenerator.Commands.DependencyInjection;

namespace Cocosoft.Cli.Tools.QRGenerator.Commands;

internal abstract class BaseCommandHandler : ICommandHandler
{
    public abstract Task<int> InvokeAsync(InvocationContext context);
}
