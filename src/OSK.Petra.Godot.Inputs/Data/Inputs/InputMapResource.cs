using Godot;
using Godot.Collections;

namespace OSK.Petra.Godot.Inputs.Data.Inputs;

/// <summary>
/// A base class that provides some logic for Input Maps
/// </summary>
[Tool]
[GlobalClass]
public abstract partial class InputMapResource : Resource
{
	#region Variables

	private DeviceMapResource _deviceMapOwner;

	/// <summary>
	/// The parent resource that represents a device that possesses this input
	/// </summary>
	public DeviceMapResource DeviceMapOwner 
	{ 
		get => _deviceMapOwner;
		set 
		{
			_deviceMapOwner = value;
			NotifyPropertyListChanged();
		} 
	}

	#endregion

	#region Resource Overrides

	/// <inheritdoc/>
	public override void _ValidateProperty(Dictionary property)
	{
		var propertyName = property["name"].AsStringName();

		var inputHintString = DeviceMapOwner?.GetAvailableInputHintString(this);
		if (string.IsNullOrWhiteSpace(inputHintString))
		{
			GD.PushWarning("An attempt was made to set an input map without an input hint");
			inputHintString = string.Empty;
		}

		ValidateProperty(property, propertyName, inputHintString);
	}

    #endregion

    #region Helpers

    /// <inheritdoc/>
    protected abstract void ValidateProperty(Dictionary property, string propertyName, string inputHintString);

	#endregion
}
