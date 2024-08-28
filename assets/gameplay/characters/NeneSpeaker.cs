using System;
using System.Collections.Generic;
using Godot;
using Godot.Sharp.Extras;
using Rubicon.Space2D;

namespace Rubicon.Assets.Gameplay.Characters;

public partial class NeneSpeaker : Character2D
{
    [NodePath("Speaker/VizGroup")] private Node2D VizGroup;
    public List<AudioEffectSpectrumAnalyzerInstance> SpectrumList = new();
    private const float VUCOUNT = 15;
    private const float HEIGHT = 6;
    private const float FREQMAX = 11050;

    private const float MINDB = 7;

    public override void _Ready() {
        base._Ready();
        this.OnReady();
        
        for (int i = 0; i < VizGroup.GetChildCount(); i++)
        {
            int busIdx = i < 2 ? 3 : 2;
            if(i is >= 2 and <= 4)
                busIdx = 1;
            
            SpectrumList.Add((AudioEffectSpectrumAnalyzerInstance)AudioServer.GetBusEffectInstance(busIdx,0));
        }
    }

    private float prevHz;
    public override void _PhysicsProcess(double delta)
    {
        for (int i = 0; i < VizGroup.GetChildCount(); i++)
        {
            float random = GD.RandRange(5,15)*0.1f;
            float hz = random*FREQMAX/VUCOUNT;
            prevHz = hz;
            float f = SpectrumList[i].GetMagnitudeForFrequencyRange(prevHz,hz).Length();
            float energy = Mathf.Clamp(MINDB + Mathf.LinearToDb(f)/MINDB,0,1);
            float height = energy * HEIGHT;

            AnimatedSprite2D viz = VizGroup.GetChild<AnimatedSprite2D>(i);
            float lerpHeight = viz.Frame > height ? (float)Mathf.Lerp(viz.Frame, height, delta/100000) : height;
            viz.Frame = (int)Math.Floor(lerpHeight);
            //GD.Print(height);
        }
    }
}
