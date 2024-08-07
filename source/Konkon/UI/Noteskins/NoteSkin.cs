using System.Linq;
using Godot;
using Godot.Collections;

namespace Konkon.Game.UI.Noteskins
{
    /// <summary>
    /// A resource that contains the graphics for lanes, notes, and splashes. Can also use materials if it is dependent on the RGB shader.
    /// </summary>
    [GlobalClass]
    public partial class NoteSkin : Resource
    {
        #region Modifiable Settings
        /// <summary>
        /// Contains resources with both the note and tail, is lane count-dependent.
        /// </summary>
        [Export] private Dictionary<int, Array<NotePair>> _notes = new();
        
        /// <summary>
        /// Contains the scenes for multiple lanes, is lane count-dependent.
        /// </summary>
        [Export] private Dictionary<int, Array<PackedScene>> _lanes = new();
        
        /// <summary>
        /// Contains the splashes for multiple lanes, is lane count-dependent.
        /// </summary>
        [Export] private Dictionary<int, Array<PackedScene>> _splashes = new();
        
        /// <summary>
        /// Contains the materials for multiple notes and lanes for the RGB shader, is lane count-dependent.
        /// </summary>
        [Export] private Dictionary<int, Array<Material>> _materials = new();
        
        /// <summary>
        /// Whether to use the RGB shader included with the engine.
        /// </summary>
        [Export] public bool UseRgbShader { get; private set; } = true;

        /// <summary>
        /// The size for each lane graphic. Is usually 160 for most noteskins.
        /// </summary>
        [Export] public float LaneSize { get; private set; } = 160f;
        #endregion

        #region Public Methods
        
        #region Get Multiple
        /// <summary>
        /// Fetches an array of resources containing the note and tail graphic according to the lane count.
        /// </summary>
        /// <param name="laneCount">The amount of lanes</param>
        /// <returns>An array of resources containing the note and tail graphic</returns>
        public NotePair[] GetNotes(int laneCount) => laneCount < _notes.Count && _notes[laneCount] != null ? _notes[laneCount].ToArray() : null;
        
        /// <summary>
        /// Fetches an array of packed scenes for lanes according to the lane count.
        /// </summary>
        /// <param name="laneCount">The amount of lanes</param>
        /// <returns>An array of packed scenes for lanes</returns>
        public PackedScene[] GetLanes(int laneCount) => laneCount < _lanes.Count && _lanes[laneCount] != null ? _lanes[laneCount].ToArray() : null;

        /// <summary>
        /// Fetches an array of packed scenes for splash graphics according to the lane count.
        /// </summary>
        /// <param name="laneCount">The amount of lanes</param>
        /// <returns>An array of packed scenes for splash graphics according to the lane count.</returns>
        public PackedScene[] GetSplashes(int laneCount) => laneCount < _splashes.Count && _splashes[laneCount] != null ? _splashes[laneCount].ToArray() : null;

        /// <summary>
        /// Fetches an array of materials for the lanes and notes according to the lane count.
        /// </summary>
        /// <param name="laneCount">The amount of lanes</param>
        /// <returns>An array of materials for the lanes and notes according to the lane count.</returns>
        public Material[] GetMaterials(int laneCount) => laneCount < _materials.Count && _materials[laneCount] != null ? _materials[laneCount].ToArray() : null;
        #endregion
        
        #region Get Single
        /// <summary>
        /// Fetches a resource containing both the note and tail according to the provided lane count and lane index.
        /// </summary>
        /// <param name="laneCount">The amount of lanes</param>
        /// <param name="lane">The lane index</param>
        /// <returns>A resource containing both the note and tail</returns>
        public NotePair GetNoteFromLaneGroup(int laneCount, int lane) => laneCount < _notes.Count && _notes[laneCount] != null ? _notes[laneCount][lane] : null;
        
        /// <summary>
        /// Fetches a packed scene for a lane according to the provided lane count and lane index.
        /// </summary>
        /// <param name="laneCount">The amount of lanes</param>
        /// <param name="lane">The lane index</param>
        /// <returns>A packed scene for a lane</returns>
        public PackedScene GetLaneFromLaneGroup(int laneCount, int lane) => laneCount < _lanes.Count && _lanes[laneCount] != null ? _lanes[laneCount][lane] : null;
        
        /// <summary>
        /// Fetches a packed scene for a splash graphic according to the provided lane count and lane index.
        /// </summary>
        /// <param name="laneCount">The amount of lanes</param>
        /// <param name="lane">The lane index</param>
        /// <returns>A packed scene for a splash graphic</returns>
        public PackedScene GetSplashFromLaneGroup(int laneCount, int lane) => laneCount < _splashes.Count && _splashes[laneCount] != null ? _splashes[laneCount][lane] : null;
        
        /// <summary>
        /// Fetches the material for a note and lane according to the provided lane count and lane index.
        /// </summary>
        /// <param name="laneCount">The amount of lanes</param>
        /// <param name="lane">The lane index</param>
        /// <returns>The material for a note and lane</returns>
        public Material GetMaterialFromLaneGroup(int laneCount, int lane) => laneCount < _materials.Count && _materials[laneCount] != null ? _materials[laneCount][lane] : null;
        #endregion
        
        #endregion
    }
}