using System;

namespace Konkon.API
{
    /// <summary>
    /// An attribute to put on either an ISongScript or ISongCoroutine to allow usage on a specific song.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SongBind : Attribute
    {
        public string Song { get; private set; }

        public SongBind(string song)
        {
            Song = song;
        }
    }

    /// <summary>
    /// An attribute to put on either an ISongScript or ISongCoroutine to allow usage on a specific stage.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class StageBind : Attribute
    {
        public string Stage { get; private set; }

        public StageBind(string stage)
        {
            Stage = stage;
        }
    }
    
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