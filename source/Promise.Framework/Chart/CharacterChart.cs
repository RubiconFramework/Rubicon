using System;
using Godot;

namespace Promise.Framework.Chart
{
    /// <summary>
    /// A class to store data for individual charts (referred to as "strumlines" for some).
    /// </summary>
    [GlobalClass]
    public partial class CharacterChart : RefCounted
    {
        /// <summary>
        /// If enabled, the chart will visibly appear on the screen.
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// Character list. Use Chart Events to switch chart animation target.
        /// </summary>
        public string[] Characters = Array.Empty<string>();
        
        /// <summary>
        /// Points to which spawn point the characters will spawn at.
        /// </summary>
        public int SpawnPointIndex = 0;

        /// <summary>
        /// How many lanes this specific chart will have.
        /// </summary>
        public int Lanes = 4;

        /// <summary>
        /// Stores the data for all notes in an array. Is used to generate notes in-game.
        /// </summary>
        public NoteData[] Notes = Array.Empty<NoteData>();
    }   
}