using Godot;
using Godot.Collections;

namespace OSK.Petra.Godot.Inputs.Data.Inputs;

/// <summary>
/// Represents a simple action map  (e.g. digital inputs)
/// </summary>
[Tool]
[GlobalClass]
public sealed partial class SimpleInputResource : InputActionMapResource
{
    #region Variables

    /// <summary>
    /// The unique id of the input
    /// </summary>
    [Export]
    public int InputId { get; set; }

    #endregion

    #region InputMapResource Overrides

    /// <inheritdoc/>
    protected override void ValidateInputProperty(Dictionary property, string propertyName, string inputHintString)
    {
        if (propertyName == nameof(InputId))
        {
            property["hint"] = (int)PropertyHint.Enum;
            property["hint_string"] = inputHintString;
        }
    }

    #endregion
}
