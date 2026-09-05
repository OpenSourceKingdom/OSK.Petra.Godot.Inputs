using Godot;
using OSK.Petra.Godot.Inputs.Data.Settings;
using OSK.Petra.Inputs.Abstractions.Configuration;

namespace OSK.Petra.Godot.Inputs.Data;

/// <summary>
/// A manager configuration that is used to generate an input system <see cref="InputSystemConfiguration"/>
/// </summary>
[Tool]
[GlobalClass]
public partial class InputManagerConfiguration: Resource
{
    /// <summary>
    /// The maximum number of local players the input system will expect to play
    /// </summary>
    [Export]
    public int MaxLocalPlayers { get; set; } = 1;

    /// <summary>
    /// The behavior the device pairing will take
    /// </summary>
    [Export]
    public DevicePairingBehavior DevicePairingBehavior { get; set; }

    /// <summary>
    /// The behavior of new users will have when joining the game session
    /// </summary>
    [Export]
    public UserJoinBehavior UserJoinBehavior { get; set; }

    /// <summary>
    /// The list of action definitions for the input system
    /// </summary>
    [Export]
    public ActionDefinitionResource[] ActionDefinitions { get; set; }

    /// <summary>
    /// Default settings for power inputs
    /// </summary>
    [Export]
    public InputPowerSettingsResource DefaultPowerSettings { get; set; }

    /// <summary>
    /// Default settings for pointers
    /// </summary>
    [Export]
    public PointerSettingsResource DefaultPointerSettings { get; set; }

    /// <summary>
    /// Determines if Gui elements should block a pointer activation
    /// </summary>
    [Export]
    public bool BlockPointerWithGui { get; set; }
}
