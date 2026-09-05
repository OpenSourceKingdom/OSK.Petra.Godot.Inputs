using Godot;
using OSK.Extensions.Petra.Inputs.Devices.Keyboards;
using OSK.Petra.Inputs.Abstractions.Devices;
using System.Threading;
using System.Threading.Tasks;

namespace OSK.Petra.Godot.Inputs.Devices.Keyboards;

/// <summary>
/// An input that represents a keyboard key
/// </summary>
/// <param name="owner">The device owner of this input</param>
/// <param name="key">The keyboard key for this input</param>
public class KeyboardKeyInput(DeviceIdentity owner, Key key)
    : KeyboardKey(GetId(key))
{
    #region Static

    /// <summary>
    /// Gets a unique id for the key
    /// </summary>
    /// <param name="key">The key to get an id for</param>
    /// <returns></returns>
    public static long GetId(Key key)
        => (long)key;

    #endregion

    #region KeyboardKey Overrides

    /// <inheritdoc/>
    public override Task<InputGlyph> GetGlyphAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new InputGlyph()
        {
            DeviceIdentity = owner,
            Input = this,
            Text = OS.GetKeycodeString(key)
        });

    #endregion
}
