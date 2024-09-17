
namespace Charter;

[Tool]
public partial class ChartEditor : Control
{
    /*[Export] TextureRect note;
    double timer = 0;
    int noteIdx = 0;
    Rect2[] noteRegion = [new Rect2(2f,2f,157f,154f), new Rect2(159f, 2f, 157f, 154f), new Rect2(2f, 156f, 157f, 154f), new Rect2(159f, 156f, 157f, 154f)];

    public override void _Process(double delta)
    {
        base._Process(delta);

        timer += delta;
        if (timer >= 1)
        {
            timer = 0;
            
            AtlasTexture noteTex = (AtlasTexture)note.Texture;
            noteTex.Region = noteRegion[noteIdx];
            noteIdx = Mathf.Wrap(noteIdx+1,0,noteRegion.Length);
        }
        
    }*/
}
