using System.Collections.Generic;
using System.Linq;

namespace FlashImporter.addons.flashimport.Importers;
[Tool] public partial class XMLSpritesheet : Node
{
    private string spritePath;
	private int fps;
	private bool loop;
	private bool stackFrames;
	/*private AnimatedSprite2D finalSprite;
	string[] animArray;*/
	private EditorFileDialog fileDialog;

    public void OnButtonPress()
	{
		GD.Print("Converting XML Spritesheet...");

		spritePath = GetNode<LineEdit>("SpritePath").Text;
		fps = (int)GetNode<SpinBox>("FPS").Value;
        loop = GetNode<Button>("Stack/Loop").ButtonPressed;
        stackFrames = GetNode<Button>("Stack").ButtonPressed;
		//finalSprite = GetNode<AnimatedSprite2D>("FinalSprite");

		List<string> spriteList = new();
		bool pathIsDirectory = spritePath.EndsWith("/") || !spritePath.EndsWith(".png") && !spritePath.EndsWith(".xml");

		if(pathIsDirectory)
		{
			List<string> folderSprites = ImportUtils.ListDirectory(spritePath);
			spriteList.AddRange(from sprite in folderSprites where sprite.EndsWith(".png") && FileAccess.FileExists(spritePath + sprite.GetBaseName()+".xml") select spritePath + sprite);
		}
		else
			spriteList.Add(spritePath);

		foreach(string sprite in spriteList) 
		{
			if(ResourceLoader.Exists(sprite)) 
			{
				Texture2D texture = GD.Load<Texture2D>(sprite);

				string atlas = $"{sprite.GetBaseDir() + sprite.GetBaseName()}.xml";
				XmlParser xml = new();
				xml.Open(atlas);
				GD.Print($"Sprite Path: {sprite}\nAtlas Path: {atlas}");
				ConvertSprite(texture,xml, spriteList.Count > 1);
			}
			else GD.PrintErr($"No sprite found with the given path: {sprite}");
		}
	}

    public override void _EnterTree()
    {
        fileDialog = new EditorFileDialog();
        fileDialog.Title = "Select a spritesheet";
        fileDialog.FileMode = EditorFileDialog.FileModeEnum.OpenAny;
        fileDialog.Size = new Vector2I(512, 512);
        fileDialog.InitialPosition = Window.WindowInitialPosition.CenterMainWindowScreen;
        fileDialog.Filters = ["*.png", "*.xml"];
		fileDialog.DirSelected += DirSelected;
        fileDialog.FileSelected += FileSelected;
        AddChild(fileDialog);
    }

    public override void _ExitTree()
    {
        fileDialog.DirSelected -= DirSelected;
        fileDialog.FileSelected -= FileSelected;
    }

    public void FolderButton()
	{
		fileDialog.PopupFileDialog();
    }

	public void FileSelected(string path)
	{
		GetNode<LineEdit>("SpritePath").Text = path;
    }

	public void DirSelected(string path)
	{
		if (!path.EndsWith("/")) path += "/";
        GetNode<LineEdit>("SpritePath").Text = path;
    }

	private void ConvertSprite(Texture2D texture, XmlParser xml, bool showResult = false)
	{
		SpriteFrames spriteFrame = new();
		spriteFrame.RemoveAnimation("default");

		int duppedFrameCount = 0;

		Rect2 rectDupe = new();
		AtlasTexture atlasDupe = new();

		while (xml.Read() == Error.Failed)
		{
			GD.PrintErr("Atlas failed loading at given path");
			return;
		}

		while (xml.Read() == Error.Ok)
		{
			if (xml.GetNodeType() == XmlParser.NodeType.Text) continue;
			StringName nodeName = xml.GetNodeName();

			if (nodeName != "SubTexture") continue;
			AtlasTexture frameData;

			var animName = xml.GetNamedAttributeValue("name");
			animName = animName.Left(animName.Length - 4);

			Rect2 frameRect = new(
				new(xml.GetNamedAttributeValue("x").ToFloat(), xml.GetNamedAttributeValue("y").ToFloat()),
				new(xml.GetNamedAttributeValue("width").ToFloat(), xml.GetNamedAttributeValue("height").ToFloat())
			);

			if (stackFrames && rectDupe == frameRect)
			{
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

					float[] frameSizeData = { rawFrameWidth, rawFrameHeight };
					if (frameSizeData[0] == 0 && frameSizeData[1] == 0)
					{
						frameSizeData[0] = frameRect.Size[0];
						frameSizeData[1] = frameRect.Size[1];
					}

					float[] marginPosition = { -rawFrameX, -rawFrameY };
					float[] marginSize = { rawFrameWidth - frameRect.Size[0], rawFrameHeight - frameRect.Size[1] };

					if (marginSize[0] < Math.Abs(marginPosition[0]))
						marginSize[0] = Math.Abs(marginPosition[0]);
					if (marginSize[1] < Math.Abs(marginPosition[1]))
						marginSize[1] = Math.Abs(marginPosition[1]);
					
					frameData.Margin = new(new(marginPosition[0], marginPosition[1]), new(marginSize[0], marginSize[1]));;
				}

				frameData.FilterClip = true;
				atlasDupe = frameData;
				rectDupe = frameRect;
			}

			if (!spriteFrame.HasAnimation(animName))
			{
				spriteFrame.AddAnimation(animName);
				spriteFrame.SetAnimationLoop(animName, loop);
				spriteFrame.SetAnimationSpeed(animName, fps);
			}

			spriteFrame.AddFrame(animName, frameData);
		}

		string resPath = texture.ResourcePath.GetBaseName() + ".tres";
		ResourceSaver.Save(spriteFrame, resPath);
		if (ResourceLoader.Exists(resPath))
			GD.Print($"SpriteFrame successfully created at path: {resPath}\nFound {duppedFrameCount} dupped frames.");

		/*if (showResult)
		{
			finalSprite.SpriteFrames = GD.Load<SpriteFrames>(resPath);
			animArray = finalSprite.SpriteFrames.GetAnimationNames();

			finalSprite.Play(animArray[0]);
			finalSprite.AnimationFinished += FinalSpriteFinished;
		}*/
	}

    /*public void FinalSpriteFinished()
	{
		int animIndex = 0;
		if (animIndex < animArray.Length - 1)
			finalSprite.Play(animArray[animIndex]);
		else
			finalSprite.AnimationFinished -= FinalSpriteFinished;
    }*/
}
