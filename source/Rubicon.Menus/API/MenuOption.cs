using Godot;

namespace Rubicon.Menus.API;

/// <summary>
/// Contains data about an option in BaseMenu.
/// </summary>
[GlobalClass]
public partial class MenuOption : Resource
{
    /// <summary>
    /// The animation to play when this option is selected.
    /// </summary>
    [Export] public string SelectedAnimation;
    
    /// <summary>
    /// The animation to play when this option is deselected.
    /// </summary>
    [Export] public string DeselectedAnimation;
    
    /// <summary>
    /// The node path to the object from the BaseMenu.
    /// </summary>
    [ExportGroup("References"), Export] public NodePath PathToObject;
    
    /// <summary>
    /// The node path to the object's animation player from the BaseMenu.
    /// </summary>
    [Export] public NodePath PathToAnimationPlayer;
    
    /// <summary>
    /// Determines whether this option is selected.
    /// </summary>
    public bool IsSelected = false;
}