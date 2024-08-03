using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;

namespace Rubicon.backend.misc.animatedfont;

[Tool]
[GlobalClass]
[Icon("res://assets/misc/animatedfont.png")]
public partial class AnimatedFont : Control
{
    /* Properties, you can see these in the editor */
    [Export] public SpriteFrames NormalFont
    {
        get => normalFont;
        set
        {
            normalFont = value;
            CalculateCharacterDimensions();
            QueueRedraw();
        }
    }

    [Export] public SpriteFrames BoldFont
    {
        get => boldFont;
        set
        {
            boldFont = value;
            CalculateCharacterDimensions();
            QueueRedraw();
        }
    }

    [Export(PropertyHint.MultilineText)] public string Text
    {
        get => text;
        set
        {
            if (text != value)
            {
                text = value;
                isDirty = true;
                QueueRedraw();
            }
        }
    }

    [Export] public bool IsBold
    {
        get => isBold;
        set
        {
            if (isBold != value)
            {
                isBold = value;
                isDirty = true;
                QueueRedraw();
            }
        }
    }

    [Export] public float AnimationSpeed
    {
        get => animationSpeed;
        set
        {
            if (!animationSpeed.Equals(value))
            {
                animationSpeed = value;
                isDirty = true;
                QueueRedraw();
            }
        }
    }

    [Export] public float Separation
    {
        get => separation;
        set
        {
            if (!separation.Equals(value))
            {
                separation = value;
                isDirty = true;
                QueueRedraw();
            }
        }
    }

    [Export] public Godot.Collections.Array<AnimatedFontCharacter> CharacterSeparators
    {
        get => characterSeparators;
        set
        {
            characterSeparators = value;
            isDirty = true;
            QueueRedraw();
        }
    }

    /* Variables */
    private SpriteFrames normalFont;
    private SpriteFrames boldFont;
    private string text = "";
    private bool isBold;
    private bool isDirty = true;
    private float animationSpeed = 1.0f;
    private float separation;

    private Godot.Collections.Array<AnimatedFontCharacter> characterSeparators = new();
    private readonly Dictionary<char, (float width, float height)> characterDimensions = new();

    public override void _Ready() => CalculateCharacterDimensions();

    public override void _Draw()
    {
        if (!isDirty) return;

        foreach (Node child in GetChildren())
        {
            RemoveChild(child);
            child.QueueFree();
        }

        if (normalFont == null) return;

        float xOffset = 0;
        float yOffset = 0;
        float lineHeight = 0;
        float maxWidth = 0;

        string processedText = ProcessText(text);

        foreach (char c in processedText)
        {
            switch (c)
            {
                case '\n':
                    xOffset = 0;
                    yOffset += lineHeight;
                    lineHeight = 0;
                    continue;
                case ' ':
                case '\t':
                {
                    float spaceWidth = c == ' ' ? characterDimensions[' '].width : characterDimensions[' '].width * 4;
                    xOffset += spaceWidth;
                    continue;
                }
            }

            string animationName = c.ToString().ToUpper();

            if (!characterDimensions.TryGetValue(char.ToUpper(c), out var dimensions))
                continue;

            AnimatedSprite2D charSprite = new AnimatedSprite2D();
            charSprite.SpriteFrames = isBold && boldFont != null ? boldFont : normalFont;
            charSprite.Position = new Vector2(xOffset, yOffset);
            charSprite.SpeedScale = AnimationSpeed;
            
            if (charSprite.SpriteFrames.HasAnimation(animationName)) charSprite.Play(animationName);
            else
            {
                GD.PrintErr($"Animation '{animationName}' not found. Skipping character.");
                continue;
            }
            
            AddChild(charSprite);

            float customSeparation = Separation;
            try
            {
                AnimatedFontCharacter separator = characterSeparators.FirstOrDefault(s => s != null && s.AnimationName == animationName);
                if (separator != null) customSeparation = separator.Separation;
            }
            catch (Exception e)
            {
                GD.PrintErr($"Error processing CharacterSeparators: {e.Message}");
            }

            xOffset += customSeparation;

            xOffset += dimensions.width;
            lineHeight = Math.Max(lineHeight, dimensions.height);
            maxWidth = Math.Max(maxWidth, xOffset);

            if (GetChildCount() > 1000)
            {
                GD.PrintErr("Too many characters. Aborting render.");
                break;
            }
        }

        CustomMinimumSize = new Vector2(maxWidth, yOffset + lineHeight);
        Size = CustomMinimumSize;
        isDirty = false;
    }

    private string ProcessText(string input)
    {
        const string pattern = @"/?\[(-?\w+)(?::([nb]|normal|bold))?\]/?";
        return Regex.Replace(input, pattern, match =>
        {
            string fullMatch = match.Value;
            string animationName = match.Groups[1].Value.ToUpper();

            if (fullMatch.StartsWith("/[") && fullMatch.EndsWith("]/"))
                return fullMatch.Substring(1, fullMatch.Length - 2);

            SpriteFrames targetFont = match.Groups[2].Value.ToLower() is "b" or "bold" ? BoldFont : NormalFont;

            if (targetFont != null && targetFont.HasAnimation(animationName)) 
                return animationName;
            
            GD.PrintErr($"Animation '{animationName}' not found in the specified font. Keeping original text.");
            return fullMatch;
        });
    }

    private void CalculateCharacterDimensions()
    {
        characterDimensions.Clear();
        if (normalFont == null) return;

        SpriteFrames currentFont = isBold && boldFont != null ? boldFont : normalFont;

        foreach (string animationName in currentFont.GetAnimationNames())
        {
            if (animationName.Length != 1) continue;
            char character = animationName[0];
            AtlasTexture frame = (AtlasTexture)currentFont.GetFrameTexture(animationName, 0);
            characterDimensions[char.ToUpper(character)] = (frame.Region.Size.X, frame.Region.Size.Y);
        }

        if (!characterDimensions.ContainsKey(' ')) characterDimensions[' '] = characterDimensions.TryGetValue('A', out var aDimensions) 
            ? (aDimensions.width / 2, aDimensions.height) : (10, 10);
    }

    public void SetText(string newText, bool bold = false)
    {
        text = newText;
        isBold = bold;
        isDirty = true;
        QueueRedraw();
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (normalFont == null)
            warnings.Add("NormalFont is not set.");

        if (isBold && boldFont == null)
            warnings.Add("Bold is enabled but BoldFont is not set.");

        return warnings.ToArray();
    }
}
