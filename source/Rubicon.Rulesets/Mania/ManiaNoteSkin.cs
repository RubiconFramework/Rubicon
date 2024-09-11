using System.Collections.Generic;
using System.Linq;

namespace Rubicon.Rulesets.Mania;

/// <summary>
/// A resource that holds important information for Mania-related graphics.
/// </summary>
[GlobalClass]
public partial class ManiaNoteSkin : Resource
{
    /// <summary>
    /// The SpriteFrames resource to grab note textures from.
    /// </summary>
    [Export] public SpriteFrames NoteAtlas;

    /// <summary>
    /// The SpriteFrames resource to grab lane textures from.
    /// </summary>
    [Export] public SpriteFrames LaneAtlas;

    /// <summary>
    /// The SpriteFrames resource to grab hold and tail textures from.
    /// </summary>
    [Export] public SpriteFrames HoldAtlas;

    /// <summary>
    /// Whether to enable tiling on hold graphics. For reference, hold graphics are usually stretched.
    /// </summary>
    [Export] public bool UseTiledHold = false;

    /// <summary>
    /// The scale used when generating notes and lanes.
    /// </summary>
    [Export] public Vector2 Scale = Vector2.One * 0.7f;

    /// <summary>
    /// The width of each lane.
    /// </summary>
    [Export] public float LaneSize = 160f * 0.7f;

    /// <summary>
    /// The filtering used when generating notes and lanes.
    /// </summary>
    [Export] public CanvasItem.TextureFilterEnum Filter = CanvasItem.TextureFilterEnum.Linear;

    /// <summary>
    /// Specifies direction names for each lane count.
    /// </summary>
    [Export] public ManiaDirection[] Directions = [new ManiaDirection()];

    private Dictionary<int, Texture2D[]> _tiledHoldGraphics = new();
    
    /// <summary>
    /// Initializes tiled hold graphics. Automatically called by <see cref="GetTiledHold"/> if it hasn't been called already.
    /// </summary>
    public void InitializeTileHolds()
    {
        if (!UseTiledHold || _tiledHoldGraphics.Count != 0)
            return;
        
        foreach (ManiaDirection direction in Directions)
        {
            int laneCount = direction.LaneCount;

            Texture2D[] textures = new Texture2D[laneCount];
            for (int i = 0; i < laneCount; i++)
            {
                string curDirection = direction.Directions[i];
                Texture2D curTexture = HoldAtlas.GetFrameTexture($"{curDirection}NoteHold", 0);
                if (curTexture is not AtlasTexture atlasTexture)
                {
                    textures[i] = curTexture;
                    continue;
                }
                
                int xPos = (int)atlasTexture.Region.Position.X;
                int yPos = (int)atlasTexture.Region.Position.Y;
                int width = (int)atlasTexture.Region.Size.X;
                int height = (int)atlasTexture.Region.Size.Y;
                Image curHoldImage = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
                curHoldImage.BlitRect(atlasTexture.GetImage(), new Rect2I(xPos, yPos, width, height), Vector2I.Zero);
                textures[i] = ImageTexture.CreateFromImage(curHoldImage);
            }
            
            _tiledHoldGraphics.Add(laneCount, textures);
        }
    }
    
    /// <summary>
    /// Gets a tiled hold graphic based on lane count and lane number if tiled holds are active.
    /// </summary>
    /// <param name="lane">The lane index</param>
    /// <param name="laneCount">The amount of lanes.</param>
    /// <returns>A Texture2D compatible with tiling.</returns>
    public Texture2D GetTiledHold(int lane, int laneCount = 4)
    {
        if (!UseTiledHold)
            return null;

        if (_tiledHoldGraphics.Count == 0)
            InitializeTileHolds();

        return _tiledHoldGraphics[laneCount][lane];
    }

    /// <summary>
    /// Gets a direction name based on lane count and lane number.
    /// </summary>
    /// <param name="lane">The lane index</param>
    /// <param name="laneCount">The amount of lanes.</param>
    /// <returns>A direction name if found (Ex: "left"), otherwise an empty name.</returns>
    public string GetDirection(int lane, int laneCount = 4)
    {
        string[] directions = GetDirections(laneCount);
        if (lane < directions.Length && lane >= 0)
            return directions[lane];

        return "";
    }
    
    /// <summary>
    /// Gets an array of directions based on the lane count provided.
    /// </summary>
    /// <param name="laneCount">The amount of lanes.</param>
    /// <returns>An array of direction names. (Ex: ["left", "down", "up", "right"])</returns>
    public string[] GetDirections(int laneCount = 4)
    {
        ManiaDirection direction = Directions.FirstOrDefault(x => x.LaneCount == laneCount);
        if (direction == null)
            return [];
        
        return direction.Directions;
    }
}