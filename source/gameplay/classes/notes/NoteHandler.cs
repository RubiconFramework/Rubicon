using System.Collections.Generic;
using Rubicon.backend.autoload;
using Rubicon.gameplay.classes.elements;
using Rubicon.gameplay.classes.strums;
using Rubicon.gameplay.resources;

namespace Rubicon.gameplay.classes.notes;

[GlobalClass]
public partial class NoteHandler : Node
{
    /*	Handles notes behavior, like going out of bounds,
        getting hit and all that. */

    [NodePath("../../")] public GameplayBase Gameplay;
    public List<Note> NotesToHit = new();
    private float NoteDeadZone = 3.5f;
    private float NoteMissZone = 1;
    private float NoteSafeZone = 10;
    private float NoteSafeZoneOffset;

    public delegate void NoteEvents(Note note);
    public event NoteEvents OnNoteHit;
    public event NoteEvents OnNoteMiss;

    public override void _Ready()
    {
        this.OnReady();
        NoteSafeZoneOffset = NoteSafeZone / 60 * 1000;
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (dynamic character in Gameplay.CharGroup.GetChildren())
        {
            if (!character.StaticSustain)
                character.HoldAnimTimer += (float)delta * 1000;
        }
    }

    public override void _Process(double delta)
    {
        foreach (Note note in GetChildren())
        {
            Strum strum = note.CurrentStrumline.GetChild<Strum>(note.Direction);
            bool ShouldMove = !(note.IsSustainNote && !note.WasMissed);

            if (ShouldMove)
                note.Position = new Vector2(strum.GlobalPosition.X, (float)(strum.GlobalPosition.Y - (Conductor.SongPosition - note.Time) * (0.45f * note.DownscrollMultiplier) * Gameplay.ScrollSpeed));
            else
                note.GlobalPosition = strum.GlobalPosition;

            if (!note.CurrentStrumline.AutoPlay)
            {
                float FinalSafeZone = NoteSafeZoneOffset * 1.2f * note.HitWindowMult;
                note.TimeToHit = note.Time > Conductor.SongPosition - (FinalSafeZone / 2) && note.Time < Conductor.SongPosition + FinalSafeZone && !note.WasHit;
                if (note.TimeToHit && !note.CurrentStrumline.NotesToHit.Contains(note)) note.CurrentStrumline.NotesToHit.Add(note);
            }
            else
            {
                note.TimeToHit = note.Time <= Conductor.SongPosition;
                if (note.TimeToHit && !note.IsSustainNote)
                {
                    strum.Pressed = true;
                    NoteHit(note);
                }
            }

            if (note.InitialLength > 50)
                note.ScrollSpeed = Gameplay.ScrollSpeed;
            if (note.WasHit)
            {
                if (note.InitialLength > 50 && note.SustainLength > -150)
                    note.SustainLength -= (float)delta * 1000;
                if (!note.IsSustainNote && note.SustainLength > 50)
                    note.IsSustainNote = true;

                if (note.IsSustainNote)
                {
                    if (!note.CurrentStrumline.AutoPlay && !strum.Pressed && !note.WasMissed && note.SustainLength > 80)
                    {
                        note.WasMissed = true;
                        NoteMiss(note);
                    }

                    if (!note.WasMissed && (note.CurrentStrumline.AutoPlay || !note.CurrentStrumline.AutoPlay && strum.Pressed))
                    {
                        note.HoldTime += (float)delta * 1000;
                        if (note.CurrentStrumline.FocusedCharacters[0].HoldAnimTimer >= Conductor.StepDuration && note.SustainLength >= 20)
                        {
                            NoteHit(note);
                        }
                    }

                    if (note.SustainLength <= 80 && (!strum.Pressed || note.SustainLength <= 20) && !note.WasMissed)
                    {
                        strum.PlaySusSplash($"{note.RawType}Sustain", $"{strum.Name} end", note.SustainSplashOffset["end"]);
                        if (note.CurrentStrumline.AutoPlay) strum.Pressed = false;
                    }
                }
            }

            if (note.Time <= Conductor.SongPosition - NoteMissZone * 100 && note.ShouldHit && !note.WasHit && !note.WasMissed)
                NoteMiss(note);

            if (note.Time + note.InitialLength <= Conductor.SongPosition - NoteDeadZone * 100)
                note.CurrentStrumline.RemoveFromList(note);
        }
    }

    public void NoteHit(Note note)
    {
        Strum strum = note.CurrentStrumline.GetChild<Strum>(note.Direction);

        if (note.InitialLength > 50 && !note.IsSustainNote)
            strum.PlaySusSplash($"{note.RawType}Sustain", "start", note.SustainSplashOffset["start"]);

        if (!note.IsSustainNote)
        {
            note.HitTime = (float)Conductor.SongPosition - note.Time;

            if (!note.CurrentStrumline.AutoPlay)
            {
                Judgement judgement = HighScore.JudgeHit(Mathf.Abs(note.HitTime));
                Gameplay.highScore.JudgementHitList[judgement.JudgementName.ToLower()]++;

                JudgementSprite judgementSprite = Gameplay.Hud.JudgementTemplate.Duplicate() as JudgementSprite;
                Gameplay.Hud.JudgementGroup.AddChild(judgementSprite);
                judgementSprite?.StartJudgement(judgement, new Vector2(5, 70), new Vector2(-5, -25));

                Gameplay.highScore.Score += judgement.ScoreAdd;
                Gameplay.Hud.HealthBar.Value += judgement.HealthAdd * note.HealthGain;
                Gameplay.Hud.UpdateScoreLabel();

                if (Gameplay.songGroup.HasNode("VocalsOpponent"))
                    Gameplay.songGroup.GetNode<AudioStreamPlayer>("Vocals").VolumeDb = 0;

                if (!judgement.ValidHit)
                {
                    note.WasMissed = true;
                    if (note.InitialLength > 50) strum.GetNode<AnimatedSprite2D>($"{note.RawType}Sustain").Visible = false;
                    NoteMiss(note, true, false);
                    return;
                }
                else strum.PlayAnim("confirm", true);

                if (judgement.ShouldSplash)
                {
                    strum.PlaySplash($"{note.RawType}Splash", note.SplashAnimRange);
                }
            }
            note.WasHit = true;
        }

        if (note.CurrentStrumline.AutoPlay || note.IsSustainNote)
            strum.PlayAnim("confirm", true);

        foreach (dynamic character in note.CurrentStrumline.FocusedCharacters)
        {
            if (character.StaticSustain && note.IsSustainNote) return;
            character.PlayAnim($"sing{strum.Name.ToString().ToUpper()}", false);
            character.SingTimer = 0;
            character.HoldAnimTimer = 0;
        }

        OnNoteHit?.Invoke(note);

        note.Sprite.Visible = false;
        if (note.SustainLength <= 0)
        {
            note.CurrentStrumline.RemoveFromList(note);
        }
    }

    public void NoteMiss(Note note, bool PlayAnimation = true, bool FakeMiss = false)
    {
        GD.Print("missed note");

        note.WasMissed = true;
        if (PlayAnimation)
            foreach (dynamic character in note.CurrentStrumline.FocusedCharacters)
            {
                character.PlayAnim($"sing{note.CurrentStrumline.GetChild(note.Direction).Name.ToString().ToUpper()}miss", false);
                character.SingTimer = 0;
            }
        if (!note.CurrentStrumline.AutoPlay)
        {
            Gameplay.Hud.HealthBar.Value -= Gameplay.HealthLossOnMiss * note.HealthLoss;

            if (!FakeMiss)
            {
                Gameplay.highScore.Misses++;
            }
            Gameplay.Hud.UpdateScoreLabel();

            if (Gameplay.songGroup.HasNode("VocalsOpponent") && !FakeMiss)
                Gameplay.songGroup.GetNode<AudioStreamPlayer>("Vocals").VolumeDb = Mathf.LinearToDb(0);
        }

        if (note.IsSustainNote && note.WasHit)
        {
            note.Time += note.HoldTime;
            Strum strum = note.CurrentStrumline.GetChild<Strum>(note.Direction);
            strum.GetNode<AnimatedSprite2D>($"{note.RawType}Sustain").Visible = false;
        }

        OnNoteMiss?.Invoke(note);
    }
}