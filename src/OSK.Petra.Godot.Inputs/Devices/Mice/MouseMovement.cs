using OSK.Extensions.Petra.Inputs.Devices.Mice;
using OSK.Petra.Inputs.Abstractions.Devices;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Godot.Inputs.Devices.Mice;

/// <summary>
/// An input for mouse movement (pointer)
/// </summary>
/// <param name="owner">The device owner of this input</param>
/// <param name="distanceThreshold">The amount of distance that is used to determine intentional movement by a user</param>
public class MouseMovement(DeviceIdentity owner, float distanceThreshold = .1f): MouseMotion(MouseMovementId, distanceThreshold), IMouseInput
{
    #region Variables

    /// <summary>
    /// A unique identifier for mouse motion input
    /// </summary>
    public const long MouseMovementId = 100;

    #endregion

    #region PointerInput Overrides

    /// <inheritdoc/>
    public override Task<InputGlyph> GetGlyphAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new InputGlyph()
        {
            DeviceIdentity = owner,
            Input = this,
            Text = "Mouse Movement"
        });

    #endregion
}