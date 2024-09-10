using Godot.Sharp.Extras;

namespace Rubicon.Core;

[GlobalClass]
public partial class HUDStyle : Resource
{
    //[Export] public PackedScene HealthBar { get; set; } = GD.Load<PackedScene>("res://resources/hudStyles/default/DefaultHealthbar.tscn");
	
    [ExportGroup("Notes")]
	[Export] public SpriteFrames StrumTexture { get; set; } = GD.Load<SpriteFrames>("res://assets/game/hudStyles/default/strums/defaultStrums.tres");
	[Export] public SpriteFrames DefaultNoteTexture { get; set; } = GD.Load<SpriteFrames>("res://assets/game/noteTypes/default/defaultNotes.tres");
	[Export] public bool NoteAntialiasing { get; set; } = true;
	[Export] public bool SplashAntialiasing { get; set; } = true;
	[Export] public float NoteScale { get; set; } = 0.6f;
	[Export] public float StrumScale { get; set; } = 0.6f;
	[Export] public float StrumTransparency { get; set; } = 0.75f;
	[Export] public float SplashScale { get; set; } = 0.9f;
	[Export] public float SustainXOffset { get; set; }

	[ExportGroup("Ratings")]
	[Export] public SpriteFrames RatingTexture { get; set; } = GD.Load<SpriteFrames>("res://assets/game/hudStyles/default/score/judgements.tres");
	[Export] public SpriteFrames ComboTexture { get; set; } = GD.Load<SpriteFrames>("res://assets/game/hudStyles/default/score/combo.tres");
	[Export] public bool RatingAntialiasing { get; set; } = true;
	[Export] public bool ComboAntialiasing { get; set; } = true;
	[Export] public float RatingScale { get; set; } = 0.65f;
	[Export] public float ComboScale { get; set; } = 0.5f;
	[Export] public float RatingTransparency { get; set; } = 0.75f;
	//[Export] public Judgement[] JudgementSet { get; set; }
	
	[ExportGroup("Intro")]
	[Export] public CompressedTexture2D ReadyTexture { get; set; } = GD.Load<CompressedTexture2D>("res://assets/game/hudStyles/default/intro/ready.png");
	[Export] public CompressedTexture2D SetTexture { get; set; } = GD.Load<CompressedTexture2D>("res://assets/game/hudStyles/default/intro/set.png");
	[Export] public CompressedTexture2D GoTexture { get; set; } = GD.Load<CompressedTexture2D>("res://assets/game/hudStyles/default/intro/go.png");
	[Export] public bool CountdownAntialiasing { get; set; } = true;
	[Export] public float CountdownScale { get; set; } = 1;
	[Export] public AudioStream PrepareSound { get; set; } = GD.Load<AudioStream>("res://assets/audio/intro/default/intro3.ogg");
	[Export] public AudioStream ReadySound { get; set; } = GD.Load<AudioStream>("res://assets/audio/intro/default/intro2.ogg");
	[Export] public AudioStream SetSound { get; set; } = GD.Load<AudioStream>("res://assets/audio/intro/default/intro1.ogg");
	[Export] public AudioStream GoSound { get; set; } = GD.Load<AudioStream>("res://assets/audio/intro/default/introGo.ogg");
}