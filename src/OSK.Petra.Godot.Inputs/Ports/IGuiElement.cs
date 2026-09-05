using OSK.Hexagonal.MetaData;

namespace OSK.Petra.Godot.Inputs.Ports;

/// <summary>
/// An element that visually displays some GUI style element to a user
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.ConsumerOptional)]
public interface IGuiElement
{
    /// <summary>
    /// An optional override that determines whether a pointer action should be projected through the element
    /// </summary>
    bool? BlockPointer { get; }
}
