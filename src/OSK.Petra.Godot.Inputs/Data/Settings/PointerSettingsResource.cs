using Godot;

namespace OSK.Petra.Godot.Inputs.Data.Settings;

/// <summary>
/// Allows configuring pointer default settings
/// </summary>
[GlobalClass]
[Tool]
public partial class PointerSettingsResource: Resource
{
    /// <summary>
    /// The distance a pointer style input must move to be considered a valid activation
    /// </summary>
    [Export]
    public float PointerDeadzoneTolerance { get; set; }

    /// <summary>
    /// Sets the maximum number of entries any particular pointer will track beyond the current position.
    /// </summary>
    [Export]
    public int MaxRecordsPerPointer { get; set; } = 2;
}
