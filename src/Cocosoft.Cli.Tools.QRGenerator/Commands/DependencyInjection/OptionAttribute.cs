namespace Cocosoft.Cli.Tools.QRGenerator.Commands.DependencyInjection;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
internal class OptionAttribute : Attribute
{
    public bool IgnoreNull { get; set; } = false;
}
