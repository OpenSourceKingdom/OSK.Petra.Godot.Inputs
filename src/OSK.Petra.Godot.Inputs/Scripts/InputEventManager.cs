using Godot;
using OSK.Petra.Godot.Inputs.Ports;
using OSK.Petra.Inputs.Notifications;
using System;

namespace OSK.Petra.Godot.Inputs.Scripts;

/// <summary>
/// An input manager that utilizes Godot's input event system to notify the input system of user interactions
/// </summary>
[GlobalClass]
public partial class InputEventManager: InputManager
{
    #region Variables

    private Viewport _viewPort;

    #endregion

    #region Godot Overrides

    /// <inheritdoc/>
    public override void _EnterTree()
    {
        _viewPort = GetViewport();
        InputSystemNotifier.OnSystemNotification += HandleSystemNotification;
    }

    /// <inheritdoc/>
    public override void _ExitTree()
    {
        InputSystemNotifier.OnSystemNotification -= HandleSystemNotification;
    }

    /// <inheritdoc/>
    public override void _Input(InputEvent inputEvent)
    {
        if (_viewPort is not null && _viewPort.IsUserTypingInView())
        {
            return;
        }

        var mappedNotification = inputEvent.ToInputNotification(TimeSpan.FromSeconds(GetProcessDeltaTime()));
        if (mappedNotification is null)
        {
            return;
        }

        var user = InputSystem.UserManager.GetUserForDevice(mappedNotification.DeviceIdentifier.DeviceId);
        var shouldBlockPointer = ShouldBlockPointer(_viewPort);
        if (user is not null && InputSystem.AreUserActionsSurpressed(user.Id, ActionGroups.Pointer) != shouldBlockPointer)
        {
            InputSystemNotifier.Notify(new ModifyActionGroupSuppressionNotification()
            {
                UserIds = [user.Id],
                ActionGroups = [ActionGroups.Pointer],
                Suppress = shouldBlockPointer
            });
        }

        InputSystemNotifier.Notify(mappedNotification);
    }

    #endregion

    #region Helpers

    private bool ShouldBlockPointer(Viewport viewport)
        => viewport.GuiGetHoveredControl() switch
        {
            IGuiElement gui => gui.BlockPointer.GetValueOrDefault(Configuration.BlockPointerWithGui),
            not null => Configuration.BlockPointerWithGui,
            _ => false
        };

    private void HandleSystemNotification(SystemNotification notification)
    {
        if (notification is ActionExecutedNotification)
        {
            _viewPort?.SetInputAsHandled();
        }
    }

    #endregion
}
