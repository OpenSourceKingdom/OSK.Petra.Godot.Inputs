using Godot;
using OSK.Petra.Inputs.Capabilities.Power;

namespace OSK.Petra.Godot.Inputs.Devices.Gamepads;

public static class JoyAxisExtensions
{
    /// <summary>
    /// Converts a Godot JoyAxis into the input system equivalent power axis
    /// </summary>
    /// <param name="axis">The joy axis to convert</param>
    /// <returns>The equivalent power axis</returns>
    public static PowerAxis ToPowerAxis(this JoyAxis axis)
        => axis switch
        {
            JoyAxis.LeftX => PowerAxis.X,
            JoyAxis.RightX => PowerAxis.X,
            JoyAxis.LeftY => PowerAxis.Y,
            JoyAxis.RightY => PowerAxis.Y,
            JoyAxis.Max => PowerAxis.Neutral,
            JoyAxis.SdlMax => PowerAxis.Neutral,
            JoyAxis.TriggerLeft => PowerAxis.Neutral,
            JoyAxis.TriggerRight => PowerAxis.Neutral,
            _ => PowerAxis.Neutral
        };
}
