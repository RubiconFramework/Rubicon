using System.Collections.Generic;
using Godot;

namespace Rubicon.gameplay.objects.classes.song;

public partial class Section : Resource
{
    public float Bpm { get; set; }
    public bool ChangeBpm { get; set; }
    public bool IsPlayer { get; set; }
    public List<SectionNote> SectionNotes { get; set; }
    public int LengthInSteps { get; set; } = 16;
    public bool AltAnimation {get; set;} = false;
}
