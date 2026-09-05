using Godot;
using OSK.Extensions.Petra.Inputs.Devices.Gamepads;
using OSK.Petra.Inputs.Abstractions.Devices;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Godot.Inputs.Devices.Gamepads;

/// <summary>
/// Represents a button input on a gamepad
/// </summary>
/// <param name="owner">The device owner of this input</param>
/// <param name="button"></param>
public class GamepadJoyButton(DeviceIdentity owner, JoyButton button)
    : GamepadButton(GetId(button))
{
    #region Static

    /// <summary>
    /// Gets a unique id for the button
    /// </summary>
    /// <param name="button">The button to get an id for</param>
    /// <returns>The unique id</returns>
    public static long GetId(JoyButton button)
        => (long)button;

    #endregion

    #region DigitalInput Overrides

    /// <inheritdoc/>
    public override Task<InputGlyph> GetGlyphAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new InputGlyph()
        {
            DeviceIdentity = owner,
            Input = this,
            Text = new InputEventJoypadButton() { ButtonIndex = button }.AsText()
        });

    #endregion
}
