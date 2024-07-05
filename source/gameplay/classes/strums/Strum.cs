
using Godot.Collections;

namespace FNFGodot.Gameplay.Classes.Strums;
public partial class Strum : AnimatedSprite2D
{
	public bool Pressed = false;
    private bool IsPlayingAnim = false;
	public float idleTransparency = 0.7f;
    public int Direction = 0;
    [Export] public bool OptimizedStrum = true;
    [Export] public Dictionary<string,Color> optimizedStrumColor = new();
    [Export] public Dictionary<string,float> optimizedStrumRotation = new();

    public override void _Ready()
    {
        if(OptimizedStrum) RotationDegrees = optimizedStrumRotation[Name];
        Connect("animation_finished", Callable.From(GoStatic));
    }

    public void GoStatic() {
        if(GetParent<StrumLine>().AutoPlay) PlayAnim("static",true);
    }

	public void PlayAnim(string anim, bool force = false)
	{
        if(IsPlayingAnim && !force) return;

        IsPlayingAnim = true;
		Frame = 0;
        Offset = new Vector2(0,0);
		string direction = Name.ToString().ToLower();
		switch (anim)
        {
            case "confirm" or "glow" or "hit":
                Play(OptimizedStrum ? "confirm" : $"{direction} confirm");
                
                if(OptimizedStrum) Modulate = optimizedStrumColor[direction];
                Modulate = new(Modulate.R,Modulate.G,Modulate.B,1);
                
                Offset = new Vector2(1,1);
                break;
            case "press" or "pressed":
                Play(OptimizedStrum ? "pressed" : $"{direction} pressed");

                if(OptimizedStrum) Modulate = optimizedStrumColor[direction];
                Modulate = new(Modulate.R,Modulate.G,Modulate.B,idleTransparency);

                break;
            default:
                Play(OptimizedStrum ? "static" : $"{direction} static");

                Modulate = new(1,1,1,idleTransparency);

                IsPlayingAnim = false;
                break;
        }
	}

    public void PlaySplash(string splashName, int animRange = 1) {
        AnimatedSprite2D splash = GetNodeOrNull<AnimatedSprite2D>(splashName);
        if(splash is not null) {
            splash.Frame = 0;
            splash.Visible = true;
            splash.Play($"splash {Name} {GD.RandRange(1,animRange)}");
        }
    }

    public void PlaySusSplash(string susSplashName, string anim, Vector2 offset) {
        AnimatedSprite2D splash = GetNodeOrNull<AnimatedSprite2D>(susSplashName);
        if(splash is not null) {
            if(!anim.EndsWith("end")) {
                splash.Frame = 0;
                splash.Visible = true;
            }
            splash.Offset = offset;
            splash.Play(anim);
        }
    }
}

public partial class OptimizedStrumData
{
    [Export] public Color color = new();
    [Export] public float rotation;
}
