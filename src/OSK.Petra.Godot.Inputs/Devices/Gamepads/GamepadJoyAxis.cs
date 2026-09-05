using Godot;
using OSK.Extensions.Petra.Inputs.Devices.Gamepads;
using OSK.Petra.Inputs.Abstractions.Devices;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Godot.Inputs.Devices.Gamepads;

/// <summary>
/// Represents a joy axis or other analog input for the game pad
/// </summary>
/// <param name="owner">The device owner of this input</param>
/// <param name="powerAxis">The axis the input aligns with</param>
/// <param name="sensitivityThreshold">How much power should be applied to be consider </param>
public class GamepadJoyAxis(DeviceIdentity owner, JoyAxis powerAxis, float sensitivityThreshold = 0.1f)
    : GamepadAnalog(GetId(powerAxis), powerAxis.ToPowerAxis(), sensitivityThreshold)
{
    #region Static

    /// <summary>
    /// Gets a unique id for the axis input
    /// </summary>
    /// <param name="axis">The axis to get an id for</param>
    /// <returns>The unique id for the axis</returns>
    public static long GetId(JoyAxis axis)
        // JoyButtons and JoyAxis inputs are used with gamepads and must have unique ids, since they both start at 0, we need to deconflict
        // JoyButtons has a max set to 128, so using 200 as a healthy separation
        => (long)axis + 200;

    #endregion

    #region AnalogInput Overrides

    /// <inheritdoc/>
    public override Task<InputGlyph> GetGlyphAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new InputGlyph()
        {
            DeviceIdentity = owner,
            Input = this,
            Text = new InputEventJoypadMotion() { Axis = powerAxis }.AsText()
        });

    #endregion
}
