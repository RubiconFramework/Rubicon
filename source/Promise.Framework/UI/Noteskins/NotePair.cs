using Godot;
using Promise.Framework;

namespace Promise.Framework.UI.Noteskins
{
    /// <summary>
    /// A resource that links to note and tail scenes. Used for noteskins.
    /// </summary>
    [GlobalClass]
    public partial class NotePair : Resource
    {
        /// <summary>
        /// A reference to the note object.
        /// </summary>
        [ExportGroup("References"), Export] public PackedScene Note;

        /// <summary>
        /// A reference to the tail object. Should contain the tail end too.
        /// </summary>
        [Export] public PackedScene Tail;
    } 
}