using Godot;
using Godot.Collections;

namespace OSK.Petra.Godot.Inputs.Data.Inputs;

/// <summary>
/// Represents a combination of inputs on a particular device
/// </summary>
[Tool]
[GlobalClass]
public sealed partial class CombinationInputResource : InputActionMapResource
{
    #region Variables

    /// <summary>
    /// Unique ids of the inputs that are tied to the combination
    /// </summary>
    [Export]
    public int[] InputIds { get; set; }

    #endregion

    #region InputMapResource Overrides

    /// <inheritdoc/>
    protected override void ValidateInputProperty(Dictionary property, string propertyName, string inputHintString)
    {
        if (propertyName == nameof(InputIds))
        {
            // Hint 24 is PropertyHint.ArrayType
            // The hint_string format for an array of enums is: "2/2:HintString"
            // Where the first '2' is Variant.Type.Int, and the second '2' is PropertyHint.Enum
            property["hint"] = (int)PropertyHint.ArrayType;
            property["hint_string"] = $"{(int)Variant.Type.Int}/{(int)PropertyHint.Enum}:{inputHintString}";
        }
    }

    #endregion
}