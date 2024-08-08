using System;
using Godot;

namespace Rubicon.Game.Data
{
    /// <summary>
    /// Contains information about the characters.
    /// </summary>
    [GlobalClass]
    public partial class CharacterChart : RefCounted
    {
        /// <summary>
        /// The character list to spawn.
        /// </summary>
        public string[] Characters = Array.Empty<string>();

        /// <summary>
        /// The index where the game will spawn these characters at.
        /// </summary>
        public int SpawnPointIndex = 0;
    }
}