using Godot;
using Godot.Collections;
using OSK.Petra.Godot.Inputs.Devices.Gamepads;
using OSK.Petra.Godot.Inputs.Devices.Keyboards;
using OSK.Petra.Godot.Inputs.Devices.Mice;
using OSK.Petra.Inputs.Abstractions.Devices;
using System;
using System.Linq;
using OSK.Petra.Godot.Inputs.Data.Inputs;
using OSK.Extensions.Petra.Inputs.Devices;
using GodotMouseButton = Godot.MouseButton;

namespace OSK.Petra.Godot.Inputs.Data;

/// <summary>
/// A configurable device map that can be configured within the Godot inspector
/// </summary>
[Tool]
[GlobalClass]
public partial class DeviceMapResource: Resource
{
	#region Variables

	private InputSchemeResource _inputScheme;
	private InputMapResource[] _inputMaps { get; set; }
	private DeviceType _selectedDeviceType;
	private string _selectedDeviceName;

	private DeviceFamily _deviceFamily = DeviceFamily.Generic;

	/// <summary>
	/// The device type
	/// </summary>
	[Export]
	public DeviceType DeviceType
	{
		get => _selectedDeviceType;
		set
		{
			_selectedDeviceType = value;
			GD.PushWarning("An update to the input device type has caused a change in the map options. Current maps have been cleared.");
			InputMaps = [];

			NotifyPropertyListChanged();

			DeviceName = DeviceIdentities.GenericDeviceName;
		}
	}

	/// <summary>
	/// The name of the device
	/// </summary>
	[Export]
	public string DeviceName
	{
		get => string.IsNullOrWhiteSpace(_selectedDeviceName) ? DeviceIdentities.GenericDeviceName : _selectedDeviceName;
		set
		{
			_selectedDeviceName = value;
            GD.PushWarning("An update to the input device name has caused a change in the map options. Current maps have been cleared.");
            InputMaps = [];

            NotifyPropertyListChanged();
        }
	}

	/// <summary>
	/// The input maps that are assigned to this device map
	/// </summary>
	[Export]
	public InputMapResource[] InputMaps
	{
		get => _inputMaps;
		set
		{
			InputMapResource[] newMaps = value is null
				? null
				: _inputMaps is null
					? null
					: [.. value.Where(v => !_inputMaps.Contains(v))];
			if (newMaps is not null)
			{
				CallDeferred(nameof(UpdateInputMaps), newMaps);
			}

			_inputMaps = value;
		} 
	}

	/// <summary>
	/// The parent scheme resource
	/// </summary>
	public InputSchemeResource InputScheme 
	{ 
		get => _inputScheme;
		set
		{
			_inputScheme = value;
			NotifyPropertyListChanged();
		}
	}

	/// <summary>
	/// Gets the identity for the device map
	/// </summary>
	/// <returns>The device identity</returns>
	/// <exception cref="NotSupportedException">if the device type is not a valid type</exception>
	public DeviceIdentity GetDeviceIdentity()
		=> DeviceType switch
		{
			DeviceType.Keyboard => DeviceIdentities.Keyboard(_deviceFamily, DeviceName),
			DeviceType.Mouse => DeviceIdentities.Mouse(_deviceFamily, DeviceName),
			DeviceType.Gamepad => DeviceIdentities.Gamepad(_deviceFamily, DeviceName),
			_ => throw new NotSupportedException($"An unkown device type, '{DeviceType}', was used and no device identity exists to utilize it.")
		};

    #endregion

    #region Helpers

	/// <inheritdoc/>
    public override void _ValidateProperty(Dictionary property)
    {
        var propertyName = property["name"].AsStringName();

        if (propertyName == nameof(DeviceName))
        {
            property["hint"] = (int)PropertyHint.Enum;

			string[] supportedDevices = [DeviceIdentities.GenericDeviceName];

            property["hint_string"] = string.Join(",", supportedDevices);
        }
    }

	/// <summary>
	/// Gets the list of available actions for this device map
	/// </summary>
	/// <returns>The list of unused action names</returns>
    public string[] GetAvailableInputActionNames()
		=> InputScheme?.GetAvailableInputActionNames() ?? [];

	/// <summary>
	/// 
	/// </summary>
	/// <param name="mapResource"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public string GetAvailableInputHintString(InputMapResource mapResource)
	{
		var isDigitalOnly = mapResource is CombinationInputResource;

		var validEnumData = DeviceType switch
		{
            DeviceType.Keyboard => DeviceHelper.GetValues<Key>()
											   .Select(keyboardKey => new { Name = $"{keyboardKey}", Id = KeyboardKeyInput.GetId(keyboardKey) }),
            DeviceType.Mouse => DeviceHelper.GetValues<GodotMouseButton>()
											.Select(mouseButton => new { Name = $"{mouseButton}", Id = MouseButtonInput.GetId(mouseButton) })
										     // For mice, we'll currently limit combination inputs to buttons only
											.Concat(isDigitalOnly ? [] : [new { Name = "Mouse Movement", Id = MouseMovement.MouseMovementId }]),
            DeviceType.Gamepad => DeviceHelper.GetValues<JoyButton>()
										      .Select(gamepadButton => new { Name = $"{gamepadButton}", Id = GamepadJoyButton.GetId(gamepadButton) })
											  // Joy Button max is 128, so add 200 to give enough clean gap to differentiate
											  // For gamepads, we'll currently limit combination inputs to buttons only
											  .Concat(isDigitalOnly ? [] : DeviceHelper.GetValues<JoyAxis>().Select(axis => new { Name = $"{axis}", Id = GamepadJoyAxis.GetId(axis) })),
			_ => throw new InvalidOperationException($"DeviceType '{DeviceType}' does not have an input hint string mapping")
		}; 

		return string.Join(",", validEnumData.Select(data => $"{data.Name}:{data.Id}"));
	}

	private void UpdateInputMaps(InputMapResource[] inputMaps)
	{
		if (inputMaps is null)
		{
			return;
		}
		
		foreach (var map in inputMaps.Where(map => map is not null))
		{
			map.DeviceMapOwner = this;
		}

		NotifyPropertyListChanged();
	}

	#endregion
}
