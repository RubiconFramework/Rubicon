using System.Collections.Generic;
using Newtonsoft.Json;
using static FlashImporter.Importers.SpriteAtlasAnimation;
using static FlashImporter.Importers.SpriteAtlasData;

namespace FlashImporter.Importers;
public partial class SpriteAtlas : Node
{
    private string spritePath;
    
    public void OnButtonPress()
    {
        GD.Print("Converting Sprite Atlas...");

		spritePath = GetNode<LineEdit>("SpritePath").Text;

        if(spritePath.EndsWith(".png") || spritePath.EndsWith(".json"))
            spritePath = spritePath.GetBaseDir();
        
        if(!spritePath.EndsWith("/"))
            spritePath += "/";

        FileAccess animationFile = FileAccess.Open(spritePath+"Animation.json", FileAccess.ModeFlags.Read);
        SpriteAtlasAnimation animation = JsonConvert.DeserializeObject<SpriteAtlasAnimation>(animationFile.GetAsText());
        animationFile.Close();

        var spritemapFile = FileAccess.Open(spritePath+"spritemap1.json", FileAccess.ModeFlags.Read);
        SpriteAtlasData spritemap = JsonConvert.DeserializeObject<SpriteAtlasData>(spritemapFile.GetAsText());
        spritemapFile.Close();

        Texture2D texture = GD.Load<Texture2D>(spritePath+"spritemap1.png");

        if(texture != null && spritemap != null && animation != null) GD.Print($"Texture: {spritePath}spritemap1.png\nSpritemap Data{spritePath}spritemap1.json\nAnimation Data: {spritePath}Animation.json");
        else {
            if(texture == null) GD.PrintErr($"No texture found at given path: {spritePath}spritemap1.png");
            if(spritemap == null) GD.PrintErr($"No spritemap data found at given path: {spritePath}spritemap1.json");
            if(animation == null) GD.PrintErr($"No animation data found at given path: {spritePath}Animation.json");
        }

        ConvertAtlas(spritePath, texture, animation, spritemap);
    }

    private void ConvertAtlas(string folder, Texture2D texture, SpriteAtlasAnimation animation, SpriteAtlasData spritemap)
    {
        // Sprite parsing
        ConvertSpritemap(texture,spritemap);

        
        // Animation parsing
        float snapTime = 1/animation.atlasMetadata.Framerate;
        Animation newAnim = new();
        Node newNode = new();
        int longestFrame = 0;

        foreach(AnimationLayer layer in animation.Animation.Timeline.Layers) {
            foreach(AnimationFrame frame in layer.Frames)
            {
                double frameTime = snapTime * frame.FrameIndex;
                if(frame.FrameIndex > longestFrame) longestFrame = frame.FrameIndex;

                foreach(FrameElements element in frame.Elements)
                {
                    string trackPath = $"../Animation/{element.Instance.SymbolName}";

                    int spriteAnim = newAnim.AddTrack(Animation.TrackType.Animation);
                    newAnim.TrackSetPath(spriteAnim,trackPath+":visible");
                    newAnim.TrackInsertKey(spriteAnim,frameTime,true);
                    newAnim.TrackInsertKey(spriteAnim,frameTime+(snapTime*frame.FrameDuration),false);
                    
                    int posAnim = newAnim.AddTrack(Animation.TrackType.Animation);
                    newAnim.TrackSetInterpolationType(posAnim,Animation.InterpolationType.Nearest);
                    newAnim.TrackSetPath(posAnim,trackPath+":position");
                    newAnim.TrackInsertKey(posAnim,frameTime,new Vector2(
                        element.Instance.decompMatrix.Position.X,
                        element.Instance.decompMatrix.Position.Y
                    ));
                    
                    int rotAnim = newAnim.AddTrack(Animation.TrackType.Animation);
                    newAnim.TrackSetInterpolationType(rotAnim,Animation.InterpolationType.Nearest);
                    newAnim.TrackSetPath(rotAnim,trackPath+":rotation");
                    newAnim.TrackInsertKey(rotAnim,frameTime, element.Instance.decompMatrix.Rotation.X);

                    int scaleAnim = newAnim.AddTrack(Animation.TrackType.Animation);
                    newAnim.TrackSetInterpolationType(scaleAnim,Animation.InterpolationType.Nearest);
                    newAnim.TrackSetPath(scaleAnim,trackPath+":scale");
                    newAnim.TrackInsertKey(scaleAnim,frameTime,new Vector2(
                        element.Instance.decompMatrix.Scaling.X,
                        element.Instance.decompMatrix.Scaling.Y
                    ));
                }
            }
        }
        newAnim.Length = snapTime * longestFrame;
        Error saveResult = ResourceSaver.Save(newAnim,folder+"Animation.tres");
        if(saveResult == Error.Ok) GD.Print("Animation was converted succesfully.");
        else GD.PrintErr("An error ocurred when converting the animation.");
    }

    private void ConvertSpritemap(Texture2D tex, SpriteAtlasData map)
    {
        SpriteFrames spriteFrame = new();
        spriteFrame.RemoveAnimation("default");

        foreach (AtlasSprites spriteData in map.atlas.AtlasSprites)
        {
            //idk easier access ig
            AtlasSprite sprite = spriteData.Sprite;

            Rect2 frameRect = new(
                new(sprite.X,sprite.Y),
                new(sprite.Width,sprite.Height)
            );

            AtlasTexture frameData = new()
            {
                Atlas = tex,
                Region = frameRect
            };

            if(!spriteFrame.HasAnimation(sprite.SpriteName))
            {
                spriteFrame.AddAnimation(sprite.SpriteName);
                spriteFrame.SetAnimationLoop(sprite.SpriteName, false);
                spriteFrame.SetAnimationSpeed(sprite.SpriteName,1);
                spriteFrame.AddFrame(sprite.SpriteName, frameData);
            }
        }
        string resPath = tex.ResourcePath.GetBaseName() + ".tres";
        ResourceSaver.Save(spriteFrame, resPath);
        if (ResourceLoader.Exists(resPath))
			GD.Print($"Spritemap successfully parsed to path: {resPath}");
    }
}