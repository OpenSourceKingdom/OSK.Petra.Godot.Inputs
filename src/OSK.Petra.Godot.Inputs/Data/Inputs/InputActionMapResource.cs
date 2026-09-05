using Godot;
using Godot.Collections;

namespace OSK.Petra.Godot.Inputs.Data.Inputs;

/// <summary>
/// A base class that represents an input map that is tied to an action.
/// </summary>
public abstract partial class InputActionMapResource: InputMapResource
{
    #region Variables

    /// <summary>
    /// The action key this input maps to. This should be a method name located in the <see cref="CSharpScript"/> that is assigned to the definition
    /// </summary>
    [Export]
    public string ActionName { get; set; }

    #endregion

    #region InputMapResource Overrides

    /// <inheritdoc/>
    protected override void ValidateProperty(Dictionary property, string propertyName, string inputHintString)
    {
        var actionNames = DeviceMapOwner?.GetAvailableInputActionNames() ?? [];

        if (actionNames is null || actionNames.Length is 0)
        {
            GD.PushWarning("An attempt was made to set an input map without actions");
            actionNames = [];
        }

        if (propertyName == nameof(ActionName))
        {
            property["hint"] = (int)PropertyHint.Enum;
            property["hint_string"] = string.Join(",", actionNames ?? []);
        }

        ValidateInputProperty(property, propertyName, inputHintString);
    }

    /// <inheritdoc/>
    protected abstract void ValidateInputProperty(Dictionary property, string propertyName, string inputHintString);

    #endregion
}
