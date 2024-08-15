using System.Collections.Generic;
using System.Linq;
using Godot;
using Promise.Framework;
using Rubicon.Data.Interfaces;
using Rubicon.Data.Stage;

namespace Rubicon.Space2D;

/// <summary>
/// A character class to be used in 2D spaces.
/// </summary>
public partial class Character2D : Node2D, ICharacter
{
    #region Exported Variables
    [Export] public CharacterData Data { get; set; }

    [ExportGroup("Status"), Export] public string CurrentAnimation { get; set; } = "";

    [Export] public bool Holding { get; set; } = false;
    
    [Export] public bool NoteLocked { get; set; } = false;

    [Export] public bool Losing { get; set; } = false;

    [Export] public bool Winning { get; set; } = false;
    
    [Export] public bool Missed { get; set; } = false;

    [Export] public bool Dead { get; set; } = false;

    [ExportSubgroup("Advanced"), Export] public string CurrentState { get; set; } = "";

    [Export] public string AnimationSuffix { get; set; }  = "";

    [ExportGroup("Settings"), Export] public bool Active { get; set; } = true;
    
    [Export] public bool CanIdle { get; set; } = true;

    [Export] public float SpeedScale { get; set; } = 1f;

    [Export] public int LaneCount { get; set; } = 4;

    [ExportGroup("References"), Export] public AnimationPlayer AnimationPlayer { get; set; }

    [Export] public CameraFocusPoint2D FocusPoint;
    #endregion

    #region Private Variables
    private int _lastBeat = 0;
    private int _lastStep = 0;
    private bool _lastWinning = false;
    private bool _lastLosing = false;
    private bool _justChangedState = false;
    #endregion

    public override void _Ready() => CurrentAnimation = Data.IdleAnimation[0];

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        int beat = Mathf.FloorToInt(Conductor.CurrentBeat);
        int step = Mathf.FloorToInt(Conductor.CurrentStep);
            
        if (CanIdle && !(NoteLocked && Data.NoteAnimations[LaneCount].Contains(CurrentState)) && !_justChangedState && beat != _lastBeat && beat % Data.IdleBeat == 0)
            PlayIdleAnim();

        bool regularNoteHolding = !Data.StepRepeat && AnimationPlayer.CurrentAnimationPosition >= AnimationPlayer.CurrentAnimationLength * 0.125f;
        bool stepNoteHolding = Data.StepRepeat && step != _lastStep;
        if (!_justChangedState && Data.NoteAnimations[LaneCount].Contains(CurrentState) && Holding && (regularNoteHolding || stepNoteHolding))
            AnimationPlayer.Seek(0f, true);

        if (((Winning != _lastWinning && Data.Animations.Count(x => x.OverrideAnimation == CurrentAnimation && x.Winning) > 0) || (Losing != _lastLosing && Data.Animations.Count(x => x.OverrideAnimation == CurrentAnimation && x.Losing) > 0)) && !Dead)
            PrepareAndPlay(CurrentAnimation, Missed, (float)AnimationPlayer.CurrentAnimationPosition);

        _lastBeat = beat;
        _lastStep = step;
        _lastWinning = Winning;
        _lastLosing = Losing;
        _justChangedState = false;
    }
    
    public void PlayIdleAnim()
    {
        // this for the sans loopinng shit
        string currentIdle = Data.IdleAnimation[Mathf.FloorToInt(Mathf.Abs(Conductor.CurrentBeat)) / Data.IdleBeat % Data.IdleAnimation.Length];
        if (AnimationPlayer.GetAnimation(currentIdle).LoopMode != Animation.LoopModeEnum.None && CurrentAnimation == currentIdle) 
            return;

        PrepareAndPlay(currentIdle);
    }
    
    public void PlayNoteAnim(int note, bool miss = false)
    {
        Missed = miss;

        note %= Data.NoteAnimations[LaneCount].Length;
        PrepareAndPlay(Data.NoteAnimations[LaneCount][note], miss);
    }
    
    public bool PrepareAndPlay(string state, bool miss = false, double time = 0d)
    {
        string anim = state;

        CharacterAnimation nextAnim = null;
        if (Data.Animations.Count(x => x.Name == state + AnimationSuffix) > 0)
            state = anim + AnimationSuffix;

        IEnumerable<CharacterAnimation> next = Data.Animations.Where(x => x.Name == state);
        if (next.Any())
            nextAnim = next.First();

        if (nextAnim != null && Scale.X < 0f && !string.IsNullOrEmpty(nextAnim.FlipXOverride))
        {
            state = nextAnim.FlipXOverride;
            nextAnim = Data.Animations.First(x => x.Name == state);
        }

        for (int i = 0; i < Data.Animations.Length; i++)
        {
            if (!string.IsNullOrEmpty(Data.Animations[i].OverrideAnimation) && Data.Animations[i].OverrideAnimation == anim)
            {
                if ((miss && Data.Animations[i].Miss)
                    || (Losing && Data.Animations[i].Losing)
                    || (Winning && Data.Animations[i].Winning))
                {
                    nextAnim = Data.Animations[i];
                    GD.Print($"{Name}: Overriding {state} with {nextAnim.Name}; Miss = {miss && Data.Animations[i].Miss}, Losing = {Losing && Data.Animations[i].Losing}, Winning: {Winning && Data.Animations[i].Winning}");
                    state = nextAnim.Name;
                    break;
                }
            }
        }

        if (nextAnim == null)
        {
            GD.Print($"Next animation data {state} is null!");
            return false;
        }

        if (PlayAnimation(state, time))
        {
            CurrentAnimation = anim;
            return true;
        }

        return false;
    }
    
    public bool PlayAnimation(string state, double time = 0d)
    {
        CharacterAnimation anim = Data.Animations.First(x => x.Name == CurrentAnimation);

        string nextAnim = Data.Animations.Count(x => x.FlipXOverride == state) > 0 ? Data.Animations.First(x => x.FlipXOverride == state).Name : state;

        if ((!anim.NoteInterrupt && Data.NoteAnimations[LaneCount].Contains(nextAnim) && !Data.NoteAnimations[LaneCount].Contains(CurrentAnimation))
            || (!anim.IdleInterrupt && Data.IdleAnimation.Contains(nextAnim) && !Data.IdleAnimation.Contains(CurrentAnimation) && !Data.NoteAnimations[LaneCount].Contains(nextAnim)))
        {
            if (AnimationPlayer.CurrentAnimationPosition < AnimationPlayer.CurrentAnimationLength)
                return false;
        }

        CurrentState = state;
        _justChangedState = true;

        AnimationPlayer.SpeedScale = SpeedScale;
        AnimationPlayer.Play(state);
        AnimationPlayer.Seek(time, true);

        return true;
    }
    
    public ICharacter GetDeathCharacter()
    {
        return Data.DeathCharacter == null ? this : Data.DeathCharacter.Instantiate<Character2D>();
    }
}