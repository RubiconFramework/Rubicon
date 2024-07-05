#region Imports
using System.Collections.Generic;
#endregion

namespace FNFGodot.Backend.Classes.ChartType;
public partial class DefaultChartData : Node
{
	//raw data
	public class RawSong
	{
		public RawSongData song;
	}

	public class RawSongData
	{
		public string song;
		public double bpm;
		public string player1;
		public string player2;
		public string player3;
		public string gfVersion;
		public string gf;
		public string watcher;
		public string spectator;
		public string stage;
		public string uiStyle;
		public List<RawSection> notes;
		public int keyCount;
		public bool is3D;
		public float speed;
	}

	public class RawSection
	{
		public int lengthInSteps;
		public List<List<dynamic>> sectionNotes;
		public bool mustHitSection;
		public bool changeBPM;
		public double bpm;
		public bool altAnim;
	}
}
