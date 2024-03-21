namespace BaseRubicon.Addons.SparrowConverter;
[Tool]
public partial class SparrowConvert : EditorPlugin
{
	private static Control dockScene;
	private static LineEdit spritePath, atlasPath, fps;
	private static Button loop, optimized, convertBttn;

	public override void _EnterTree()
	{
		dockScene = GD.Load<PackedScene>("res://addons/sparrowconverter/SparrowConvert.tscn").Instantiate<Control>();

		spritePath = dockScene.GetNode<LineEdit>("Panel/SpritePath");
        atlasPath = dockScene.GetNode<LineEdit>("Panel/AtlasPath");
		fps = dockScene.GetNode <LineEdit>("Panel/FPS");
        loop = dockScene.GetNode<Button>("Panel/Loop");
        optimized = dockScene.GetNode<Button>("Panel/Optimize");

        convertBttn = dockScene.GetNode<Button>("Panel/Convert");
		convertBttn.Connect("pressed", new(this,nameof(ConvertSprite)));

        AddControlToDock(DockSlot.LeftUr, dockScene);
    }

	private void ConvertSprite()
	{
		string finalSpritePath = spritePath.Text;
		string finalAtlasPath = atlasPath.Text;

		if (finalAtlasPath == "")
			finalAtlasPath = $"{finalSpritePath.GetBaseName()}.xml";

		Texture2D texture = GD.Load<Texture2D>(finalSpritePath);
		if (texture is null)
		{
			GD.PrintErr($"Sprite failed loading at path: {finalSpritePath}");
			return;
		}

		GD.Print($"Sprite Path: {finalSpritePath}\nAtlas Path: {finalAtlasPath}");

		SpriteFrames spriteFrame = new();
		spriteFrame.RemoveAnimation("default");

		XmlParser xml = new();
		xml.Open(finalAtlasPath);

		Rect2 previousRect = new();
		AtlasTexture previousAtlas = new();

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

					if (optimized.ButtonPressed && previousRect == frameRect)
						frameData = previousAtlas;
					else
					{
						frameData = new();
						frameData.Atlas = texture;
						frameData.Region = frameRect;

						if (xml.HasAttribute("frameX"))
						{
							GD.Print("frame data exists");

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

						previousAtlas = frameData;
						previousRect = frameRect;
					}
					if (!spriteFrame.HasAnimation(animName))
					{
						spriteFrame.AddAnimation(animName);
						spriteFrame.SetAnimationLoop(animName, loop.ButtonPressed);
						spriteFrame.SetAnimationSpeed(animName, fps.Text.ToInt());
					}
					spriteFrame.AddFrame(animName, frameData);
				}
			}
		}
		GD.Print(spriteFrame);
		string resPath = finalSpritePath.GetBaseName() + ".res";
        ResourceSaver.Save(spriteFrame, resPath, ResourceSaver.SaverFlags.Compress);
		if (ResourceLoader.Exists(resPath)) GD.Print($"SpriteFrame succesfully created at path: {resPath}");
	}

	public override void _ExitTree()
	{
		RemoveControlFromDocks(dockScene);
		dockScene.QueueFree();
	}
}
