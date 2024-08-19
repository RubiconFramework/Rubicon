using System;

namespace Rubicon.Data;

/// <summary>
/// Flags for NoteEventResult. Will prevent the action from being activated. Has Rubicon-specific flags.
/// </summary>
[Flags]
public enum RubiconNoteFlags : byte
{
    // From Promise.Framework.API.NoteEventResult
    None = 0b00000000,
    Health = 0b00000001,
    Score = 0b00000010,
    Splash = 0b00000100,
    
    // Specific to Rubicon Engine
    Animation = 0b00001000
}