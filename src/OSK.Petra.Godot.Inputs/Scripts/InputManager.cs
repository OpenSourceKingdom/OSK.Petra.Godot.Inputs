using Godot;
using OSK.Extensions.Petra.Godot.Roslyn;
using OSK.Extensions.Petra.Inputs.Configuration;
using OSK.Operations.Workflows.Managers;
using OSK.Operations.Workflows.Ports;
using OSK.Petra.DependencyInjection.Attributes;
using OSK.Petra.Godot.Inputs.Data;
using OSK.Petra.Godot.Inputs.Ports;
using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Capabilities.Power;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Ports;
using System;
using System.Linq;
using System.Threading.Tasks;
using OSK.Petra.Godot.Inputs.Data.Inputs;
using OSK.Petra.Inputs.Abstractions.Devices;

namespace OSK.Petra.Godot.Inputs.Scripts;

/// <summary>
/// Provides a base set of APIs/logic that <see cref="IInputManager"/> can use
/// </summary>
public abstract partial class InputManager: Node, IInputManager
{
    #region Variables

    [Export]
    private InputManagerConfiguration _configuration;

    /// <summary>
    /// The notifier for the input system
    /// </summary>
    [Inject]
    protected IInputSystemNotifier InputSystemNotifier { get; private set; }

    /// <summary>
    /// The input system
    /// </summary>
    [Inject]
    protected IInputSystem InputSystem;

    [Inject]
    private ITaskOperationManager _taskManager;

    #endregion

    #region Godot Overrides

    /// <inheritdoc/>
    public override void _Ready()
    {
        Input.JoyConnectionChanged += OnGamepadConnectionChange;

        _taskManager.Configure(settings =>
        {
            settings.Defaults.MaxConcurrentOperations = 1;
        });

        _taskManager.AddValueTask(() => ConfigureInputSystemAsync());
    }

    /// <inheritdoc/>
    public override void _Process(double delta)
    {
        var deltaTimespan = TimeSpan.FromSeconds(delta);
        _taskManager?.Update(deltaTimespan);

        if (InputSystem?.Configuration is null)
        {
            GD.PrintRich("[color=orange]Input System:[/color] input system or configuraion is null and unusable ...");
            return;
        }

        InputSystem.Update(deltaTimespan);
    }

    /// <inheritdoc/>
    public override void _Notification(int what)
    {
        if (what == NotificationApplicationFocusOut)
        {
            InputSystemNotifier?.Notify(new InputSystemFocusNotification(hasFocus: false));
        }
        if (what == NotificationApplicationFocusIn)
        {
            InputSystemNotifier?.Notify(new InputSystemFocusNotification(hasFocus: true));
        }
    }

    #endregion

    #region IInputManager

    /// <inheritdoc/>
    public InputManagerConfiguration Configuration => _configuration;

    #endregion

    #region Helpers

    private async ValueTask ConfigureInputSystemAsync()
    {
        var configuration = CreateInputSystemConfiguration();
        await InputSystem.InitializeAsync(configuration);
    }

    private InputSystemConfiguration CreateInputSystemConfiguration()
        => InputSystemConfigurationFactory.Create(inputSystemConifgurationBuilder =>
        {
            inputSystemConifgurationBuilder
                .WithJoinPolicy(new InputSystemJoinPolicy()
                {
                    DevicePairingBehavior = _configuration.DevicePairingBehavior,
                    UserJoinBehavior = _configuration.UserJoinBehavior,
                    MaxUsers = _configuration.MaxLocalPlayers
                });

            if (_configuration.DefaultPointerSettings is not null)
            {
                inputSystemConifgurationBuilder.WithCapabilityOptions<PointerCapabilityOptions>(o =>
                {
                    o.MaxPositionEntries = _configuration.DefaultPointerSettings.MaxRecordsPerPointer;
                });
            }
            if (_configuration.DefaultPointerSettings is not null)
            {
                inputSystemConifgurationBuilder.WithCapabilityOptions<PowerCapabilityOptions>(o =>
                {
                    o.ReactivationTime = _configuration.DefaultPowerSettings.TapReactivationTime;
                    o.ActiveTimeThreshold = _configuration.DefaultPowerSettings.ActiveTimeThreshold;
                });
            }

            foreach (var definition in _configuration.ActionDefinitions)
            {
                inputSystemConifgurationBuilder.WithDefinition(definition.Name, definition.ActionScript.GetScriptType(), definitionBuilder =>
                {
                    if (definition.IsDefault)
                    {
                        definitionBuilder.MakeDefault();
                    }

                    foreach (var scheme in definition.InputSchemes)
                    {
                        definitionBuilder.WithScheme(scheme.Name, schemeBuilder =>
                        {
                            if (scheme.IsDefault)
                            {
                                schemeBuilder.MakeDefault();
                            }

                            var inputMaps = scheme.DevicesMaps.SelectMany(deviceMap =>
                            {
                                var identity = deviceMap.GetDeviceIdentity();
                                return deviceMap.InputMaps.Select(map => new { Identity = identity, Map = map });
                            });

                            foreach (var inputMap in inputMaps)
                            {
                                switch (inputMap.Map)
                                {
                                    case PointerInputResource pointerResource:
                                        schemeBuilder.WithMap(inputMap.Identity, pointerResource.InputId, pointerResource.ActionName);
                                        break;
                                    case SimpleInputResource simpleInputResource:
                                        schemeBuilder.WithMap(inputMap.Identity, simpleInputResource.InputId, simpleInputResource.ActionName);
                                        break;
                                    case CombinationInputResource combinationInputResource:
                                        var inputIdentifiers = combinationInputResource.InputIds.Select(id => new DeviceInputIdentifier(inputMap.Identity, id));
                                        schemeBuilder.WithVirtualInput(new PowerCombinationInput(inputIdentifiers), combinationInputResource.ActionName);
                                        break;
                                }
                            }
                        });
                    }
                });
            }
        });

    private void OnGamepadConnectionChange(long device, bool connected)
    {
        if (InputSystem is null)
        {
            return;
        }

        var deviceIdentifier = new InputEventJoypadButton() { Device = (int)device }.GetRuntimeDeviceIdentifier();
        var deviceStateChangeNotification = new DeviceStateChangedNotification(deviceIdentifier, status: connected ? DeviceStatus.Active : DeviceStatus.Disconnected);

        InputSystemNotifier.Notify(deviceStateChangeNotification);
    }

    #endregion
}
