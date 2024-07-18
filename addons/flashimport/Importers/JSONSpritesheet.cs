using System.Collections.Generic;

namespace Rubicon.addons.flashimport.Importers;
[Tool] public partial class JSONSpritesheet : Node
{
    private static string spritePath;
	private static string fps;
	private static bool loop;
	private static bool stackFrames;

    public void OnButtonPress()
	{
		GD.Print("Converting JSON Spritesheet...");

		spritePath = GetNode<LineEdit>("SpritePath").Text;
		fps = GetNode<LineEdit>("FPS").Text;
        loop = GetNode<Button>("Loop").ButtonPressed;
        stackFrames = GetNode<Button>("Stack").ButtonPressed;

		List<string> spriteList = new();

		if(!spritePath.EndsWith(".png"))
		{
			if(!spritePath.EndsWith("/"))
			{
				spritePath += "/";
			}
			
			List<string> folderSprites = ImportUtils.ListDirectory(spritePath);
			foreach(string sprite in folderSprites){
				if(sprite.EndsWith(".png") && FileAccess.FileExists(spritePath+sprite.Replace(".png",".json")))
					spriteList.Add(spritePath+sprite);
			}
		}
		else
			spriteList.Add(spritePath);

		foreach(string sprite in spriteList) {
			if(ResourceLoader.Exists(sprite)) {
				Texture2D texture = GD.Load<Texture2D>(sprite);

				string atlas = sprite.Replace(".png",".json");

				

				GD.Print($"Sprite Path: {sprite}\nAtlas Path: {atlas}");

				//ConvertSprite(texture,json);
			}
			else
				GD.PrintErr($"No sprite found with the given path: {sprite}");
		}
	}

	/*private void ConvertSprite(Texture2D texture, XmlParser json)
	{
		SpriteFrames spriteFrame = new();
		spriteFrame.RemoveAnimation("default");

		int duppedFrameCount = 0;

		Rect2 rectDupe = new();
		AtlasTexture atlasDupe = new();

		while(xml.Read() == Error.Failed)
		{
			GD.PrintErr($"Atlas failed loading at given path");
			return;
		}

		while (xml.Read() == Error.Ok)
		{
			if (xml.GetNodeType() != XmlParser.NodeType.Text)
			{
				StringName nodeName = xml.GetNodeName();

				if (nodeName == "SubTexture")
				{
					AtlasTexture frameData;

					var animName = xml.GetNamedAttributeValue("name");
					animName = animName.Left(animName.Length-4);

					Rect2 frameRect = new(
						new(xml.GetNamedAttributeValue("x").ToFloat(), xml.GetNamedAttributeValue("y").ToFloat()),
						new(xml.GetNamedAttributeValue("width").ToFloat(), xml.GetNamedAttributeValue("height").ToFloat())
						);
					

					if (stackFrames && rectDupe == frameRect) {
						duppedFrameCount++;
						frameData = atlasDupe;
					}
					else
					{
						frameData = new();
						frameData.Atlas = texture;
						frameData.Region = frameRect;

						if (xml.HasAttribute("frameX"))
						{
							int rawFrameX = xml.GetNamedAttributeValue("frameX").ToInt();
							int rawFrameY = xml.GetNamedAttributeValue("frameY").ToInt();

							int rawFrameWidth = xml.GetNamedAttributeValue("frameWidth").ToInt();
							int rawFrameHeight = xml.GetNamedAttributeValue("frameHeight").ToInt();

							Vector2 frameSizeData = new(rawFrameWidth, rawFrameHeight);
							if (frameSizeData == Vector2.Zero)
								frameSizeData = frameRect.Size;

							Rect2 margin = new(
								new(-rawFrameX, -rawFrameY),
								new(rawFrameWidth - frameRect.Size.X, rawFrameHeight - frameRect.Size.Y));

							if (margin.Size.X < Math.Abs(margin.Position.X))
								margin.Size = new(Math.Abs(margin.Position.X), margin.Size.Y);
							if (margin.Size.Y < Math.Abs(margin.Position.Y))
								margin.Size = new(margin.Size.X, Math.Abs(margin.Position.Y));

							frameData.Margin = margin;
						}

						frameData.FilterClip = true;

						atlasDupe = frameData;
						rectDupe = frameRect;
					}
					if (!spriteFrame.HasAnimation(animName))
					{
						spriteFrame.AddAnimation(animName);
						spriteFrame.SetAnimationLoop(animName, loop);
						spriteFrame.SetAnimationSpeed(animName, fps.ToInt());
					}
					spriteFrame.AddFrame(animName, frameData);
				}
			}
		}
		string resPath = texture.ResourcePath.GetBaseName() + ".tres";
        ResourceSaver.Save(spriteFrame, resPath, ResourceSaver.SaverFlags.None);
		if (ResourceLoader.Exists(resPath)) GD.Print($"SpriteFrame succesfully created at path: {resPath}\nFound {duppedFrameCount} dupped frames.");
	}*/
}

public class JsonFrameData
{

}
