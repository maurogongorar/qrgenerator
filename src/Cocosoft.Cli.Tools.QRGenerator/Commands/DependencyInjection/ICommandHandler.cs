namespace Cocosoft.Cli.Tools.QRGenerator.Commands.DependencyInjection;

internal interface ICommandHandler
{
    Task<int> InvokeAsync(InvocationContext context);
}
