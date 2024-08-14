using Godot;

namespace Rubicon.Space2D.Objects;

/// <summary>
/// A focus point for the camera in 2D spaces, used for characters and stages.
/// </summary>
public partial class CameraFocusPoint2D : Node2D
{
    /// <summary>
    /// When enabled, depending on the note sung, the camera will offset to the direction of the note slightly.
    /// </summary>
    [ExportGroup("Settings"), Export] public bool EnableNoteOffset = false;

    /// <summary>
    /// When enabled, sets the zoom to the number specified in Zoom when the camera switches focus to this point.
    /// </summary>
    [Export] public bool UseCustomZoom = false;
    
    /// <summary>
    /// If UseCustomZoom is enabled, the camera's zoom will be set to this number upon focusing on this point.
    /// </summary>
    [Export] public float Zoom = 1.0f;
}