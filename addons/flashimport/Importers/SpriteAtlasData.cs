using System.Collections.Generic;
using Newtonsoft.Json;

// im gonna kms this shits a mess i hate texture atlases
namespace FlashImporter.addons.flashimport.Importers;
public class SpriteAtlasData
{
    // Sprite Data
    [JsonProperty("ATLAS")] public Atlas atlas;
    public class Atlas
    {
        [JsonProperty("SPRITES")] public List<AtlasSprites> AtlasSprites = new();
    }

    public class AtlasSprites
    {
        [JsonProperty("SPRITE")] public AtlasSprite Sprite;
    }
    
    public class AtlasSprite
    {
        [JsonProperty("name")] public string SpriteName;
        [JsonProperty("x")] public float X;
        [JsonProperty("y")] public float Y;
        [JsonProperty("w")] public float Width;
        [JsonProperty("h")] public float Height;
        [JsonProperty("rotated")] public bool Rotated;
    }
}

// attempt of adding support to optimized atlases before i realized
// the other ones are better since its not gonna read it on runtime
/*public class SpriteAtlasAnimation
{
    // Animation Data
    [JsonProperty("AN")] public AtlasAnimation Animation;
    [JsonProperty("MD")] public AtlasMetadata atlasMetadata;

    public class AtlasAnimation
    {
        [JsonProperty("SN")] public string SpriteName;
        [JsonProperty("TL")] public AnimationTimeline Timeline;
    }

    public class AtlasMetadata
    {
        [JsonProperty("FRT")] public float Framerate;
    }

    public class AnimationTimeline
    {
        [JsonProperty("L")] public List<AnimationLayer> Layers = new();
    }

    public class AnimationLayer
    {
        [JsonProperty("LN")] public string LayerName;
        [JsonProperty("FR")] public List<AnimationFrame> Frames = new();
    }

    public class AnimationFrame
    {
        [JsonProperty("I")] public int FrameIndex;
        [JsonProperty("DU")] public int FrameDuration;
        [JsonProperty("E")] public List<FrameElements> Elements = new();
    }

    public class FrameElements
    {
        [JsonProperty("ASI")] public AnimationInstace Instance;
    }

    public class AnimationInstace
    {
        [JsonProperty("N")] public string SpriteFrame;
        [JsonProperty("TRP")] public TransformationPoint transformPoint;

        // my worst nightmare
        [JsonProperty("M3D")] public List<float> Matrix3D = new();
    }

    public class TransformationPoint
    {
        [JsonProperty("x")] public float X;
        [JsonProperty("y")] public float Y;
    }

    // formatted animation
    public class NicelyFormattedAnimation
    {
        
    }

    public class NiceAnimation
    {
        public Transform3D DecomposedMatrix = new();
    }
}*/

public class SpriteAtlasAnimation
{
    // Animation Data
    [JsonProperty("ANIMATION")] public AtlasAnimation Animation;
    [JsonProperty("SYMBOL_DICTIONARY")] public SymbolDictionary symbolDictionary;
    [JsonProperty("metadata")] public AtlasMetadata atlasMetadata;

    public class AtlasAnimation
    {
        [JsonProperty("TIMELINE")] public AnimationTimeline Timeline;
    }

    public class AtlasMetadata
    {
        [JsonProperty("framerate")] public float Framerate;
    }

    public class AnimationTimeline
    {
        [JsonProperty("LAYERS")] public List<AnimationLayer> Layers = new();
    }

    public class AnimationLayer
    {
        [JsonProperty("Layer_name")] public string LayerName;
        [JsonProperty("Frames")] public List<AnimationFrame> Frames = new();
    }

    public class AnimationFrame
    {
        [JsonProperty("index")] public int FrameIndex;
        [JsonProperty("duration")] public int FrameDuration;
        [JsonProperty("elements")] public List<FrameElements> Elements = new();
    }

    public class FrameElements
    {
        [JsonProperty("SYMBOL_Instance")] public AnimationInstace Instance;
    }

    public class AnimationInstace
    {
        [JsonProperty("SYMBOL_name")] public string SymbolName;
        [JsonProperty("firstFrame")] public int FirstFrame;
        [JsonProperty("transformationPoint")] public TransformationPoint transformPoint;
        [JsonProperty("DecomposedMatrix")] public DecompMatrix decompMatrix;
    }

    public class DecompMatrix
    {
        public MatrixVector Position;
        public MatrixVector Rotation;
        public MatrixVector Scaling;
    }

    public class MatrixVector
    {
        [JsonProperty("x")] public float X;
        [JsonProperty("y")] public float Y;
        [JsonProperty("z")] public float Z;
    }

    public class TransformationPoint
    {
        [JsonProperty("x")] public float X;
        [JsonProperty("y")] public float Y;
    }

    // Symbol Data

    public class SymbolDictionary
    {
        
    }
}