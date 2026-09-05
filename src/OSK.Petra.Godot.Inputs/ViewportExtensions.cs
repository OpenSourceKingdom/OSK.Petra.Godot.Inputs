using Godot;

namespace OSK.Petra.Godot.Inputs;

public static class ViewportExtensions
{
    /// <summary>
    /// Validates whether the user is interacting with a input editor
    /// </summary>
    /// <param name="viewport">The viewport to validate user interaction</param>
    /// <returns>Whether the user is utilizing an input editor</returns>
    public static bool IsUserTypingInView(this Viewport viewport)
        => viewport.GuiGetFocusOwner() switch
        {
            LineEdit => true,
            TextEdit => true,
            _ => false
        };
}
