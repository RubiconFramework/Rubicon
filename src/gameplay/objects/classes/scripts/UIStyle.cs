namespace Rubicon.gameplay.objects.classes.scripts;

public partial class UIStyle : Resource
{
	[ExportGroup("Textures")]
	[Export]
	public SpriteFrames strumTexture { get; set; } = GD.Load<SpriteFrames>("res://assets/gameplay/hud/default/strums/assets.res");
	[Export]
	public SpriteFrames noteTexture { get; set; } = GD.Load<SpriteFrames>("res://assets/gameplay/notes/default/assets/default/assets.res");
	[Export]
	public SpriteFrames sustainTexture { get; set; } = GD.Load<SpriteFrames>("res://assets/gameplay/hud/default/strums/splashes.res");
	[Export]
	public int splashAnims { get; set; } = 2;

	[Export]
	public SpriteFrames ratingTexture { get; set; } = GD.Load<SpriteFrames>("res://assets/gameplay/hud/default/score/judgements.res");
	[Export]
	public SpriteFrames comboTexture { get; set; } = GD.Load<SpriteFrames>("res://assets/gameplay/hud/default/score/combo.res");
	
	[Export]
	public CompressedTexture2D readyTexture { get; set; } = GD.Load<CompressedTexture2D>("res://assets/gameplay/hud/default/countdown/ready.png");
	[Export]
	public CompressedTexture2D setTexture { get; set; } = GD.Load<CompressedTexture2D>("res://assets/gameplay/hud/default/countdown/set.png");
	[Export]
	public CompressedTexture2D goTexture { get; set; } = GD.Load<CompressedTexture2D>("res://assets/gameplay/hud/default/countdown/go.png");

	[ExportGroup("Scale")]
	[Export]
	public float noteScale { get; set; } = 0.6f;
	[Export]
	public float splashScale { get; set; } = 0.9f;
	[Export]
	public float sustainXOffset { get; set; }

	[Export]
	public float ratingScale { get; set; } = 0.7f;
	[Export]
	public float comboScale { get; set; } = 0.5f;
	[Export]
	public float countdownScale { get; set; } = 1f;

	[ExportGroup("Antialiasing")]
	[Export]
	public bool noteFilter { get; set; } = true;
	[Export]
	public bool splashFilter { get; set; } = true;
	[Export]
	public bool ratingFilter { get; set; } = true;
	[Export]
	public bool comboFilter { get; set; } = true;
	[Export]
	public bool countdownFilter { get; set; } = true;

	[ExportGroup("Sounds")]
	[Export]
	public AudioStream prepareSound { get; set; } = GD.Load<AudioStream>("res://assets/sounds/default/countdown/intro3.ogg");
	[Export]
	public AudioStream readySound { get; set; } = GD.Load<AudioStream>("res://assets/sounds/default/countdown/intro2.ogg");
	[Export]
	public AudioStream setSound { get; set; } = GD.Load<AudioStream>("res://assets/sounds/default/countdown/intro1.ogg");
	[Export]
	public AudioStream goSound { get; set; } = GD.Load<AudioStream>("res://assets/sounds/default/countdown/introGo.ogg");
}
