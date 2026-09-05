# OSK.Petra.Godot.Inputs

An implementation/integration of the [OSK.Petra.Inputs](https://github.com/OpenSourceKingdom/OSK.Petra.Inputs). This library provides a mechanism to both configure and run an input
system using an `IInputManager`. An `InputEventManager` is the only script necessary to run and process Godot InputEvents with the Petra input system. 

To fully utilize this library in a godot scene:
- Use a DI container and add required services using `AddGodotInputSystem` (see [Petra's Service Module](https://github.com/OpenSourceKingdom/OSK.Petra.Godot.Modules) to utilize a .NET native DI container for Godot)
- Add an input manager to the scene (see `InputEventManager` for one such implementation)
- Configure the input system for your scene's needs (local players, join behavior, action definitions, etc.)
- Enjoy a .NET DI based input system within Godot!

For more information on the project architecture, please see related information on the design in the [Hub](https://opensourcekingdom.github.io/OSK.Hub/)

# Contributions and Issues
Any and all contributions are appreciated! Please be sure to follow the branch naming convention OSK-{issue number}-{deliminated}-{branch}-{name} as current workflows rely on it for automatic issue closure. Please submit issues for discussion and tracking using the github issue tracker.