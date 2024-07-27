using Godot;
using System;

public partial class FolderButton : Button
{
    public void ButtonPress()
    {
        GetChild<FileDialog>(0).Visible = true;
    }
    public void DirSelected(string dir)
    {
        GetParent<LineEdit>().Text = dir;
    }
}
