using Godot;

namespace Rubicon.Game.Data
{
    [GlobalClass]
    public partial class SongDifficulty : Resource
    {
        /// <summary>
        /// The internal name that is referred to in code.
        /// </summary>
        [Export] public string InternalName = "normal";
        
        /// <summary>
        /// The name displayed on-screen.
        /// </summary>
        [Export] public string DisplayName = "Normal";
        
        /// <summary>
        /// The path to the chart file.
        /// </summary>
        [Export(PropertyHint.File, "*.json")] public string ChartPath;
        
        /// <summary>
        /// The color associated with this difficulty.
        /// </summary>
        [Export] public Color Color = Colors.Black;
        
        /// <summary>
        /// The charter's name, just to give credit.
        /// </summary>
        [Export] public string Charter = "";
        
        /// <summary>
        /// Whether this difficulty is hidden from Free Play.
        /// </summary>
        [Export] public bool Hidden = false;
    }
}