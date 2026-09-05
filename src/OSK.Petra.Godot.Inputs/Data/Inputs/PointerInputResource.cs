using Godot;
using Godot.Collections;

namespace OSK.Petra.Godot.Inputs.Data.Inputs;

/// <summary>
/// Represents an input that provides continouous data
/// </summary>
[Tool]
[GlobalClass]
public sealed partial class PointerInputResource : InputActionMapResource
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
