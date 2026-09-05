using Godot;

namespace OSK.Petra.Godot.Inputs.Devices;

/// <summary>
/// Well known device identifiers for Godot input system devices
/// </summary>
public static class DeviceIdentifiers
{
    /// <summary>
    /// A unique identifier for keyboard style input within Godot
    /// -1 is used for <see cref="InputEvent.DeviceIdEmulation"/>
    /// </summary>
    public const int KeyboardDeviceId = -100;

    /// <summary>
    /// A unique identifier for mouse style input within Godot
    /// -1 is used for <see cref="InputEvent.DeviceIdEmulation"/>
    /// </summary>
    public const int MouseDeviceId = -101;
}
