using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Cocosoft.Cli.Tools.QRGenerator.CommandLine.Options;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using System.Globalization;

namespace Cocosoft.Cli.Tools.QRGenerator.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCliOptionsConfiguration(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(LoggingCommandLineOptions.BuildConfiguration(Environment.GetCommandLineArgs()))
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        return services;
    }

    public static IServiceCollection AddSerilogLogging(this IServiceCollection services)
    {
        services.AddOptions<LoggingOptions>().BindConfiguration(LoggingOptions.SectionName);
        return services.AddLogging(builder => builder.AddSerilog())
            .AddSerilog((sp, configuration) =>
            {
                var options = sp.GetRequiredService<IOptions<LoggingOptions>>().Value;

                var levelSwitch = new LoggingLevelSwitch
                {
                    MinimumLevel = options.Verbose ? LogEventLevel.Verbose : LogEventLevel.Information
                };

                configuration
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.FromLogContext()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails();
            });
    }
}
