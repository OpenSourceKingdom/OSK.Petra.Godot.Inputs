using OSK.Petra.Inputs.Capabilities.Pointer;

namespace OSK.Petra.Godot.Inputs;

/// <summary>
/// A collection of well known action groups that are inherently supported by the Input System. 
/// 
/// Action groups are meant to allow enabling/disabling a group of input actions. The values of a group are defined by the application that may need them,
/// but these serve as a 'known' list of values. These values are meant to serve a special purpose that are supported by the input system, so these values should
/// be avoided by consuming applications to avoid potential logic issues with the input system.
/// 
/// For example, the pointer group has specific built-in logic to handle user editing and GUI controls with the input.
/// 
/// As such, it is recommended to utilize values that are not listed below. A healthy starting value for custom action groups would be values greater or equal to 100, in order
/// to provide a deconfliction with potential future feature additions to these action groups.
/// </summary>
public static class ActionGroups
{
    /// <summary>
    /// Actions that utilize <see cref="IPointer"/> style inputs
    /// </summary>
    public const int Pointer = 1;
}
