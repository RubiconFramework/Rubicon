using System.Collections.Generic;

namespace Rubicon.Backend.UI.Alphabet;

[Tool]
[Icon("res://assets/miscIcons/alphabet.png")]
public partial class Alphabet : ReferenceRect
{
    [Export] public bool bold = true;
    [Export(PropertyHint.MultilineText)] public string text = "";
    [Export] public bool isMenuItem;
    [Export] public int targetY;

    private readonly List<List<Texture>> letterTextures = new();
    private float totalWidth;
    private float totalHeight;
    private int forceX;

    private const float LetterSpacing = 25.0f;
    private const float LineSpacing = 60.0f;
    private const float AnimationLerpRate = 0.1f;
    private const float AnimationScaleFactor = 1.3f;
    private const float InitialPositionY = 720 * 0.48f;

    private readonly Dictionary<char, string> letterAnimations = new();
    private readonly Dictionary<char, float> letterYOffsets = new();
    
    private readonly Dictionary<char, (string anim, float offset)> boldCharacterData = new()
    {
        { '?', ("-question mark-", -10.0f) }, { '!', ("-exclamation point-", -10.0f) },
        { '\'', ("-apostrophe-", -5.0f) },
        { '\"', ("-end quote-", -5.0f) }, { '-', ("-dash-", 20.0f) }, { '*', ("-multiply x-", 20.0f) },
        { '.', ("-period-", 40.0f) }, { ',', ("-comma-", 40.0f) }, { '~', ("", 20.0f) },
        { '\\', ("-back slash-", 0.0f) }, { '/', ("-forward slash-", 0.0f) }
    };

    private readonly Dictionary<char, (string anim, float offset)> nonBoldCharacterData = new()
    {
        { '\'', ("-apostrophe-", 0.0f) }, { '\\', ("-back slash-", 0.0f) }, { '/', ("-forward slash-", 0.0f) },
        { '\"', ("-end quote-", 0.0f) }, { '?', ("-question mark-", 0.0f) }, { '!', ("-exclamation point-", 0.0f) },
        { '.', ("-period-", 42.0f) }, { ',', ("-comma-", 42.0f) }, { '-', ("-dash-", 14.0f) },
        { '←', ("-left arrow-", 5.0f) }, { '↓', ("-down arrow-", 5.0f) }, { '↑', ("-up arrow-", 5.0f) },
        { '→', ("-right arrow-", 5.0f) }
    };

    private readonly Dictionary<char, float> commonOffsets = new()
    {
        { 'A', 30.0f }, { 'B', 17.0f }, { 'F', 17.0f }, { 'C', 30.0f }, { 'D', 15.0f }, { 'H', 20.0f },
        { 'T', 25.0f }, { 'I', 25.0f }, { 'J', 25.0f }, { 'K', 25.0f }, { 'L', 25.0f }, { 'E', 32.5f },
        { 'G', 32.5f }, { 'M', 33.5f }, { 'N', 35.0f }, { 'O', 35.0f }, { 'P', 35.0f }, { 'Q', 35.0f },
        { 'R', 35.0f }, { 'S', 35.0f }, { 'U', 35.0f }, { 'V', 35.0f }, { 'W', 35.0f }, { 'X', 35.0f },
        { 'Y', 35.0f }, { 'Z', 35.0f }, { ':', 10.0f }, { ';', 10.0f }, { '*', 10.0f }, { '0', 10.0f },
        { '1', 10.0f }, { '2', 10.0f }, { '3', 10.0f }, { '4', 10.0f }, { '5', 10.0f }, { '6', 10.0f },
        { '7', 10.0f }, { '8', 10.0f }, { '9', 10.0f }
    };

    public override void _Ready()
    {
        letterAnimations.Clear();
        letterYOffsets.Clear();
        
        foreach (var entry in bold ? boldCharacterData : nonBoldCharacterData)
        {
            letterAnimations[entry.Key] = entry.Value.anim;
            letterYOffsets[entry.Key] = entry.Value.offset;
        }

        foreach (var entry in commonOffsets) letterYOffsets[entry.Key] = entry.Value;
        
        Node2D lettersNode = GetNode<Node2D>("Letters");
        if (lettersNode == null) return;

        foreach (Node child in lettersNode.GetChildren()) child.QueueFree();

        float xPos = 0.0f;
        float yPos = 0.0f;
        AnimatedSprite2D template = bold ? GetNode<AnimatedSprite2D>("BoldTemplate") : GetNode<AnimatedSprite2D>("DefaultTemplate");
        string[] split = text.Split('\n');

        letterTextures.Clear();
        totalWidth = 0.0f;
        totalHeight = 0.0f;

        foreach (string line in split)
        {
            letterTextures.Add(new());

            foreach (char letter in line)
            {
                if (letter == ' ')
                {
                    xPos += LetterSpacing;
                    continue;
                }

                if (!letterAnimations.TryGetValue(letter, out string anim)) continue;

                AnimatedSprite2D letterSprite = (AnimatedSprite2D)template.Duplicate();
                letterYOffsets.GetValueOrDefault(letter, 0.0f);

                letterSprite.Play(anim);
                letterSprite.Visible = true;
                lettersNode.AddChild(letterSprite);

                Texture2D texture = letterSprite.SpriteFrames.GetFrameTexture(letterSprite.Animation, 0);
                xPos += texture.GetWidth();
                letterTextures[^1].Add(texture);
            }

            yPos += LineSpacing;
            totalWidth = Mathf.Max(totalWidth, xPos);
            xPos = 0;
        }

        totalHeight = yPos + LineSpacing;
        CustomMinimumSize = new(totalWidth, totalHeight);
        Size = new(totalWidth, totalHeight);
    }

    public override void _Process(double delta)
    {
        if (!isMenuItem || Engine.IsEditorHint()) return;

        float scaledY = Mathf.Remap(targetY, 0, 1, 0, AnimationScaleFactor);
        float lerpVal = Mathf.Clamp((float)delta * 60 * AnimationLerpRate, 0, 1);

        var position = Position;
        position.Y = Mathf.Lerp(position.Y, (scaledY * LineSpacing) + InitialPositionY, lerpVal);
        position.X = forceX != 0 ? forceX : Mathf.Lerp(position.X, (targetY * 20) + 90, lerpVal);
    }

    public void ScreenCenter(string axes = "XY")
    {
        switch (axes.ToUpperInvariant())
        {
            case "X":
                Position = new Vector2((Main.WindowSize.X * 0.5f) - (totalWidth / 2f), Position.Y);
                break;
            case "Y":
                Position = new Vector2(Position.X, (Main.WindowSize.Y * 0.5f) - (totalHeight / 2f));
                break;
            default:
                Position = new Vector2((Main.WindowSize.X * 0.5f) - (totalWidth / 2f), (Main.WindowSize.Y * 0.5f) - (totalHeight / 2f));
                break;
        }
    }
}
