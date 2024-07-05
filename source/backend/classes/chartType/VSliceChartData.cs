#region Imports
	using System.Collections.Generic;
using Newtonsoft.Json;
#endregion

namespace Rubicon.Backend.Classes.ChartType;
public partial class VSliceChartData : Node
{
	//VSlice chart support (pain in the ass)

	public class RawVSliceSong
	{
		public Dictionary<string,float> scrollSpeed;
		//public List<RawVSliceEvent> events;
		public Dictionary<string,List<RawVSliceNote>> notes;
	}

	public class RawVSliceDifficulty {
		public List<RawVSliceNote> DifficultyNotes;
	}

	public class RawVSliceNote
	{
		//cryptic ass names
		public float t; //time
		public float l; //length
		public int d; //direction
		public string k; //type
	}

	public class RawVSliceEvent
	{
		public float t; //time
		public string e; //event name
		public Dictionary<string,dynamic> v; //values
	}

	/*public class RawVSliceEventData
	{
		// you cant just use char in c# and get away with it
		[JsonProperty("char")] public int character;
		public float x;
		public float y;
		public float duration;
		public string ease;
		public float zoom;
	}*/

	public partial class SongMetadata
	{
		public string artist = "Super Mario and his Brothers"; //unused for now
		public PlayMetadata playData = new();
		public string songName = "Test";
		public List<TimeMetadata> timeChanges = new();
	}

	public partial class PlayMetadata
	{
		public string stage = "stage";
		public CharacterMetadata characters = new();
		public string[] difficulties = {"easy","normal","hard"};
	}

	public partial class CharacterMetadata
	{
		public string player = "bf";
		public string girlfriend = "bf";
		public string opponent = "bf";
	}

	public partial class TimeMetadata
	{
		public float bpm = 120;
	}
}
