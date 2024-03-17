using System.Collections.Generic;
using Rubicon.gameplay.elements.scripts;
using Rubicon.gameplay.elements.strumlines;
using Global = Rubicon.backend.autoload.Global;
using Section = Rubicon.gameplay.elements.classes.song.Section;
using SectionNote = Rubicon.gameplay.elements.classes.song.SectionNote;

namespace Rubicon.gameplay;

public partial class GameplayScene
{
    private void InitializeStrumGroups()
    {
        InitializeStrumLine(ref oppStrums, (Global.windowSize.X * 0.5f) - 320f);
        InitializeStrumLine(ref playerStrums, (Global.windowSize.X * 0.5f) + 320f);
    }

    private void InitializeStrumLine(ref StrumLine strumLine, float positionX)
    {
        strumLine = GD.Load<PackedScene>($"res://src/gameplay/elements/strumlines/{Song.KeyCount}K.tscn").Instantiate<StrumLine>();
        strumLine.uiStyle = uiStyle;
        strumGroup.AddChild(strumLine);
        strumLine.Position = new(positionX, 100);
        if (strumLine == playerStrums) strumLine.readsInput = true;
    }

    public void GenerateNotes(float skipTime = -1f)
    {
        foreach (Section section in Song.Sections)
        {
            foreach (SectionNote note in section.SectionNotes)
            {
                if (note.Time > skipTime) continue;

                SectionNote newNote = (SectionNote)note.Duplicate();

                string noteTypePath = $"res://assets/gameplay/notes/{note.Type.ToLower()}/";
                IEnumerable<string> noteTypeDir = Global.FilesInDirectory(noteTypePath);
                foreach (string file in noteTypeDir)
                {
                    if (!CachedNotes.ContainsKey(note.Type) && (file.EndsWith(".tscn") || file.EndsWith(".remap"))) 
                        CachedNotes[note.Type] = GD.Load<Note>(noteTypePath + file.Replace(".remap", ""));
                }

                NoteData.Add(newNote);
            }
        }
        NoteData.Sort((a, b) => a.Time.CompareTo(b.Time));

        foreach (SectionNote note in NoteData) GD.Print(note.Time);
    }
}
