using System.Diagnostics.CodeAnalysis;
using Rubicon.backend.autoload;
using Rubicon.gameplay.classes;
using Rubicon.gameplay.classes.elements;
using Rubicon.gameplay.classes.notes;
using Rubicon.gameplay.classes.strums;
using Rubicon.gameplay.resources;

namespace Rubicon.gameplay;
public partial class GameplayHUD : CanvasLayer
{
	/*	Contains all code for the HUD.
		Some methods are executed in GameplayBase.	*/

	public GameplayBase GameplayBase;

	public Healthbar HealthBar;
	[NodePath("JudgementText")] public RichTextLabel judgementText;

	[NodePath("StrumHandler")] public StrumHandler strumHandler;
	[NodePath("NoteHandler")] public NoteHandler noteHandler;

	[NodePath("JudgementGroup")] public Control JudgementGroup;
	[NodePath("JudgementGroup/JudgementTemplate")] public JudgementSprite JudgementTemplate;
	
	public StrumLine PlayerStrums;
	public StrumLine CpuStrums;

	// modcharting purposes only
	public Vector2 BasePosition { get; set; } = new(0,0);
	public bool LerpScale = true;
	public float LerpScaleSpeed = 9;
	public float LerpScaleStatic = 1;

	[SuppressMessage("ReSharper", "PossibleLossOfFraction")] //SHUT THE FUCK UP RESHARPER
	public override void _Ready()
	{
		this.OnReady();
		BasePosition = new(GetWindow().Size.X/2, GetWindow().Size.Y/2);

		base._Ready();
		// Checking that you're not being silly and using this class on a non GameplayBase inheritor
		GameplayBase = GetParentOrNull<dynamic>();
		try {
			//what do i test this with bruh i removed almost everything here
		}
		catch {
			GD.PushError("HUD is used on a class that doesnt inherit GameplayBase, which is not allowed.");
			return;
		}
	}

	public override void _Process(double delta)
	{
		if (LerpScale) 
		{
			float FinalSpeed = (float)Mathf.Clamp(delta * LerpScaleSpeed, 0, 1);
			Scale = new(Mathf.Lerp(Scale.X, LerpScaleStatic, FinalSpeed),Mathf.Lerp(Scale.Y, LerpScaleStatic, FinalSpeed));
			CenterPosition();
		}

		PositionIcon(ref HealthBar.PlayerIcon);
		PositionIcon(ref HealthBar.OpponentIcon);
	}

	private void CenterPosition() => Offset = BasePosition*-(Scale - new Vector2(LerpScaleStatic,LerpScaleStatic));

	private void PositionIcon(ref Icon icon)
	{
		float iconX = HealthBar.Size.X + (HealthBar.Size.X * (float)(-HealthBar.Value/HealthBar.MaxValue));
		icon.Position = new(iconX,0);
		icon.Position += new Vector2(icon.IsPlayer ? HealthBar.PlayerXOffset : HealthBar.OpponentXOffset, 0);
		icon.FlipH = icon.IsPlayer;
	}

	public void GenerateStrum(ref StrumLine strumline, int KeyCount, Vector2 Position, bool Playable = false)
	{
		string path = $"res://source/gameplay/resources/strumlines/{KeyCount}Keys.tscn";
		const string defaultPath = "res://source/gameplay/resources/strumlines/4Keys.tscn";
		strumline = ResourceLoader.Exists(path) ? GD.Load<PackedScene>(path).Instantiate<StrumLine>() : GD.Load<PackedScene>(defaultPath).Instantiate<StrumLine>();
		strumline.Position = Position;
		strumline.ApplyStyle(GameplayBase.Style);
		strumHandler.AddChild(strumline);

		if (Playable) strumHandler.FocusStrumline(ref strumline);
	}

	public void GenerateHealthbar(UIStyle Style)
	{
		HealthBar = Style.HealthBar.Instantiate<Healthbar>();
		AddChild(HealthBar);
		HealthBar.Value = (float)(HealthBar.MaxValue/2);

		if (RubiconSettings.Gameplay.Downscroll)
		{
			HealthBar.SetPosition(HealthBar.BarDownscrollPos);
			HealthBar.ScoreLabel.SetPosition(HealthBar.ScoreLabelDownscrollPos);
			JudgementTemplate.Position = new(JudgementTemplate.Position.X,-JudgementTemplate.Position.Y);
		}

		UpdateScoreLabel();
	}

	public void UpdateScoreLabel()
	{
		HighScore hs = GameplayBase.highScore;
		string finalRank = "N/A";
		HealthBar.ScoreLabel.Text = $"[center]Score: {hs.Score} • Breaks: {hs.Misses+hs.JudgementHitList["shit"]} • Rating: {hs.Rating} • [{finalRank}]";

		judgementText.Text =$@"[left]
		[outline_color=1e303c][color=39de65]S: {hs.JudgementHitList["sick"]}[/color][/outline_color]
		[outline_color=1c1b69][color=28b4e7]G: {hs.JudgementHitList["good"]}[/color][/outline_color]
		[outline_color=903c20][color=f0af37]B: {hs.JudgementHitList["bad"]}[/color][/outline_color]
		[outline_color=4c0a26][color=de3a38]S: {hs.JudgementHitList["shit"]}[/color][/outline_color]
		[outline_color=4c0a26][color=9448c4]M: {hs.Misses}[/color][/outline_color]
		[color=ffffff38](temporal)[/color][/left]";
	}
}
