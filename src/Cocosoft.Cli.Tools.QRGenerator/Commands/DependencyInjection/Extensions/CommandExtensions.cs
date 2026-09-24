using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.Reflection;

namespace Cocosoft.Cli.Tools.QRGenerator.Commands.DependencyInjection.Extensions;


internal static class CommandExtensions
{
    public static readonly MethodInfo ParseResultGetOptionValue =
        typeof(ParseResult).GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Single(m => m.Name == nameof(ParseResult.GetValue) &&
            m.IsGenericMethod &&
            m.GetParameters().Length == 1 &&
            m.GetParameters()[0].ParameterType.IsGenericType &&
            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Option<>));

    private static readonly MethodInfo ParseResultArgumentGetValue =
        typeof(ParseResult).GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Single(m => m.Name == nameof(ParseResult.GetValue) &&
            m.IsGenericMethod &&
            m.GetParameters().Length == 1 &&
            m.GetParameters()[0].ParameterType.IsGenericType &&
            m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Argument<>));

    public static IServiceCollection AddCommand<TCommand, TCommandHandler>(this IServiceCollection services)
        where TCommand : Command
        where TCommandHandler : class, ICommandHandler
        => services.AddCommand<Command, TCommand, TCommandHandler>();

    public static IServiceCollection AddRootCommand<TCommand, TCommandHandler>(this IServiceCollection services)
        where TCommand : RootCommand
        where TCommandHandler : class, ICommandHandler
    {
        return services.AddCommand<RootCommand, TCommand, TCommandHandler>();
    }

    private static IServiceCollection AddCommand<TService, TCommand, TCommandHandler>(this IServiceCollection services)
        where TService : Command
        where TCommand : TService
        where TCommandHandler : class, ICommandHandler
    {
        services.AddTransient<TCommand>();
        services.AddTransient<TService>(sp =>
        {
            var command = sp.GetRequiredService<TCommand>();
            var handler = sp.GetRequiredService<TCommandHandler>();

            command.SetAction((parseResult, ct) =>
            {
                var context = new InvocationContext(ct);
                var handlerType = typeof(TCommandHandler);

                var options = command.Options.AsEnumerable();
                foreach (var parent in command.Parents.OfType<Command>())
                {
                    options = options.Concat(parent.Options.Where(o => o.Recursive));
                }

                foreach (var option in options)
                {
                    var optType = option.GetType();

                    if (!optType.IsGenericType)
                    {
                        continue;
                    }

                    var propName = option.Name.ToPascalCaseFromKebabCase();
                    var propInfo = handlerType.GetProperty(
                        propName,
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.IgnoreCase);
                    var value = parseResult.GetParsedValue(option);

                    if (propInfo is null ||
                    (value is null && propInfo.GetCustomAttribute<OptionAttribute>() is { IgnoreNull: true }))
                    {
                        continue;
                    }

                    propInfo?.SetValue(handler, value);
                }

                foreach (var argument in command.Arguments)
                {
                    var propName = argument.Name.ToPascalCaseFromKebabCase();
                    var propInfo = handlerType.GetProperty(
                        propName,
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.IgnoreCase);
                    var value = parseResult.GetParsedValue(argument);
                    propInfo?.SetValue(handler, value);
                }

                return handler.InvokeAsync(context);
            });
            return command;
        });
        services.AddTransient<TCommandHandler>();
        return services;
    }

    public static object? GetParsedValue(this ParseResult parseResult, Argument argument)
    {
        var argType = argument.GetType();
        if (!argType.IsGenericType)
        {
            return null;
        }

        var argValueType = argType.GetGenericArguments()[0];
        return ParseResultArgumentGetValue.MakeGenericMethod(argValueType).Invoke(parseResult, [argument]);
    }

    public static object? GetParsedValue(this ParseResult parseResult, Option option)
    {
        var optType = option.GetType();
        if (!optType.IsGenericType)
        {
            return null;
        }

        var optValueType = optType.GetGenericArguments()[0];
        return ParseResultGetOptionValue.MakeGenericMethod(optValueType).Invoke(parseResult, [option]);
    }
}
