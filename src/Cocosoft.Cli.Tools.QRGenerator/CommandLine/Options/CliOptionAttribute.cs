namespace Cocosoft.Cli.Tools.QRGenerator.CommandLine.Options;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
internal class CliOptionAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
