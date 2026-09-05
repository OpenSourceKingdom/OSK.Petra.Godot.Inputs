using Godot;
using OSK.Extensions.Petra.Godot.Roslyn;
using OSK.Extensions.Petra.Inputs.Configuration;
using OSK.Extensions.Petra.Inputs.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OSK.Petra.Godot.Inputs.Data.Inputs;

namespace OSK.Petra.Godot.Inputs.Data;

/// <summary>
/// A Godot inspector editable scheme resource
/// </summary>
[Tool]
[GlobalClass]
public partial class InputSchemeResource: Resource
{
    #region Variables

    private ActionDefinitionResource _definition;
    private DeviceMapResource[] _deviceMaps = [];

    /// <summary>
    /// The action definition this scheme utilizes
    /// </summary>
    public ActionDefinitionResource Definition 
    { 
        get => _definition; 
        set
        {
            _definition = value;
            CallDeferred(nameof(UpdateDeviceMaps), _deviceMaps);
            NotifyPropertyListChanged();
        }
    }

    /// <summary>
    /// The name for the scheme
    /// </summary>
    [Export]
    public string Name { get; set; }
    
    /// <summary>
    /// Whether the scheme is a default scheme
    /// </summary>
    [Export]
    public bool IsDefault { get; set; }

    /// <summary>
    /// The device map list for the scheme
    /// </summary>
    [Export]
    public DeviceMapResource[] DevicesMaps
    {
        get => _deviceMaps;
        set
        {
            DeviceMapResource[] newMaps = value is null
                ? null
                : _deviceMaps is null
                    ? null
                    : [.. value.Where(v => !_deviceMaps.Contains(v))];
            if (newMaps is not null)
            {
                CallDeferred(nameof(UpdateDeviceMaps), newMaps);
            }

            _deviceMaps = value;
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Gets the currently available action names for the input scheme.
    /// i.e. this will return a list of only available action names that have not been assigned to another input
    /// </summary>
    /// <returns></returns>
    public string[] GetAvailableInputActionNames()
    {
        var existingActionNames = DevicesMaps?.SelectMany(map => map.InputMaps ?? [])
                                              .OfType<InputActionMapResource>()
                                              .Where(input => !string.IsNullOrWhiteSpace(input?.ActionName))
                                              .GroupBy(inputGroup => inputGroup.ActionName)
                                              .Select(group => group.Key)
                                              .ToHashSet()
                                  ?? new HashSet<string>();

        return Definition?.ActionScript?.GetScriptType()?.GetActionMethods()
                                                         .Select(m =>
                                                         {
                                                             var actionAttribute = m.GetCustomAttribute<InputActionAttribute>();
                                                             return string.IsNullOrWhiteSpace(actionAttribute?.ActionName)
                                                                ? m.Name : actionAttribute.ActionName;
                                                         })
                                                         .Where(actionName => !existingActionNames.Contains(actionName))
                                                         .ToArray()
                          ?? [];
    }

    private void UpdateDeviceMaps(DeviceMapResource[] deviceMaps)
    {
        GD.PrintRich($"[color=yellow]Input Scheme:[/color] updating device maps ...");
        if (deviceMaps is null)
        {
            GD.PrintRich($"[color=yellow]Input Scheme:[/color] device maps are null ...");
            return; 
        }

        foreach (var deviceMap in deviceMaps)
        {
            if (deviceMap is null)
            {
                continue;
            }

            GD.PrintRich($"[color=yellow]Input Scheme:[/color] setting definition to device map {deviceMap.GetDeviceIdentity()} ...");
            deviceMap.InputScheme = this;
            deviceMap.NotifyPropertyListChanged();
        }
    }

    #endregion
}
