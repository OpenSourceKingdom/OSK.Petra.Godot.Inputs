using Godot;
using OSK.Petra.Inputs.Abstractions.Devices;
using System.Threading;
using System.Threading.Tasks;
using GodotMouseButton = Godot.MouseButton;
using PetraMouseButton = OSK.Extensions.Petra.Inputs.Devices.Mice.MouseButton;

namespace OSK.Petra.Godot.Inputs.Devices.Mice;

/// <summary>
/// An input that represents a mouse button
/// </summary>
/// <param name="owner">The device owner of this input</param>
/// <param name="button">The button for this input</param>
public class MouseButtonInput(DeviceIdentity owner, GodotMouseButton button)
    : PetraMouseButton(GetId(button))
{
    #region Static

    /// <summary>
    /// Gets a unique id for the button
    /// </summary>
    /// <param name="button">The button to get the id for</param>
    /// <returns>A unique id for the button</returns>
    public static long GetId(GodotMouseButton button)
        => (long) button;

    #endregion

    #region DigitalInput Overrides

    /// <inheritdoc/>
    public override Task<InputGlyph> GetGlyphAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new InputGlyph()
        {
            DeviceIdentity = owner,
            Input = this,
            Text = new InputEventMouseButton() { ButtonIndex = button }.AsText()
        });

    #endregion
}
