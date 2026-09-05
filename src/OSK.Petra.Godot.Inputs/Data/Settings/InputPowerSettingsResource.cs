using Godot;
using OSK.Petra.Godot.Primitives.Data;
using OSK.Petra.Godot.Primitives.Data.Nullables;

namespace OSK.Petra.Godot.Inputs.Data.Settings;

/// <summary>
/// Allows configuring default power settings
/// </summary>
[GlobalClass]
[Tool]
public partial class InputPowerSettingsResource: Resource
{
    /// <summary>
    /// The amount of time an input should be allotted to be considered a tap continuation of a previous input activation
    /// </summary>
    [Export]
    public NullableTimeSpan TapReactivationTime { get; set; }

    /// <summary>
    /// The amount of time an input must remain activated to be considered 'active'
    /// </summary>
    [Export]
    public TimeSpanResource ActiveTimeThreshold { get; set; }

    /// <summary>
    /// The amount of power that must be supplied to be considered a valid activation
    /// </summary>
    [Export]
    public float PowerSensitivityThreshold { get; set; }
}
