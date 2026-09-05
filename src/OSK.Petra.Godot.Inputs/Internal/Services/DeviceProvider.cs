using Godot;
using OSK.Extensions.Petra.Inputs.Devices;
using OSK.Extensions.Petra.Inputs.Devices.Gamepads;
using OSK.Extensions.Petra.Inputs.Devices.Keyboards;
using OSK.Extensions.Petra.Inputs.Devices.Mice;
using OSK.Operations.Outputs;
using OSK.Operations.Outputs.Models;
using OSK.Petra.Godot.Inputs.Data.Settings;
using OSK.Petra.Godot.Inputs.Devices.Gamepads;
using OSK.Petra.Godot.Inputs.Devices.Keyboards;
using OSK.Petra.Godot.Inputs.Devices.Mice;
using OSK.Petra.Godot.Inputs.Ports;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Capabilities.Power;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using GodotMouseButton = Godot.MouseButton;

namespace OSK.Petra.Godot.Inputs.Internal.Services;

internal class DeviceProvider(IInputManager inputManager) : IDeviceProvider
{
    #region IDeviceProvider

    public Task<Output<IEnumerable<IDeviceDescriptor>>> GetDevicesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Out.Success((IEnumerable<IDeviceDescriptor>)
        [
            GetGenericKeyboard(),
            GetGenericMouse(inputManager.Configuration.DefaultPointerSettings),
            GetGenericGamepad(inputManager.Configuration.DefaultPowerSettings)
        ]));

    #endregion

    #region Helpers

    private KeyboardDescriptor GetGenericKeyboard()
    {
        var inputs = DeviceHelper.GetValues<Key>().Select(key => new KeyboardKeyInput(DeviceIdentities.GenericKeyboard, key));
        return new(inputs);
    }

    private MouseDescriptor GetGenericMouse(PointerSettingsResource pointerSettingsResource)
    {
        var distanceThreshold = pointerSettingsResource?.PointerDeadzoneTolerance ?? new PointerSettings().DistanceThreshold;

        var inputs = DeviceHelper.GetValues<GodotMouseButton>()
                                 .Select(button => new MouseButtonInput(DeviceIdentities.GenericMouse, button))
                                 .Append((IMouseInput)new MouseMovement(DeviceIdentities.GenericMouse, distanceThreshold));
        return new(inputs);
    }

    private GamepadDescriptor GetGenericGamepad(InputPowerSettingsResource powerSettingsResource)
    {
        var sensitivityThreshold = powerSettingsResource?.PowerSensitivityThreshold ?? new PowerSettings().PowerSensitivityThreshold;
        var inputs = DeviceHelper.GetValues<JoyButton>().Select(button => (IGamepadInput)new GamepadJoyButton(DeviceIdentities.GenericGamepad, button))
            .Concat(DeviceHelper.GetValues<JoyAxis>().Select(axis => new GamepadJoyAxis(DeviceIdentities.GenericGamepad, axis, sensitivityThreshold)));

        return new(inputs);
    }

    #endregion
}
