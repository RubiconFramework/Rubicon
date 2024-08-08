using System;

namespace Promise.Framework.API
{
    /// <summary>
    /// An attribute to put on an INoteScript instance to allow usage on a specific note type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NoteTypeBind : Attribute
    {
        public string NoteType { get; private set; }

        public NoteTypeBind(string type)
        {
            NoteType = type;
        }
    }
}