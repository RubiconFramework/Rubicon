namespace Rubicon.gameplay.objects.classes.chart.resources;

public partial class SectionNote : Resource
{
    public float Time { get; set; }
    public int Direction { get; set; }
    public string Type { get; set; } = "default";
    public float Length { get; set; }
    public bool AltAnim { get; set; }
    public bool PlayerSection { get; set; }
}
