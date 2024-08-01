using System.Collections.Generic;
using System.Text.Json;

namespace Rubicon.addons.flashimport.Importers;
public partial class SpriteAtlas : Node
{
    private string spritePath;
    private bool scene;
    
    public void OnButtonPress()
    {
        GD.Print("Converting Sprite Atlas...");

		spritePath = GetNode<LineEdit>("SpritePath").Text;
        scene = GetNode<CheckBox>("Scene").ButtonPressed;

        if(spritePath.EndsWith(".png") || spritePath.EndsWith(".json"))
            spritePath = spritePath.GetBaseDir();
        
        if(!spritePath.EndsWith("/"))
            spritePath += "/";

        FileAccess animationFile = FileAccess.Open(spritePath+"Animation.json", FileAccess.ModeFlags.Read);
        SpriteAtlasAnimation animation = JsonSerializer.Deserialize<SpriteAtlasAnimation>(animationFile.GetAsText());
        animationFile.Close();

        var spritemapFile = FileAccess.Open(spritePath+"spritemap1.json", FileAccess.ModeFlags.Read);
        SpriteAtlasData spritemap = JsonSerializer.Deserialize<SpriteAtlasData>(spritemapFile.GetAsText());
        spritemapFile.Close();

        Texture2D texture = GD.Load<Texture2D>(spritePath+"spritemap1.png");

        if(texture != null && spritemap != null && animation != null) GD.Print($"Texture: {spritePath}spritemap1.png\nSpritemap Data{spritePath}spritemap1.json\nAnimation Data: {spritePath}Animation.json");
        else {
            if(texture == null) GD.PrintErr($"No texture found at given path: {spritePath}spritemap1.png");
            if(spritemap == null) GD.PrintErr($"No spritemap data found at given path: {spritePath}spritemap1.json");
            if(animation == null) GD.PrintErr($"No animation data found at given path: {spritePath}Animation.json");
        }

        ConvertAtlas(new List<Texture2D> {texture}, new List<SpriteAtlasAnimation> {animation}, new List<SpriteAtlasData> {spritemap});
    }

    private void ConvertAtlas(List<Texture2D> textures, List<SpriteAtlasAnimation> animations, List<SpriteAtlasData> spritemaps)
    {
        GD.Print(animations[0].AN.TL.L[0].LN);
        Animation animation = new();

    }
}