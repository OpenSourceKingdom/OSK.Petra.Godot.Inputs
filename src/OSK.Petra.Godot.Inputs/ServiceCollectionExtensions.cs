using Microsoft.Extensions.DependencyInjection;
using OSK.Operations.Workflows;
using OSK.Petra.Godot.Inputs.Internal.Services;
using OSK.Petra.Inputs;
using OSK.Petra.Inputs.Ports;
using System;

namespace OSK.Petra.Godot.Inputs;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds an input system using default settings
    /// </summary>
    /// <param name="services">The services to add the system to</param>
    /// <returns>The services for chaining</returns>
    public static IServiceCollection AddGodotInputSystem(this IServiceCollection services)
        => services.AddGodotInputSystem(_ => { });

    /// <summary>
    /// Adds an input system using configured system settings
    /// </summary>
    /// <param name="services">The services to add the system to</param>
    /// <param name="configurator">The action that configures the system</param>
    /// <returns>The services for chaining</returns>
    public static IServiceCollection AddGodotInputSystem(this IServiceCollection services, Action<IInputSystemConfigurator> configurator)
    {
        services.AddWorkflows();
        services.AddInputSystem(config => 
        {
            configurator(config);
            config.WithDeviceProvider<DeviceProvider>();
        });

        return services;
    }
}
