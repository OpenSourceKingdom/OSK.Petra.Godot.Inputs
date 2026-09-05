using Godot;
using OSK.Extensions.Petra.Godot;
using OSK.Petra.Godot.Inputs.Devices;
using OSK.Petra.Godot.Inputs.Devices.Gamepads;
using OSK.Petra.Godot.Inputs.Devices.Keyboards;
using OSK.Petra.Godot.Inputs.Devices.Mice;
using OSK.Petra.Inputs.Abstractions.Devices;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Capabilities.Pointer;
using OSK.Petra.Inputs.Capabilities.Power;
using OSK.Petra.Inputs.Notifications;
using OSK.Petra.Inputs.Ports;
using System;
using System.Linq;

namespace OSK.Petra.Godot.Inputs;

public static class InputEventExtensions
{
    #region RuntimeDeviceIdentifier

    private static readonly string[] PlaystationDeviceTags = ["playstation", "dualshock", "ps4", "ps5"];
    private static readonly string[] XboxDeviceTags = ["xbox"];
    private static readonly string[] NintendoDeviceTags = ["switch", "joy-con", "joycon", "pro controller"];
    private static readonly string[] SteamDeviceTags = ["steam", "steam deck", "steam controller", "valve"];

    /// <summary>
    /// Gets a runtime identifier for the device and user that is initiating the input event
    /// </summary>
    /// <param name="inputEvent">The event to get the user device identifier for</param>
    /// <returns>The device identifier for the user</returns>
    public static RuntimeDeviceIdentifier GetRuntimeDeviceIdentifier(this InputEvent inputEvent)
    {
        if (inputEvent is InputEventKey)
        {
            return new RuntimeDeviceIdentifier(DeviceIdentifiers.KeyboardDeviceId, new DeviceIdentity(DeviceTopologyName.Keyboard));
        }
        if (inputEvent is InputEventMouse)
        {
            return new RuntimeDeviceIdentifier(DeviceIdentifiers.MouseDeviceId, new DeviceIdentity(DeviceTopologyName.Mouse));
        }

        var name = Input.GetJoyName(inputEvent.Device);
        var deviceFamily = name switch
        {
            var n when XboxDeviceTags.Any(tag => n.Contains(tag, StringComparison.OrdinalIgnoreCase)) => DeviceFamily.Xbox,
            var n when PlaystationDeviceTags.Any(tag => n.Contains(tag, StringComparison.OrdinalIgnoreCase)) => DeviceFamily.PlayStation,
            var n when NintendoDeviceTags.Any(tag => n.Contains(tag, StringComparison.OrdinalIgnoreCase)) => DeviceFamily.Nintendo,
            var n when SteamDeviceTags.Any(tag => n.Contains(tag, StringComparison.OrdinalIgnoreCase)) => DeviceFamily.Steam,
            _ => DeviceFamily.Generic
        };

        return new RuntimeDeviceIdentifier(inputEvent.Device, DeviceIdentities.Gamepad(deviceFamily, name));
    }

    #endregion

    #region DeviceInputNotification

    /// <summary>
    /// Converts an input event into a notification that an <see cref="IInputSystem"/> can respond to, when notified by the <see cref="IInputSystemNotifier"/>
    /// </summary>
    /// <param name="inputEvent">The event to convert</param>
    /// <param name="deltaTime">The time since last frame</param>
    /// <returns>The input notification</returns>
    public static DeviceInputNotification ToInputNotification(this InputEvent inputEvent, TimeSpan deltaTime)
    {
        var deviceIdentifier = inputEvent.GetRuntimeDeviceIdentifier();
        return inputEvent switch
        {
            InputEventKey keyEvent => new(deviceIdentifier, KeyboardKeyInput.GetId(keyEvent.Keycode), deltaTime, keyEvent.Pressed ? PowerEvent.Full() : PowerEvent.Zero()),
            InputEventMouseButton mouseButton => new(deviceIdentifier, MouseButtonInput.GetId(mouseButton.ButtonIndex), deltaTime, mouseButton.Pressed ? PowerEvent.Full() : PowerEvent.Zero()),
            InputEventMouseMotion mouseMotion => new(deviceIdentifier, MouseMovement.MouseMovementId, deltaTime, new PointerEvent(mouseMotion.GlobalPosition.ToNumerics2())),
            InputEventJoypadButton joypadButton => new(deviceIdentifier, GamepadJoyButton.GetId(joypadButton.ButtonIndex), deltaTime, joypadButton.Pressed ? PowerEvent.Full() : PowerEvent.Zero()),
            InputEventJoypadMotion joypadMotion => new(deviceIdentifier, GamepadJoyAxis.GetId(joypadMotion.Axis), deltaTime, PowerEvent.Activate(joypadMotion.Axis.ToPowerAxis(), joypadMotion.AxisValue)),
            _ => null
        };
    }

    #endregion
}
