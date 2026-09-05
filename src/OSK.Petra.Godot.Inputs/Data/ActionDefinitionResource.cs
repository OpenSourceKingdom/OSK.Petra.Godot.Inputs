using Godot;
using OSK.Petra.Inputs.Abstractions.Configuration;
using System;
using System.Linq;

namespace OSK.Petra.Godot.Inputs.Data;

/// <summary>
/// A resource that allows configuring a <see cref="ActionDefinition"/> in the Godot inspector
/// </summary>
[Tool]
[GlobalClass]
public partial class ActionDefinitionResource : Resource
{
    #region Variables

    private InputSchemeResource[] _inputSchemes;
    private CSharpScript _actionScript;

    /// <summary>
    /// A unique name for the definition
    /// </summary>
    [Export]
    public string Name { get; set; }

    /// <summary>
    /// Describes if the definition is considered the default - that is the definition to use if no other definition is specified with the input system
    /// </summary>
    [Export]
    public bool IsDefault { get; set; }

    /// <summary>
    /// In order for the configuration builder to recognize the methods in a script and utilize it, they must:
    /// <list type="number">
    /// <item>Be added to the dependency container the input system is using.</item>
    /// <item>Have methods that take only the InputEventContext as a parameter.</item>
    /// <item>Have a void return type.</item>
    /// </list>
    /// Additionally, the action method names must be unique across all scripts. You may use the InputActionAttribute to customize some of this data and enable additional features when the action is run from a triggered input.
    /// </summary>
    [Export]
    public CSharpScript ActionScript
    {
        get => _actionScript;
        set
        {
            _actionScript = value;
            UpdateInputSchemes(InputSchemes);
            NotifyPropertyListChanged();
        }
    }

    /// <summary>
    /// The collection of input schemes associated with the definition
    /// </summary>
    [Export]
    public InputSchemeResource[] InputSchemes
    {
        get => _inputSchemes;
        set
        {
            InputSchemeResource[] newSchemes = value is null
                ? null
                : _inputSchemes is null
                    ? null
                    : [.. value.Where(v => !_inputSchemes.Contains(v))];
            if (newSchemes is not null)
            {
                CallDeferred(nameof(UpdateInputSchemes), newSchemes);
            }

            _inputSchemes = value;
        }
    }

    #endregion

    #region Helpers

    private void UpdateInputSchemes(InputSchemeResource[] inputSchemes)
    {
        GD.PrintRich("[color=yellow]Input Definition:[/color] updating schemes...");

        if (inputSchemes is null)
        {
            GD.PrintRich($"[color=yellow]Input Definition:[/color] input schemes are null ...");
            return;
        }

        foreach (var scheme in inputSchemes)
        {
            if (scheme is null)
            {
                continue;
            }

            GD.PrintRich($"[color=yellow]Input Definition:[/color] setting definition to scheme {(scheme.Name ?? "{Name not set}")} ...");
            scheme.Definition = this;
            scheme.NotifyPropertyListChanged();
        }
    }

    #endregion
}
