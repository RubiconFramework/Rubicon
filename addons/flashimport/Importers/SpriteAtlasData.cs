using System.Collections.Generic;
using System.Text.Json.Serialization;

// im gonna kms this shits a mess i hate texture atlases
namespace Rubicon.addons.flashimport.Importers;
public class SpriteAtlasData
{
    // Sprite Data
    public class ATLAS
    {
        [JsonPropertyName("SPRITES")] public List<AtlasSprites> AtlasSprites = new();
    }

    public class AtlasSprites
    {
        [JsonPropertyName("SPRITE")] public AtlasSprite Sprite;
    }
    
    public class AtlasSprite
    {
        [JsonPropertyName("name")] public string SpriteName;
        [JsonPropertyName("x")] public float X;
        [JsonPropertyName("y")] public float Y;
        [JsonPropertyName("w")] public float Width;
        [JsonPropertyName("h")] public float Height;
        [JsonPropertyName("rotated")] public float Rotated;
    }
}

public class SpriteAtlasAnimation
{
    // Animation Data
    public AtlasAnimation AN;
    [JsonPropertyName("MD")] public AtlasMetadata atlasMetadata;

    public class AtlasAnimation
    {
        [JsonPropertyName("SN")] public string SpriteName;
        public AnimationTimeline TL;
    }

    public class AtlasMetadata
    {
        [JsonPropertyName("FRT")] public float Framerate;
    }

    public class AnimationTimeline
    {
        public List<AnimationLayer> L = new();
    }

    public class AnimationLayer
    {
        public string LN;
        [JsonPropertyName("FR")] public List<AnimationFrame> Frames = new();
    }

    public class AnimationFrame
    {
        [JsonPropertyName("I")] public int FrameIndex;
        [JsonPropertyName("DU")] public int FrameDuration;
        [JsonPropertyName("E")] public List<FrameElements> Elements = new();
    }

    public class FrameElements
    {
        [JsonPropertyName("ASI")] public AnimationInstace Instance;
    }

    public class AnimationInstace
    {
        [JsonPropertyName("N")] public string SpriteFrame;
        [JsonPropertyName("M3D")] public List<float> Matrix3D = new();
        public Dictionary<string,float> DecomposedMatrix = new();
    }
}