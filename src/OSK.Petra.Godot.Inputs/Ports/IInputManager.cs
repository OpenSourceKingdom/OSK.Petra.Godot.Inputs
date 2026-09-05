using OSK.Hexagonal.MetaData;
using OSK.Petra.Godot.Inputs.Data;

namespace OSK.Petra.Godot.Inputs.Ports;

/// <summary>
/// A manager that provides configuration and manages input interaction within Godot
/// </summary>
[HexagonalIntegration(HexagonalIntegrationType.LibraryProvided, HexagonalIntegrationType.ConsumerOptional, HexagonalIntegrationType.IntegrationOptional)]
public interface IInputManager
{
    /// <summary>
    /// The configuration that is used to initialize the input system with
    /// </summary>
    InputManagerConfiguration Configuration { get; }
}
