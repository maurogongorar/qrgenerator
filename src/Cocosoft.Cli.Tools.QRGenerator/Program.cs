using Microsoft.Extensions.DependencyInjection;
using QRCoder;
using Cocosoft.Cli.Tools.QRGenerator;
using Cocosoft.Cli.Tools.QRGenerator.Commands;
using Cocosoft.Cli.Tools.QRGenerator.Commands.DependencyInjection.Extensions;
using Cocosoft.Cli.Tools.QRGenerator.Extensions;
using Cocosoft.Cli.Tools.QRGenerator.Services;

var services = new ServiceCollection();
services.AddCliOptionsConfiguration();
services.AddSerilogLogging();

// Add other services and configurations as needed
services.AddRootCommand<MainCommand, MainCommandHandler>();
services.AddCommand<GenerateUrlQrCommand, GenerateUrlQrCommandHandler>();
services.AddCommand<GenerateWhatsAppQrCommand, GenerateWhatsAppQrCommandHandler>();
services.AddSingleton(new QRCodeGenerator());
services.AddSingleton<IIconProvider, IconProvider>();
services.AddSingleton<IQrGeneratorService, QrGeneratorService>();

using var serviceProvider = services.BuildServiceProvider();
var entryPoint = ActivatorUtilities.CreateInstance<EntryPoint>(serviceProvider);
await entryPoint.ExecuteAsync(args);